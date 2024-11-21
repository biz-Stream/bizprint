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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BizPrintCommon
{
    class FormCreater
    {
        //作成関数が返すForm
        PrintForm rtnFrom = null;

        //作成成功フラグ
        bool isSuccess = false;

        //作成時例外フラグ
        bool isException = false;
        public bool ExceptionOccuerd = false;
        //例外発生でリトライが終了した際に投げるエクセプション
        Exception toThrowE = null;

        //作成時タイムアウトフラグ
        bool isTimeOut = false;
        public bool TimeOutOccuerd = false;

        //リトライ回数(全回数はこれ＋1)
        int numRetry = 5;
        //リトライループのウェイト時間
        int retryWaitmsec = 500;

        //タイムアウトチェックループのウェイト時間
        int timeoutChkWaitmsec = 100;
        //作成失敗タイムアウト時間
        int createTimeoutmsec = 2500;

        ProcKiller killer = new ProcKiller();

        public SettingManeger SettingMng { set; get; }
        /// <summary>印刷パラメータ</summary>
        private PrintParameter PrintParam { set; get; } = null;

        /// <summary>エラーコード</summary>
        public int ErrorCode { private set; get; } = ErrCodeAndmErrMsg.STATUS_OK;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="param"></param>
        /// <param name="mng"></param>
        public FormCreater(PrintParameter param, SettingManeger mng)
        {
            PrintParam = param;
            SettingMng = mng;
            createTimeoutmsec = mng.FormCreateTimeoutMsec;
            numRetry = mng.FormCreateRetryNum;
            retryWaitmsec = mng.FormCreateRetryWaitMsec;
            timeoutChkWaitmsec = mng.FormCreateTimeoutCheckMsec;

        }
        /// <summary>
        /// リトライも行ってフォームを作成する関数本体
        /// </summary>
        /// <returns></returns>
        public PrintForm CreateFormDoRetry()
        {
            int tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            try
            {

                LogUtility.OutputLog("421", tid.ToString());
                //最初の1回があるので、総ループはnumRetry + 1
                for (int i = 0; i < numRetry + 1; i++)
                {
                    if (i != 0)
                    {
                        Thread.Sleep(retryWaitmsec);
                    }
                    isSuccess = false;
                    isException = false;
                    isTimeOut = false;


                    //スレッド起動
                    Thread timeoutchkThraed = new Thread(new ThreadStart(FormTimeoutWorker));
                    timeoutchkThraed.IsBackground = true;
                    timeoutchkThraed.SetApartmentState(ApartmentState.STA);//for batch
                    timeoutchkThraed.Start();

                    //フォーム作成
                    try
                    {

                        //フォームコンストラクト
                        rtnFrom = new PrintForm(PrintParam, SettingMng);
#if DEBUG
                        //1回だけ例外
                        if (i == 0 && PrintParam.JobID.IndexOf("_0006") > 0)
                        {
                            Exception dbgEx = new Exception("デバッグ用");
                            throw dbgEx;
                        }
                        //1回だけタイムアウト
                        else if (i == 0 && PrintParam.JobID.IndexOf("_0007") > 0)
                        {
                                for (int j = 0; j < 50; j++) {
                                    Thread.Sleep(100);
                                }
                        }
                        //全て例外
                        else if (PrintParam.JobID.IndexOf("_0008") > 0)
                        {
                            Exception dbgEx = new Exception("デバッグ用");
                            throw dbgEx;
                        }
                        //全てタイムアウト
                        else if (PrintParam.JobID.IndexOf("_0009") > 0)
                        {
                            for (int j = 0; j < 50; j++)
                            {
                                Thread.Sleep(100);
                            }
                        }
#endif
                        if (!isTimeOut)
                        {
                            isSuccess = true;
                        }

                    }
                    catch (Exception ex)
                    {
                        isException = true;
                        ExceptionOccuerd = true;
                        isSuccess = false;
                        toThrowE = ex;

                    }
#if DEBUG
                    //log432用
                    if (PrintParam.JobID.IndexOf("_0010") > 0)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            Thread.Sleep(100);
                        }
                    }
#endif
                    //この時点で監視スレッドは殺していい
                    if (timeoutchkThraed.IsAlive)
                    {
                        timeoutchkThraed.Abort();
                    }


                    //成功なら返す
                    if (isSuccess)
                    {
                        LogUtility.OutputLog("422", tid.ToString());
                        return rtnFrom;
                    }
                    //例外ならリトライ
                    else if (isException)
                    {
                        if (rtnFrom != null) {
                            rtnFrom.Dispose();
                        }
                        LogUtility.OutputLog("423", toThrowE.Message, tid.ToString());
                    }
                    //タイムアウトならリトライ
                    else if (isTimeOut)
                    {
                        if (rtnFrom != null)
                        {
                            rtnFrom.Dispose();
                        }
                        LogUtility.OutputLog("424", tid.ToString());
                        TimeOutOccuerd = true;
                    }

                    if (i < numRetry)
                    {
                        LogUtility.OutputLog("425", retryWaitmsec.ToString(),(numRetry - i).ToString() );
                    }
                }
                //リトライ限界まで失敗した場合
                if (!isSuccess)
                {
                    LogUtility.OutputLog("426", tid.ToString());
                    ErrorCode = ErrCodeAndmErrMsg.ERR_CODE_0201;
                }
            }
            catch (Exception ex)
            {
                LogUtility.OutputLog("423", ex.Message, tid.ToString());
                toThrowE = ex;
                ExceptionOccuerd = true;
                ErrorCode = ErrCodeAndmErrMsg.ERR_CODE_0201;
            }
            if (ExceptionOccuerd)
            {
                throw toThrowE;
            }
            return null;
        }

        /// <summary>
        /// フォーム作成タイムアウトを監視するワーカスレッド
        /// </summary>
        private void FormTimeoutWorker()
        {                
            //開始時刻取得
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            DateTime starttime = DateTime.Now;
            LogUtility.OutputLog("431", id.ToString());
            try
            {
                while (true)
                {
                    //既に作成成功してれば自身を終了
                    if (this.isSuccess)
                    {
                        LogUtility.OutputLog("432", id.ToString());
                        break;
                    }

                    //タイムアウト時間を超えてるならばタイムアウトにしてAcrobatReader殺す
                    TimeSpan timeDiffer = DateTime.Now - starttime;
                    if (timeDiffer.TotalMilliseconds > createTimeoutmsec)
                    {
                        LogUtility.OutputLog("433", timeDiffer.TotalMilliseconds.ToString(),id.ToString());
                        //タイムアウトフラグ立てる
                        isTimeOut = true;
                        //Acrobat Reader殺す
                        killer.killAcrobatProocesses(SettingMng.KillProcNames);
                        break;
                    }
                    Thread.Sleep(timeoutChkWaitmsec);
                }


            }
            catch (Exception ex)
            {
                //スレッド強制終了時。なにもしない。
                LogUtility.OutputLog("434");
            }

        }

    }
}
