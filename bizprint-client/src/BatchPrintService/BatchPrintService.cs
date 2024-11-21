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
using BizPrintCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatchPrintServiceMain
{
    /// <summary>バッチ印刷メインフォーム(最小化、タスクバー表示なし、トレイアイコンのみのフォーム)</summary>
    public partial class BatchPrintSeviceMain : Form
    {
        /// <summary>設定マネージャ</summary>
        private static SettingManeger SettingMng;

        /// <summary>ソケット接続待ちスレッド</summary>
        private HttpReciever RecieveThread = null;
        /// <summary>印刷キュー監視スレッド</summary>
        private PrintReqProcesser PrintThread = null;
        /// <summary>
        /// ソケット接続待ちスレッドの終了結果を監視するスレッド。
        /// </summary>
        private Thread RecieveTHWatcher = null;

        /// <summary>
        /// 印刷キュー監視スレッドの終了結果を監視するスレッド。
        /// </summary>
        private Thread PrintTHWatcher = null;

        /// <summary>
        /// 再起動要求を監視するスレッド。
        /// </summary>
        private Thread RestartTHWatcher = null;

        /// <summary>
        /// 右クリックメニュー呼び出し2回目以降の強制終了フラグ
        /// </summary>
        private bool ForceCloseFlg = false;

        private BatchRestarter BRestarter = null;

        /// <summary>
        /// 自身の処理から閉じる際にtrueにする
        /// </summary>
        private Boolean closeOwnFlg = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BatchPrintSeviceMain()
        {
            InitializeComponent();

            //ログ設定初期化
            //ログ出力初期化と開始ログ出力
            string LogConfPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + ServicetConstants.BatchConfFolderName + "\\" + ServicetConstants.BPlogConfFileName;
            LogUtility.InitLog4Net(LogConfPath);

            LogUtility.InitLogUtility(ServicetConstants.BPlogIDBaseBatch, ServicetConstants.BPlogConfBaseBatch, ServicetConstants.BatchConfFolderName);
            LogUtility.OutputStaticLog("BP000", CommonConstants.LOGLEVEL_INFO, "BatchPrintService Start.");

            if (!Init())
            {
                closeOwnFlg = true;
                this.Close();
                return;
            }

        }
        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <returns>true:成功 false:失敗</returns>
        private bool Init()
        {
            bool rtn = true;
            Version OSVer = Environment.OSVersion.Version;
            //初期化ログ
            LogUtility.OutputStaticLog("BP001", CommonConstants.LOGLEVEL_INFO, CommonConstants.STATIC_LOG_001 + System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion() + " [" + Environment.Version.ToString() + "]" + LogUtility.GetWindowsVersion());
            LogUtility.OutputStaticLog("BP002", CommonConstants.LOGLEVEL_INFO, CommonConstants.STATIC_LOG_002 + System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() + " Excecute File Version=" + LogUtility.GetSelfFileVersion());
            LogUtility.OutputStaticLog("BP003", CommonConstants.LOGLEVEL_INFO, CommonConstants.STATIC_LOG_003 + System.Runtime.InteropServices.RuntimeEnvironment.SystemConfigurationFile);
            //設定ファイル読み込み
            SettingMng = new SettingManeger(CommonConstants.MODE_BATCH);
            if (!SettingMng.LoadSetting())
            {
                //設定ファイル読み込みに失敗
                LogUtility.OutputLog("014", "Can not Load Setting File");
                return false;
            }
            SettingManeger.UpdateLatestEvent();
            ErrCodeAndmErrMsg.LoadErrorDetailFile(ServicetConstants.BatchConfFolderName);

            //Staticなクラスの初期化
            WebBrowserUtil.SettingMng = SettingMng;
            PrintReqQueue.NumberOfQueMax = SettingMng.QueueMaxSize;
            PrintHistoryManager.SettingMng = SettingMng;

            //一時フォルダクリーンアップフラグがtrueなら処理を行う
            FolderCleanUp.CleanUpFolder(SettingMng.TmpFolderPath);

            try
            {
                //キュー監視スレッド起動
                PrintThread = new PrintReqProcesser(CommonConstants.MODE_BATCH, SettingMng);
                PrintThread.StartPrintProcessThread();

                //HTTP受信スレッド起動
                RecieveThread = new HttpReciever(CommonConstants.MODE_BATCH, SettingMng);
                RecieveThread.StartListhen();

            }
            catch (Exception ex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                //スレッド起動に失敗したのでエラー終了
                LogUtility.OutputLog("015", ex.Message);
                return false;
            }

            //終了イベントタイマー開始

            //HTTP受信スレッド終了結果を監視するスレッド起動
            RecieveTHWatcher = new Thread(new ThreadStart(RecieveTHEndWatch));
            RecieveTHWatcher.IsBackground = true;
            RecieveTHWatcher.Start();

            //印刷スレッド終了結果を監視するスレッド起動
            PrintTHWatcher = new Thread(new ThreadStart(printTHEndWatch));
            PrintTHWatcher.IsBackground = true;
            PrintTHWatcher.Start();

            BRestarter = new BatchRestarter();

            //再起動要求監視スレッド
            RestartTHWatcher = new Thread(new ThreadStart(RestartTHEndWatch));
            RestartTHWatcher.IsBackground = true;
            RestartTHWatcher.Start();
            return rtn;
        }
        /// <summary>
        /// 停止処理 1：Http受信スレッド停止
        /// </summary>
        private void StopHttpReciever()
        {
            if (RecieveThread != null)
            {
                HttpReciever.IsThreadStarted = false;
                HttpReciever.MreDiscontinuanceRequest.Set();
                HttpReciever.MreAllDone.Set();
            }
        }
        /// <summary>
        /// 停止処理 2：受信スレッドの終了を待ち受け、印刷キュー監視スレッド停止要求を出す
        /// </summary>
        private void RecieveTHEndWatch()
        {
            try
            {


                // ソケット接続待ちスレッドの終了結果シグナルがONになるまで待機する。
                HttpReciever.MreDiscontinuanceResult.WaitOne();

                // 印刷キュー監視監視終了セット
                StopPrintProcess();
            }
            catch (ThreadAbortException tex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = tex.Message;
            }
            catch (Exception ex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
            }
            finally
            {
                LogUtility.OutputLog("116");
            }
        }
        /// <summary>
        /// 停止処理 3：印刷キュー監視スレッドに停止要求を出す
        /// </summary>
        private void StopPrintProcess()
        {
            if (PrintThread != null)
            {
                PrintThread.IsPrintProcessThreadStarted = false;
                PrintReqProcesser.MrePrintEndRequest.Set();
                HttpReciever.MreAllDone.Set();
            }
        }
        /// <summary>
        /// 停止処理 4：印刷キュー監視スレッドの停止を待ち、アプリケーションそのものを終了させる
        /// </summary>
        private void printTHEndWatch()
        {
            try
            {


                // 印刷キュー監視スレッドの終了結果シグナルがONになるまで待機する。
                PrintReqProcesser.MrePrintEndResult.WaitOne();


            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
            }
            finally
            {
                //終了時の一時フォルダクリーンアップ
                FolderCleanUp.CleanUpFolder(SettingMng.TmpFolderPath);

                LogUtility.OutputLog("117");
                //アプリケーションを終了する
                LogUtility.OutputLog("200");
                closeOwnFlg = true;
                if (!SettingMng.DoRestartAtBatch)
                {
                    //System.InvalidOperationException回避のため
                    try
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.Close();
                            Application.Exit();
                        });
                    }
                    catch (Exception ex)
                    {
                        ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                    }
                }
                else
                {
                    //再起動する
                    LogUtility.OutputLog("209");
                    try
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            Application.Restart();
                        });
                    }
                    catch (Exception ex)
                    {
                        ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                    }
                }
            }
        }

        /// <summary>
        /// 右クリックメニューからの終了処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BatchPrintServiceCloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeOwnFlg = true;
            if (!ForceCloseFlg)
            {
                LogUtility.OutputLog("201");
                //右クリックメニューからの終了の場合、再起動はしない
                SettingMng.DoRestartAtBatch = false;
                //終了処理実行
                if (RecieveThread != null && PrintThread != null)
                {
                    StopHttpReciever();
                }
                ForceCloseFlg = true;
            }
            else
            {
                //2回目以降は、強制終了として扱う
                try
                {
                    //アプリケーションを強制終了する 
                    LogUtility.OutputLog("202");
                    this.Close();
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    LogUtility.OutputLog("203", ex.Message);
                }
            }

        }
        /// <summary>
        /// 再起動イベント受信スレッド
        /// </summary>
        private void RestartTHEndWatch()
        {
            try
            {


                // 再起動要求シグナルがONになるまで待機する。
                BatchRestarter.MrRestartRequest.WaitOne();
                LogUtility.OutputLog("210");

                // 終了処理を呼ぶ
                StopHttpReciever();
            }
            catch (ThreadAbortException tex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = tex.Message;
                LogUtility.OutputLog("211", tex.Message);
            }
            catch (Exception ex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                LogUtility.OutputLog("211", ex.Message);
            }
            finally
            {

            }
        }
        /// <summary>
        /// 再起動処理の起点
        /// </summary>
        private void RestartBatchProntService()
        {
            //終了処理実行
            if (RecieveThread != null && PrintThread != null)
            {
                StopHttpReciever();
            }
        }
        /// <summary>
        /// タスクトレイアイコン右クリックからの終了用処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuBatchPrint_Opening(object sender, CancelEventArgs e)
        {
            if (PrintThread != null) {
                PrintThread.setNowEndDlg(true);
            }
            //フォーカスを自身に渡す
            this.Activate();
            //メッセージボックスを表示する
            DialogResult result = MessageBox.Show("biz-Stream バッチ印刷を終了してもよろしいですか？",
                "biz-Stream バッチ印刷の終了",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);
            if (PrintThread != null)
            {
                PrintThread.setNowEndDlg(false);
            }
            if (result == DialogResult.OK)
            {
                closeOwnFlg = true;
                //「OK」が選択された時
                if (!ForceCloseFlg)
                {
                    LogUtility.OutputLog("201");
                    //右クリックメニューからの終了の場合、再起動はしない
                    SettingMng.DoRestartAtBatch = false;
                    //終了処理実行
                    if (RecieveThread != null && PrintThread != null)
                    {
                        StopHttpReciever();
                    }
                    ForceCloseFlg = true;
                }
                else
                {
                    //2回目以降は、強制終了として扱う
                    try
                    {
                        //アプリケーションを強制終了する 
                        LogUtility.OutputLog("202");
                        this.Close();
                        Application.Exit();
                    }
                    catch (Exception ex)
                    {
                        LogUtility.OutputLog("203", ex.Message);
                    }
                }

            }
            else if (result == DialogResult.Cancel)
            {
                //「キャンセル」が選択された時はとくになにもしない
            }

        }

        /// <summary>
        /// メインフォームのクローズイベント 自己処理以外からの場合はイベント自体をキャンセルする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BatchPrintSeviceMain_FormClosing(object sender, FormClosingEventArgs e)
        {
#if DEBUG   
            LogUtility.OutputLog("903", "sender="+ sender.ToString()+ ",e.CloseReason=" + e.CloseReason);
#endif
            if (e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.TaskManagerClosing || e.CloseReason == CloseReason.ApplicationExitCall) {
                //これらの場合素通しで終了
            }
            else if (!closeOwnFlg)
            {
                e.Cancel = true;
            }

        }

        /// <summary>
        /// メインフォームのアクティベートイベント発生時、フォーム位置を画面外に移動する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BatchPrintSeviceMain_Activated(object sender, EventArgs e)
        {
            Rectangle r = PrintForm.GetTotalBound();
            this.Location = new Point(r.X - this.Size.Width - 1, r.Y - this.Size.Height -1);
#if DEBUG   
            LogUtility.OutputLog("904", "Batch X0,Y0="+ r.X.ToString()+ ","+ r.Y.ToString()+ " this.Location="+ this.Location.X+","+ this.Location.Y);
#endif
        }
    }
}
