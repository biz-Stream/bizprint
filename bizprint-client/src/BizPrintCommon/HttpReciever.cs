// Copyright 2024 BrainSellers.com Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace BizPrintCommon
{
    /// <summary>
    /// HTTP受信スレッド
    /// 受信結果の処理はHttpAnalyzerで行う
    /// </summary>
    public class HttpReciever
    {

        /// <summary>サーバ種別</summary>
        private int ServiceType;//Direct or Batch
        /// <summary>ポート番号</summary>
        private int PortNo;
        /// <summary>受信スレッドメイン</summary>
        private Thread ListenThread = null;
        /// <summary>スレッド起動フラグ</summary>
        public static  bool IsThreadStarted { set; get; } = false;
        /// <summary>接続ソケット</summary>
        private Socket ListenSocket = null;
        /// <summary>接続受け入れIPEndPoint</summary>
        private IPEndPoint ServiceIpEndPoint = null;
        /// <summary>設定情報管理</summary>
        public SettingManeger SettingMng { set; get; }

        /// <summary>排他制御イベント</summary>
        public static ManualResetEvent MreAllDone { set; get; } = new ManualResetEvent(false);
        /// <summary>ソケット格納リスト</summary>
        private ArrayList AcceptSocketList = new ArrayList();
        /// <summary>解析・印刷スレッド格納リスト</summary>
        private ArrayList AnalyzingList = new ArrayList();


        /// <summary>終了要求イベント</summary>
        public static ManualResetEvent MreDiscontinuanceRequest { set; get; } = new ManualResetEvent(false);
        /// <summary>終了結果</summary>
        public static ManualResetEvent MreDiscontinuanceResult { set; get; } = new ManualResetEvent(false);
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="type">サーバ種別</param>
        public HttpReciever(int type, SettingManeger mng)
        {
            ServiceType = type;
            PortNo = mng.PortNo;
            SettingMng = mng;
        }

        /// <summary>
        /// 受信スレッドの起動
        /// </summary>
        /// <returns>true:成功 false:失敗</returns>
        public bool StartListhen()
        {

            //排他制御にタイムアウトして失敗
            if (!Monitor.TryEnter(this, CommonConstants.LOCK_TIMEOUT))
            {
                if (Monitor.IsEntered(this))
                {
                    Monitor.Exit(this);
                }
                LogUtility.OutputLog("018");
                return false;
            }
            //既存スレッドが存在する
            if (ListenThread != null)
            {
                if (Monitor.IsEntered(this))
                {
                    Monitor.Exit(this);
                }
                LogUtility.OutputLog("019");
                return false;
            }
            LogUtility.OutputLog("017", PortNo.ToString());
            ServiceIpEndPoint = new IPEndPoint(IPAddress.Any, PortNo);
            IsThreadStarted = true;
            // Listen開始
            LogUtility.OutputLog("020");

            ListenThread = new Thread(new ThreadStart(LithenThreadMain));
            ListenThread.IsBackground = true;
            ListenThread.Start();

            if (Monitor.IsEntered(this))
            {
                Monitor.Exit(this);
            }

            return true;

        }

        /// <summary>
        /// 受付スレッドメイン
        /// </summary>
        private void LithenThreadMain()
        {
            try
            {
                // Socketを作成し、Bind/Listen開始
                ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ListenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                ListenSocket.Bind(ServiceIpEndPoint);
                ListenSocket.Listen(0);
                LogUtility.OutputDebugLog("E801");

                while (true)
                {
                    //停止要求が来るまではループ
                    if (MreDiscontinuanceRequest.WaitOne(10, false) == true)
                    {
                        // 現在実行中の解析・印刷がない場合のみ終われる
                        if (SearchExecuteQueAddThreadCount() == 0)
                        {
                            // 終了依頼処理を受け付けたことをセットする。
                            MreDiscontinuanceResult.Set();
                            break;
                        }

                    }
                    else
                    {
                        //動作中はこの無限ループにとどまる。
                        while (IsThreadStarted)
                        {
                            try {
                                MreAllDone.Reset();
                                ListenSocket.BeginAccept(new AsyncCallback(AcceptCallback), ListenSocket);
                                MreAllDone.WaitOne();
                            }
                            catch (Exception e) {
                                LogUtility.OutputLog("502",e.Message);
                            }
                            LogUtility.OutputDebugLog("E803");

                        }
                    }
                }
            }
            catch (ThreadAbortException th)
            {
                LogUtility.OutputLog("113", th.Message);
            }
            catch (Exception ex)
            {

                LogUtility.OutputLog("113", ex.Message);
            }
            LogUtility.OutputLog("022");

        }

        /// <summary>
        /// 個々の接続に対して呼ばれる関数
        /// </summary>
        /// <param name="ar">アクセプト時のソケット</param>
        private void AcceptCallback(IAsyncResult ar)
        {
           

            if (!Monitor.TryEnter(this, CommonConstants.LOCK_TIMEOUT))
            {
                if (Monitor.IsEntered(this))
                {
                    Monitor.Exit(this);
                }
                MreAllDone.Set();
                return;
            }
            try
            {

                //イベント受信時に時間切れ・終了スレッドのクリーンアップ。
                PrintHistoryManager.CleanUpTimeOverHistory();
                CreanUpProcTheadList();
                //ダイレクトの場合のみ、時限停止タイマーカウントのリセット
                if (ServiceType == CommonConstants.MODE_DIRECT)
                {
                    SettingManeger.UpdateLatestEvent();
                }

                Socket listener = (Socket)ar.AsyncState;
                if (listener != null)
                {

                    Socket AcceptSock = (Socket)listener.EndAccept(ar);
                    AcceptSocketList.Add(AcceptSock);

                    HttpAnalyzer analyze = new HttpAnalyzer(ServiceType, AcceptSock);
                    analyze.m_SetMng = SettingMng;
                    analyze.StartWorkThread();
                    AnalyzingList.Add(analyze);
                    LogUtility.OutputLog("021", AnalyzingList.Count.ToString());
                }

            }
            catch (Exception ex)
            {
                // ログ出力
                string errstr = ex.Message;
                //バッチでは、スレッドを強制停止するので必ずExceptionが発生する為
                if (ServiceType == CommonConstants.MODE_DIRECT)
                {
                    LogUtility.OutputLog("122", errstr);
                }

            }
            finally
            {
                if (Monitor.IsEntered(this)) { 
                    Monitor.Exit(this);
                }
            }
            MreAllDone.Set();


        }
        /// <summary>
        /// 現在実行中の解析スレッド数を返す
        /// </summary>
        /// <returns>スレッド数</returns>
        private int SearchExecuteQueAddThreadCount()
        {
            int threadCount = 0;
            CreanUpProcTheadList();

            for (int i = 0; i < AnalyzingList.Count; i++)
            {
                // スレッドが実行中
                if (((HttpAnalyzer)AnalyzingList[i]).IsRunning == true)
                {
                    threadCount++;
                }
            }

            return threadCount;
        }

        /// <summary>
        /// 終了時に呼び出される破棄処理
        /// </summary>
        public void Dispose()
        {
            //Listenスレッドを停止
            stopListenThread();
            //解析処理が残ってたら削除
            if (AnalyzingList != null && AnalyzingList.Count > 0)
            {
                AnalyzingList.Clear();

            }
            //接続ソケットのクリア
            if (AcceptSocketList != null && AcceptSocketList.Count > 0)
            {
                AcceptSocketList.Clear();
            }

        }
        /// <summary>
        /// 受信スレッド停止処理
        /// </summary>
        public void stopListenThread()
        {
            LogUtility.OutputLog("024");

            IsThreadStarted = false;
            if (!Monitor.TryEnter(this, CommonConstants.LOCK_TIMEOUT))
            {
                if (Monitor.IsEntered(this))
                {
                    Monitor.Exit(this);
                }
                String msg = String.Format("ソケット接続待ちスレッドリストの排他制御タイムアウト：{0} msec", CommonConstants.LOCK_TIMEOUT);
                //タイムアウトログ
                return;
            }
            if (ListenSocket == null)
            {
                if (Monitor.IsEntered(this))
                {
                    Monitor.Exit(this);
                }
                return;
            }

            MreAllDone.Reset();
            try
            {
                //接続待ちスレッド停止
                if (ListenThread != null)
                {
                    ListenThread.Abort();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                ListenThread = null;
            }

            //全ソケットクローズ
            //--            CloseAcceptSock();

            if (ListenSocket != null)
            {
                try
                {
                    ListenSocket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    //ここで問題が発生しても、終了してしまうのでなにも出来ない
                    string errstr = ex.Message;
                }
                finally
                {
                    ListenSocket.Close();
                }

            }
            ListenSocket = null;
            ServiceIpEndPoint = null;

            if (Monitor.IsEntered(this))
            {
                Monitor.Exit(this);
            }
        }
        /// <summary>
        /// プロセスリスト・Acceptリストのうち、終了になってるものを削除する
        /// </summary>
        private void CreanUpProcTheadList()
        {
            for (int i = 0; i < AnalyzingList.Count; i++)
            {
                // スレッドが実行中
                if (((HttpAnalyzer)AnalyzingList[i]).IsRunning == false)
                {
                    AnalyzingList.RemoveAt(i);
                }
            }
            for (int j = 0; j < AcceptSocketList.Count; j++)
            {
                try
                {

                    Socket Sock = (Socket)AcceptSocketList[j];
                    if (!Sock.Connected)
                    {
                        AcceptSocketList.RemoveAt(j);
                        Sock = null;
                    }
                }
                catch (Exception ex)
                {
                    //ここで問題が発生しても、削除してしまうのでなにも出来ない
                    string errstr = ex.Message;
                }

            }


        }
        /// <summary>
        /// 現在解析処理スレッドが動いてるか確認
        /// </summary>
        /// <returns>true:動作中、false：動作なし</returns>
        public bool IsAnalyzing()
        {
            bool rtn = false;
            CreanUpProcTheadList();
            if (AnalyzingList.Count > 0)
            {
                rtn = true;
            }
            return rtn;
        }

    }
}
