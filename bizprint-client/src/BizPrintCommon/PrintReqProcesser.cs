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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace BizPrintCommon
{
    /// <summary>
    /// 印刷要求を出力する、印刷メイン処理スレッド
    /// </summary>
    public class PrintReqProcesser
    {

        ///<summary>サーバ種別</summary>
        private int ServerType;//Direct or Batch
        ///<summary>印刷サービススレッドメイン</summary>
        private Thread PrintThreadMain = null;
        /// <summary>印刷終了要求イベント</summary>
        public static ManualResetEvent MrePrintEndRequest { set; get; } = new ManualResetEvent(false);
        /// <summary>終了結果</summary>
        public static ManualResetEvent MrePrintEndResult { set; get; } = new ManualResetEvent(false);
        /// <summary>スレッド起動フラグ</summary>
        public bool IsPrintProcessThreadStarted { set; get; } = false;
        /// <summary>設定マネージャ</summary>
        public SettingManeger SettingMng { set; get; }

        /// <summary>フォーム起動中フラグ</summary>
        private bool IsPrintFormExec = false;

        /// <summary>前回印刷時に使用したプリンタ名</summary>
        private string LastPrinterName = "";
        private bool LastPrinterNameSetted = false;

        /// <summary>キューから取り出された印刷データの数</summary>
        private int queOutCount = 0;

        /// <summary>トータル印刷数</summary>
        private int totalPrintCount = 0;

        /// <summary>CPU使用パーセントに応じての処理一時待ちループ回数</summary>
        private int busyCount = 0;

        private ProcKiller killer = new ProcKiller();

        ///<summary>終了ダイアログ表示中フラグ</summary>
        public bool isNowEndDlg = false;
        ///<summary>終了ダイアログ表示中フラグセット関数</summary>
        public void setNowEndDlg(bool now)
        {
            int cnt = 0;
            int maxWait = SettingMng.LoadRetryWaitMsec * SettingMng.LoadRetryNum + (SettingMng.FormCreateTimeoutMsec + SettingMng.FormCreateRetryWaitMsec) * SettingMng.FormCreateRetryNum;
            int nowWait = 0;
            while (isDoPrinting && nowWait < maxWait)
            {
#if DEBUG
                LogUtility.OutputLog("999-setNowEndDlg Wait count="+ cnt.ToString().ToLower());
#endif
                Thread.Sleep(SettingMng.PrintProcessThreadWaitMsec / 2);
                cnt++;
                nowWait = cnt * (SettingMng.PrintProcessThreadWaitMsec / 2);

            }
            isNowEndDlg = now;
            LogUtility.OutputLog("218", now.ToString().ToLower());
        }

        ///<summary>現在印刷中フラグ</summary>
        private bool isDoPrinting = false;
        ///<summary>現在印刷中フラグ設定関数</summary>
        public void setIsDoPrinting(bool now)
        {
            isDoPrinting = now;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="type"></param>
        public PrintReqProcesser(int type, SettingManeger set)
        {
            ServerType = type;
            SettingMng = set;

            //開始時のデフォルトプリンタ―の取得はしない
 
        }

        /// <summary>
        /// スレッド起動処理
        /// </summary>
        /// <returns></returns>
        public bool StartPrintProcessThread()
        {
            //排他制御にタイムアウトして失敗
            if (!Monitor.TryEnter(this, CommonConstants.LOCK_TIMEOUT))
            {
                if (Monitor.IsEntered(this))
                {
                    Monitor.Exit(this);
                }
                LogUtility.OutputLog("125");
                return false;
            }
            //既存スレッドが存在する
            if (PrintThreadMain != null)
            {
                if (Monitor.IsEntered(this))
                {
                    Monitor.Exit(this);
                }
                LogUtility.OutputLog("126");
                return false;
            }
            LogUtility.OutputLog("056");
            IsPrintProcessThreadStarted = true;
            PrintThreadMain = new Thread(new ThreadStart(PrintProcessThread));
            PrintThreadMain.IsBackground = true;
            PrintThreadMain.SetApartmentState(ApartmentState.STA);//for batch
            PrintThreadMain.Start();

            if (Monitor.IsEntered(this))
            {
                Monitor.Exit(this);
            }

            return true;

        }

        /// <summary>
        /// 印刷キュー監視を行い、個々の印刷処理を呼ぶスレッドのメインループ
        /// </summary>
        private void PrintProcessThread()
        {

            DateTime restartCheckTime = DateTime.Now;
            try
            {
                while (true)
                {
                    // 終了依頼がセットされている場合
                    if (MrePrintEndRequest.WaitOne(10, false) == true)
                    {
                        // 現在実行中の印刷スレッドがない場合
                        if (true)
                        {
                            // 終了依頼処理を受け付けたことをセットする。
                            MrePrintEndResult.Set();
                            break;
                        }
                    }
                    else
                    {
                        while (IsPrintProcessThreadStarted)
                        {
                            //最終キューチェック時間をUpdate
                            SettingManeger.UpdateLastQueCheck();
                            //終了ダイアログ表示中ではないか、キューに要求があるか、既に印刷中ではないかチェック
                            //chkIsCPUBusyは0.1sec毎に直近5回のCPU使用率を計測する
                            if (!isNowEndDlg && PrintReqQueue.IsReqQueHaveData() && !IsPrintFormExec && !chkIsCPUBusy())
                            {
                                setIsDoPrinting( true );
                                if (!LastPrinterNameSetted)
                                {
                                    //デフォルトプリンタ―の取得
                                    DefaultPrinterGetter dpg = new DefaultPrinterGetter();
                                    if (dpg.getDefaultPrinter()) {
                                        LastPrinterName = dpg.PrinterName;
                                        LastPrinterNameSetted = true;
                                    }
                                }
                                string nextPrinterName = PrintReqQueue.getNextPrinter();
                                //前回とプリンターが変わったのかチェック。変わってるならばプリンタ書き換えが起こるので、現在の子プロセスは一度殺す
                                //KillProcPrintCountが負の場合、同じプリンタで印刷してる間は殺さない。正の場合、同じプリンタでの印刷回数がKillProcPrintCountを超えたら殺す
                                if (!LastPrinterName.Equals(nextPrinterName) || (SettingMng.KillProcPrintCount >= 0 && ( queOutCount >= SettingMng.KillProcPrintCount)))
                                {
                                    //子プロセス探して殺す
                                    killer.killAcrobatProocesses(SettingMng.KillProcNames);
                                    queOutCount = 1;
                                }
                                else {
                                    queOutCount++;
                                }
                                SettingMng.DoRestartNow = false;
                                LastPrinterName = nextPrinterName;

                                //設定に応じてフラグ設定。AUTOの場合は判定。
                                bool doThread = false;
                                //bool isAcrobat32 = AcrobatRegistryUtil.IsUsingAcrobat32bit();

                                if (SettingMng.PrintFormByThread == SettingManeger.ThreadMode_TRUE)
                                {
                                    doThread = true;
                                }
                                else if (SettingMng.PrintFormByThread == SettingManeger.ThreadMode_FALSE)
                                {
                                    doThread = false;
                                }
                                else if (SettingMng.PrintFormByThread == SettingManeger.ThreadMode_AUTO)
                                {
                                    //AUTOの場合、Acrobat32bitならばスレッド起動→AUTOの場合常にスレッド起動
                                    //if (isAcrobat32)
                                    //{
                                    //    doThread = true;
                                    //}
                                    //else
                                    //{
                                    //    doThread = false;
                                    //}
                                    doThread = true;
                                }


                                LogUtility.OutputLog("214", "printformbythread=" + doThread.ToString().ToLower());
                                //スレッド実行か関数起動かで分岐
                                if (doThread)
                                {
                                    Thread cpft = new Thread(new ThreadStart(CallPrintForm));
                                    cpft.SetApartmentState(ApartmentState.STA);
                                    cpft.IsBackground = false;
                                    cpft.Start();
                                    cpft.Join();
                                    GC.Collect(); ;
                                }
                                else
                                {
                                    CallPrintForm();
                                }
                                setIsDoPrinting( false );
                                totalPrintCount++;
                            }
                            else if (!PrintReqQueue.IsReqQueHaveData())
                            {
                                //バッチ印刷で、再起動する、印刷カウント超えたかをチェックする
                                if (SettingMng.ServiceType == CommonConstants.MODE_BATCH)
                                {
                                    if (SettingMng.DoRestartAtBatch && totalPrintCount > SettingMng.DoRestartPrintNum && !SettingMng.DoRestartNow)
                                    {
                                        LogUtility.OutputDebugLog("207", totalPrintCount.ToString(), SettingMng.DoRestartWaitTimeMin.ToString());
                                        //再起動待ち開始
                                        SettingMng.DoRestartNow = true;
                                        restartCheckTime = DateTime.Now;

                                    }
                                    //再起動待ち時間経過
                                    if (SettingMng.DoRestartNow)
                                    {
                                        TimeSpan timeDiffer = DateTime.Now - restartCheckTime;
                                        if (SettingMng.DoRestartNow)
                                        {
                                            if (timeDiffer.TotalMinutes > SettingMng.DoRestartWaitTimeMin)
                                            {
                                                LogUtility.OutputDebugLog("208", restartCheckTime.ToString(), timeDiffer.TotalMinutes.ToString());
                                                //再起動要求送信
                                                BatchRestarter.MrRestartRequest.Set();
                                                SettingMng.DoRestartNow = false;
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    //ダイレクト印刷で終了時間チェック超えたら終わる
                                    TimeSpan timeDiffer = DateTime.Now - SettingManeger.GetLatestEventTime();
                                    double diff = timeDiffer.TotalSeconds;
                                    if (SettingMng.ExitByTimerEnabled && SettingMng.ExitTimerLimit < diff)
                                    {
                                        HttpReciever.IsThreadStarted = false;
                                        HttpReciever.MreDiscontinuanceRequest.Set();
                                        HttpReciever.MreAllDone.Set();
                                    }
                                }
                            }
                            //今要求は無いかビジーなので少し待つor印刷中なので待つ
                            Thread.Sleep(SettingMng.PrintProcessThreadWaitMsec);
                            break;
                        }
                    }
                }
            }
            catch (ThreadAbortException th)
            {
                LogUtility.OutputDebugLog("E033", th.Message);
            }
            catch (Exception ex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                LogUtility.OutputDebugLog("E034", ex.Message);
            }
            finally
            {
                LogUtility.OutputLog("101");
            }
        }

        /// <summary>
        /// 印刷処理用のフォームを呼び、印刷処理を実行する
        /// </summary>
        public void CallPrintForm()
        {
            SettingManeger.UpdateLatestEvent();
            //レジストリのチェックと書き込み
            AcrobatRegistryUtil.SetAcrobatCheckRegistory();

            IsPrintFormExec = true;
            int frmRtn = 0;

            PrintParameter printParam = (PrintParameter)PrintReqQueue.GetNextReqest();
            if (printParam == null)
            {
                //リクエストがなかった(これはありえない)
                LogUtility.OutputDebugLog("E036");
                return;
            }
            LogUtility.OutputLog("058", printParam.JobID);
            //「印刷中」にアップデート
            PrintHistoryManager.UpdatePrintStatusByID(printParam.JobID, CommonConstants.JOB_STATUS_PRINTING);
            LogUtility.OutputLog("100", printParam.JobID, CommonConstants.JOB_STATUS_PRINTING.ToString());

            int totalState = ErrCodeAndmErrMsg.STATUS_OK;
            FormCreater pfc = new FormCreater(printParam, SettingMng);
            try
            {

                //印刷実行 ダイアログ表示アリ(ダイレクトのみ)
                if (printParam.IsPrintDialog && SettingMng.ServiceType == CommonConstants.MODE_DIRECT)
                {
                    //ダイアログを使用しての印刷中フラグの更新
                    LogUtility.OutputLog("453");
                    SettingManeger.IsPrintingWithDialog = true;

                    LogUtility.OutputLog("092");
                    //印刷ダイアログの作成
                    PrintForm printFrm = pfc.CreateFormDoRetry();
                    printFrm.setParentTH(this);
#if DEBUG
                    if (printParam.JobID.IndexOf("_0007") > 0) {
                        throw new Exception("Debug on _0007");
                    }
#endif
                    printFrm.StartTimer();
                    int chk;
                    //ファイルセーブの実行
                    if (printParam.SaveFileName.Length > 0)
                    {
                        frmRtn = printFrm.SavePdfFile();
                    }
                    if (frmRtn != ErrCodeAndmErrMsg.STATUS_OK)
                    {
                        //ファイルセーブに失敗。
                        PrintHistoryManager.UpdatePrintStatusByID(printParam.JobID, CommonConstants.JOB_STATUS_ERROR_FINISH);
                        //doResponce
                        chk = WebBrowserUtil.OpenResponceWithID(printParam.BrowserProcessname, printParam.ResponseURL, printParam.TargetFrame, frmRtn, printParam.JobID);
                        //ダイアログを使用しての印刷中フラグの更新
                        LogUtility.OutputLog("454");
                        SettingManeger.IsPrintingWithDialog = false;
                        return;
                    }
                    printFrm.ShowDialog();
                    frmRtn = printFrm.PrintStatusReturn;
                    if (frmRtn != ErrCodeAndmErrMsg.STATUS_OK)
                    {
                        //印刷処理に失敗
                        PrintHistoryManager.UpdatePrintStatusByID(printParam.JobID, CommonConstants.JOB_STATUS_ERROR_FINISH);
                    }
                    //doResponce
                    chk = WebBrowserUtil.OpenResponceWithID(printParam.BrowserProcessname, printParam.ResponseURL, printParam.TargetFrame, frmRtn, printParam.JobID);
                }
                else//ダイアログ表示なし
                {
                    LogUtility.OutputLog("094", printParam.NumberOfCopy.ToString());
                    //印刷ダイアログの作成
                    PrintForm printFrm = pfc.CreateFormDoRetry();
                    printFrm.setParentTH(this);

#if DEBUG
                    if (printParam.JobID.IndexOf("_0007") > 0)
                    {
                      throw new Exception("Debug on _0007");
                    }
#endif
                    printFrm.StartTimer();
                    //ファイルセーブの実行
                    if (printParam.SaveFileName.Length > 0)
                    {
                        frmRtn = printFrm.SavePdfFile();
                    }
                    if (totalState == ErrCodeAndmErrMsg.STATUS_OK && frmRtn != ErrCodeAndmErrMsg.STATUS_OK)
                    {
                        totalState = frmRtn;
                        //ファイルセーブに失敗。
                        PrintHistoryManager.UpdatePrintStatusByID(printParam.JobID, CommonConstants.JOB_STATUS_ERROR_FINISH);
                    }
                    if (totalState == ErrCodeAndmErrMsg.STATUS_OK)
                    {
                        printFrm.ShowDialog();
                        frmRtn = printFrm.PrintStatusReturn;
                        if (frmRtn != ErrCodeAndmErrMsg.STATUS_OK)
                        {
                            totalState = frmRtn;
                            //印刷処理に失敗
                            PrintHistoryManager.UpdatePrintStatusByID(printParam.JobID, CommonConstants.JOB_STATUS_ERROR_FINISH);
                        }
                        else
                        {
                            //097は成功時だけ出す
                            LogUtility.OutputLog("097", printParam.JobID, frmRtn.ToString());
                            PrintHistoryManager.UpdatePrintStatusByID(printParam.JobID, CommonConstants.JOB_STATUS_SUCCESS_FINISH);
                        }
                    }
                    int chk = WebBrowserUtil.OpenResponceWithID(printParam.BrowserProcessname, printParam.ResponseURL, printParam.TargetFrame, frmRtn, printParam.JobID);
                }
            }
            catch (Exception ex)
            {
                if (pfc.ErrorCode == ErrCodeAndmErrMsg.ERR_CODE_0201)
                {
                    //フォーム作成で失敗した場合のエラー
#if DEBUG
                    LogUtility.OutputLog("DEBUG201 ERR_CODE_0201 "+ ex.Message);
#endif
                    totalState = ErrCodeAndmErrMsg.ERR_CODE_0201;
                    ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                    WebBrowserUtil.OpenResponceWithID(printParam.BrowserProcessname, printParam.ResponseURL, printParam.TargetFrame, ErrCodeAndmErrMsg.ERR_CODE_0201, printParam.JobID);
                }
                else
                {
                    //通常の処理で検出できないエラー。
                    LogUtility.OutputLog("501", "CallPrintForm", ex.Message);
                    totalState = ErrCodeAndmErrMsg.ERR_CODE_0501;
                    ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                    int chk = WebBrowserUtil.OpenResponceWithID(printParam.BrowserProcessname, printParam.ResponseURL, printParam.TargetFrame, ErrCodeAndmErrMsg.ERR_CODE_0501, printParam.JobID);
                }


            }

            if (totalState == ErrCodeAndmErrMsg.STATUS_OK)
            {
                PrintHistoryManager.UpdatePrintStatusAndErrCodeByID(printParam.JobID, CommonConstants.JOB_STATUS_SUCCESS_FINISH, ErrCodeAndmErrMsg.STATUS_OK);
            }
            else
            {
                PrintHistoryManager.UpdatePrintStatusAndErrCodeByID(printParam.JobID, CommonConstants.JOB_STATUS_ERROR_FINISH, totalState);
            }


            SettingManeger.UpdateLatestEvent();
            //ダイアログを使用しての印刷中フラグの更新
            if (printParam.IsPrintDialog && SettingMng.ServiceType == CommonConstants.MODE_DIRECT)
            {
                LogUtility.OutputLog("454");
                SettingManeger.IsPrintingWithDialog = false;
            }

            IsPrintFormExec = false;
        }

        /// <summary>
        /// 現在印刷処理中かのチェック。
        /// </summary>
        /// <returns>true:印刷中、false：印刷していない</returns>
        public bool isPrinting()
        {
            bool rtn = false;
            //キューに残要求がある OR 印刷用フォームが存在してる
            //いずれかが満たされていればtrue
            if (PrintReqQueue.IsReqQueHaveData() || IsPrintFormExec)
            {
                rtn = true;
            }
            return rtn;
        }

        /// <summary>
        /// 直近0.5秒間のCPU使用率が設定値を超えている場合にbusyと判断する
        /// </summary>
        /// <returns>true:busy,false:safe</returns>
        public bool chkIsCPUBusy()
        {
#if DEBUG
            LogUtility.OutputLog("999-01 chkIsCPUBusy called SettingMng.BusyWaitLoopNumMax="+SettingMng.BusyWaitLoopNumMax);
#endif
            if (SettingMng.BusyWaitLoopNumMax == 0 || busyCount >= SettingMng.BusyWaitLoopNumMax)
            {
#if DEBUG
            LogUtility.OutputLog("999-02 chkIsCPUBusy return false");
#endif
                busyCount = 0;
                return false;
            }
            try
            {
                ArrayList listCPU = new ArrayList();
                var pcs = new PerformanceCounter[Environment.ProcessorCount];
                //Console.Write("pcs.Length=" + pcs.Length.ToString());

                for (var index = 0; index < pcs.Length; index++)
                {
                    // プロセッサ毎の使用率を計測するPerformanceCounterを作成
                    pcs[index] = new PerformanceCounter("Processor", "% Processor Time", index.ToString());
                }
                for (int i = 0; i < 5; i++)
                {
                    foreach (var pc in pcs)
                    {
                        listCPU.Add(pc.NextValue());
                    }
                    Thread.Sleep(100);
                }
                //全てを一元配列に格納して平均を取得
                float[] allList = new float[listCPU.Count];
                listCPU.CopyTo(allList);
                float avg = allList.Average();

                //Console.Write("avg="+ avg.ToString());
#if DEBUG
                LogUtility.OutputLog("999-03 chkIsCPUBusy avg=" + avg.ToString() + "%"+ " SettingMng.CPU_Percentage="+ SettingMng.CPU_Percentage);
#endif
                if (avg > SettingMng.CPU_Percentage)
                {
                    // LogUtility.OutputLog("501", "wait now turn", avg.ToString() + "%");
#if DEBUG
            LogUtility.OutputLog("999-04 chkIsCPUBusy wait now turn avg="+avg.ToString() + "%");
#endif
                    GC.Collect();
                    busyCount++;

                    return true;
                }
                else
                {
#if DEBUG
            LogUtility.OutputLog("999-05 chkIsCPUBusy not wait avg="+avg.ToString() + "%");
#endif
                    busyCount = 0;
                    return false;

                }
            }
            catch (Exception)
            {
            }
#if DEBUG
            //例外が発生しない限りここにはこない
            LogUtility.OutputLog("999-06 chkIsCPUBusy return false");
#endif

            busyCount = 0;
            return false;
        }

    }
}
