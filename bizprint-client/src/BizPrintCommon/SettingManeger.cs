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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizPrintCommon
{
    ///<summary>
    /// 設定ファイル読み込み・設定値管理クラス
    /// </summary>
    public class SettingManeger
    {
        ///<summary>サーバ種別</summary>
        public int ServiceType { private set; get; }//Direct or Batch
        ///<summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="serviceType"></param>
        public SettingManeger(int serviceType)
        {
            this.ServiceType = serviceType;
            LatestEventTime = DateTime.Now;
        }

        ///<summary> 最終イベント発生時刻(Directでの終了判定に使用)</summary>
        private static DateTime LatestEventTime { set; get; }
        ///<summary>最終イベント時間更新ロック用</summary>
        private static Object TimeLockObj = new Object();
        ///<summary>最終イベント時間参照用</summary>
        public static DateTime GetLatestEventTime() { return LatestEventTime; }

        ///<summary> 最終キューチェック時刻(キュー監視スレッドの生存判定に使用)</summary>
        private static DateTime LastQueCheckTime { set; get; }
        ///<summary>最終イベント時間更新ロック用</summary>
        private static Object QueCheckLockObj = new Object();
        ///<summary>最終イベント時間参照用</summary>
        public static DateTime GetLastQueCheckTime() { return LastQueCheckTime; }
        ///<summary>無応答判定時間(キューチェックタイムアウト演算値)</summary>
        public static double NoQueCheckTimeoutMsec { set; get; } = 35000;

        ///<summary>印刷ダイアログを出力する印刷実行中フラグ</summary>
        public static bool IsPrintingWithDialog { set; get; } = false;

        ///<summary>印刷タイムアウト(msec)</summary>
       //--// public int PrintTimeOut { get; private set; }
        ///<summary>スプーラ監視タイムアウト(msec)</summary>
        public int SpoolCheckTimeOut { get; private set; }
        ///<summary>sppファイル解凍パスワード(base64エンコーディング済み)</summary>
        public string SppPassword { get; private set; }
        ///<summary>印刷履歴保持最大数</summary>
        public int MaxHistoryNum { get; private set; }
        ///<summary>ポート番号</summary>
        public int PortNo { get; private set; }
        ///<summary>印刷キュー保持数</summary>
        public int QueueMaxSize { get; private set; }
        ///<summary>印刷履歴の最大保持時間(秒)</summary>
        public int HistoryHoldTime { get; private set; }
        ///<summary>時間経過によるキュー待ちスレッド終了</summary>
        public bool ExitByTimerEnabled { get; private set; } = false;
        ///<summary>キュー待ちスレッドタイムアウト(秒)</summary>
        public int ExitTimerLimit { get; private set; } = 300;//0だと最初の1回で死ぬ

        ///レスポンスひな形htmlファイル名
        public string ResponseTemplateHtmlFile { get; private set; }
        ///<summary>デフォルトブラウザ識別子</summary>
        public string DefaultBrowserType { get; private set; }
        ///<summary>一時ファイル作成フォルダパス</summary>
        public string TmpFolderPath { get; private set; }
        ///<summary>開始・終了時に一時フォルダクリーンアップを行うか、印刷終了時に一時ファイルを削除するか</summary>
        public static bool CleanupTmpFolder { get; private set; }
        ///<summary>印刷ダイアログ表示を行う場合に監視する対象のダイアログ名</summary>
        public string AcrobatPrintDialogName { get; private set; }
        ///<summary>印刷ダイアログを発見出来ない時のタイムアウト(msec)</summary>
        public int PrintDialogFindTimeOut { get; private set; }
        ///<summary>一定時間以上印刷ダイアログが表示されたままの時のログ出力までの時間(msec)</summary>
        public int PrintDialogStayingTimeCheck { get; private set; }
        ///<summary>印刷ダイアログが閉じてからフォームが終了するまでのタイムアウト(秒)</summary>
        public int PrintDialogLeaveTimeOut { get; private set; }

        ///<summary>起動元として指定可能なブラウザのプロセス名と返信先のセット</summary>
        public String[] ProcNames { get; private set; }
        public String[] BrowserNames { get; private set; }

        ///<summary>印刷キューから取り出すのを止めるパーセント閾値</summary>
        public double CPU_Percentage { get; private set; }

        ///<summary>印刷キューから取り出すのを止めるループ回数最大値</summary>
        public double BusyWaitLoopNumMax { get; private set; }

        ///<summary>プログラムの再起動フラグ</summary>
        public bool DoRestartAtBatch { get; set; } = true;
        ///<summary>プログラムの再起動フラグがtrueの場合に、何回印刷したら再起動するか</summary>
        public int DoRestartPrintNum { get; set; }
        ///<summary>プログラムの再起動フラグがtrueの場合に、キューが空になってから何分後に再起動するか</summary>
        public int DoRestartWaitTimeMin { get; set; }
        ///<summary>プログラムの再起動フラグ</summary>
        public bool DoRestartNow { get; set; } = false;
        ///<summary>印刷フォームのタイマインターバル(msec)</summary>
        public int PrintFormTimerInterval { get; set; } = 100;
        ///<summary>印刷スレッドのループ待機時間(msec)</summary>
        public int PrintProcessThreadWaitMsec { get; set; } = 100;
        ///<summary>ロード失敗時のリトライ回数</summary>
        public int LoadRetryNum { get; set; }
        ///<summary>ロード失敗時の待機時間(msec)</summary>
        public int LoadRetryWaitMsec { get; set; }

        ///<summary>フォーム作成処理タイムアウト検知時間(msec)</summary>
        public int FormCreateTimeoutMsec { get; set; }
        ///<summary>フォーム作成処理タイムアウトチェックループ時間(msec)</summary>
        public int FormCreateTimeoutCheckMsec { get; set; }
        ///<summary>フォーム作成処理最大リトライ回数</summary>
        public int FormCreateRetryNum { get; set; }
        ///<summary>フォーム作成処理リトライ待機時間(msec)</summary>
        public int FormCreateRetryWaitMsec { get; set; }

        //kill対象となるプロセス名
        public String KillProcNames { get; private set; }

        //killを行うまでの同プリンタでの印刷回数
        public int KillProcPrintCount { get; private set; }

        /// <summary> フォームのスレッド実行モード  </summary>
        public int PrintFormByThread { get; set; } = ThreadMode_TRUE;
        public static int ThreadMode_TRUE = 0;
        public static int ThreadMode_FALSE = 1;
        public static int ThreadMode_AUTO = 2;
        public static string[] TH_MODE ={ "true","false","auto"};
        public int chkThreadMode(string mode)
        {
            if (mode.Equals(TH_MODE[ThreadMode_TRUE], StringComparison.OrdinalIgnoreCase))
            {
                return ThreadMode_TRUE;
            }
            else if (mode.Equals(TH_MODE[ThreadMode_FALSE], StringComparison.OrdinalIgnoreCase))
            {
                return ThreadMode_FALSE;
            }
            else if (mode.Equals(TH_MODE[ThreadMode_AUTO], StringComparison.OrdinalIgnoreCase))
            {
                return ThreadMode_AUTO;
            }
            return ThreadMode_AUTO;
        }

        //デフォルトプリンタ取得タイムアウト(msec)
        public static int DefaultPrinterTimeout { get; private set; } = ServicetConstants.DEFAULT_PRINTER_GET_TIMEOUT;
        //デフォルトプリンタ取得チェックループインターバル(msec)
        public static int DefaultPrinterCheckInterval { get; private set; } = 33;

        ///<summary>
        /// 設定ファイルのロード
        /// </summary>
        /// <returns></returns>
        public bool LoadSetting()
        {
            string configPath;
            if (ServiceType == CommonConstants.MODE_DIRECT)
            {
                configPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + ServicetConstants.DirectConfFolderName + "\\" + ServicetConstants.DPconfFileName;
#if DEBUG
                //configPath = @"C:\ProgramData\brainsellers\DirectPrint\" + ServicetConstants.DPconfFileName;
#endif
            }
            else
            {
                configPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + ServicetConstants.BatchConfFolderName + "\\" + ServicetConstants.BatchconfFileName;
#if DEBUG
                //configPath = @"C:\ProgramData\brainsellers\BatchPrint\" + ServicetConstants.BatchconfFileName;
#endif
            }
            LogUtility.OutputLog("009", configPath);
            XMLLoader xmlLoader = new XMLLoader();
            try
            {
                xmlLoader.LoadFromXMLFile(configPath);
            }
            catch (Exception ex)
            {
                //(ログID：DI011)
                LogUtility.OutputLog("011", configPath, ex.Message);
                return false;
            }
            //設定読み込み


            //ブラウザおよび印刷ダイアログ関連設定はダイレクトのみ
            string readBrowserset = "";
            if (ServiceType == CommonConstants.MODE_DIRECT)
            {
                readBrowserset = xmlLoader.ReadEntry(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_BROWSER_SET_KEY, ServicetConstants.DEFAULT_BROWSER_SET);
                setBrowserNames(xmlLoader.ReadEntry(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_BROWSER_SET_KEY, ServicetConstants.DEFAULT_BROWSER_SET));
                CleanupTmpFolder = xmlLoader.ReadEntryBool(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_CLEANUP_TMP_KEY, ServicetConstants.DEFAULT_CLEANUP_TMP);
                BusyWaitLoopNumMax = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_CPU_CHK_LOOPMAX, ServicetConstants.DEFAULT_CPU_CHK_LOOPMAX);
                CPU_Percentage = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_CPU_CHK_PERCENTAGE, ServicetConstants.DEFAULT_CPU_CHK_PERCENTAGE);
                DefaultBrowserType = xmlLoader.ReadEntry(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_DEFAULT_BROWSER_KEY, ServicetConstants.DEFAULT_BROWSER);
                DefaultPrinterTimeout = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_PRINTER_GET_TIMEOUT, ServicetConstants.DEFAULT_PRINTER_GET_TIMEOUT);
                if (DefaultPrinterTimeout < ServicetConstants.DEFAULT_PRINTER_GET_MIN)
                {
                    DefaultPrinterTimeout = ServicetConstants.DEFAULT_PRINTER_GET_TIMEOUT;
                }
                DefaultPrinterCheckInterval = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_PRINTER_GETCHK_INTERVAL, ServicetConstants.DEFAULT_PRINTER_GETCHK_INTERVAL);
                if (DefaultPrinterCheckInterval >= DefaultPrinterTimeout) {
                    DefaultPrinterCheckInterval = ServicetConstants.DEFAULT_PRINTER_GETCHK_INTERVAL;
                }
                ExitByTimerEnabled = xmlLoader.ReadEntryBool(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_EXIT_TIME_ENABLE_KEY, ServicetConstants.DEFAULT_EXIT_TIME_ENABLE);
                ExitTimerLimit = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_EXIT_TIME_LIMIT_KEY, ServicetConstants.DEFAULT_EXIT_TIME_LIMIT);
                FormCreateRetryNum = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_FORM_RETRY_NUM, ServicetConstants.DEFAULT_FORM_RETRY_NUM);
                FormCreateRetryWaitMsec = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_FORM_RETRYWAIT_MSEC, ServicetConstants.DEFAULT_FORM_RETRYWAIT_MSEC);
                FormCreateTimeoutCheckMsec = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_FORM_TIMEOUT_ROOP_MSEC, ServicetConstants.DEFAULT_FORM_TIMEOUT_ROOP_MSEC);
                FormCreateTimeoutMsec = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_FORM_TIMEOUT_CHECK_MSEC, ServicetConstants.DEFAULT_FORM_TIMEOUT_CHECK_MSEC);
                MaxHistoryNum = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_HISTMAX_KEY, ServicetConstants.DEFAULT_HISTMAX);
                HistoryHoldTime = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_HOLD_TIME_KEY, ServicetConstants.DEFAULT_HOLDTIME);
                KillProcNames = xmlLoader.ReadEntry(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_KILL_PROC_NAMES, ServicetConstants.DEFAULT_KILL_PROC_NAMES);
                KillProcPrintCount = xmlLoader.ReadEntryIntCanMinus(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_KILL_PROCS_COUNT, ServicetConstants.DEFAULT_KILL_PROCS_COUNT);
                LoadRetryNum = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_LOAD_RETRY_NUM, ServicetConstants.DEFAULT_LOAD_RETRY_NUM);
                LoadRetryWaitMsec = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_LOAD_RETRYWAIT_MSEC, ServicetConstants.DEFAULT_LOAD_RETRYWAIT_MSEC);
                PortNo = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_PORT_KEY, ServicetConstants.DEFAULT_PORT);
                PrintDialogFindTimeOut = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_PRINTDLG_FIND, ServicetConstants.DEFAULT_PRINTDLG_FIND);
                PrintDialogLeaveTimeOut = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_PRINTDLG_LEAVE, ServicetConstants.DEFAULT_PRINTDLG_LEAVE);
                if (PrintDialogLeaveTimeOut < ServicetConstants.DEFAULT_PRINTDLG_LEAVE_MIN)
                {
                    PrintDialogLeaveTimeOut = ServicetConstants.DEFAULT_PRINTDLG_LEAVE;
                }
                AcrobatPrintDialogName = xmlLoader.ReadEntry(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_PRINT_DLG_NAME_KEY, ServicetConstants.DEFAULT_PRINT_DLG_NAME);
                PrintDialogStayingTimeCheck = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_PRINTDLG_STAYING, ServicetConstants.DEFAULT_PRINTDLG_STAYING);
                string tmp = xmlLoader.ReadEntry(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_PRINT_BY_THREAD, ServicetConstants.DEFAULT_PRINT_BY_THREAD);
                PrintFormByThread = chkThreadMode(tmp);
                PrintFormTimerInterval = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_PRINTFORM_TIMER_INTREVAL, ServicetConstants.DEFAULT_PRINTFORM_TIMER_INTREVAL);
                if (PrintFormTimerInterval < 1) {
                    PrintFormTimerInterval = ServicetConstants.DEFAULT_PRINTFORM_TIMER_INTREVAL;
                }
                PrintProcessThreadWaitMsec = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_PRINTTHREAD_WAIT_MSEC, ServicetConstants.DEFAULT_PRINTTHREAD_WAIT_MSEC);
                QueueMaxSize = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_QUEUESIZE_KEY, ServicetConstants.DEFAULT_QUEUESIZE);
                ResponseTemplateHtmlFile = xmlLoader.ReadEntry(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_RESPONCE_TEMPLATE_KEY, ServicetConstants.DEFAULT_RESPONCE_TEMPLATE);
                if (ResponseTemplateHtmlFile.Length == 0)
                {
                    ResponseTemplateHtmlFile = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\brainsellers\\DirectPrint\\html\\response.html";
                }
                SpoolCheckTimeOut = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_SPOOLTIMEOUT_KEY, ServicetConstants.DEFAULT_SPOOLTIMEOUT);
                SppPassword = xmlLoader.ReadEntry(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_SPP_PATH_KEY, ServicetConstants.DEFAULT_SPP_PATH);
                TmpFolderPath = xmlLoader.ReadEntry(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_TMP_FOLDER_PATH_KEY, ServicetConstants.DEFAULT_TMP_FOLDER);
                if (TmpFolderPath.Length == 0)
                {
                    TmpFolderPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\brainsellers\\DirectPrint\\tmp";
                }


            }
            else
            {
                CleanupTmpFolder = xmlLoader.ReadEntryBool(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_CLEANUP_TMP_KEY, ServicetConstants.DEFAULT_CLEANUP_TMP);
                BusyWaitLoopNumMax = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_CPU_CHK_LOOPMAX, ServicetConstants.DEFAULT_CPU_CHK_LOOPMAX);
                CPU_Percentage = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_CPU_CHK_PERCENTAGE, ServicetConstants.DEFAULT_CPU_CHK_PERCENTAGE);
                DefaultPrinterTimeout = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_PRINTER_GET_TIMEOUT, ServicetConstants.DEFAULT_PRINTER_GET_TIMEOUT);
                if (DefaultPrinterTimeout < ServicetConstants.DEFAULT_PRINTER_GET_MIN) {
                    DefaultPrinterTimeout = ServicetConstants.DEFAULT_PRINTER_GET_TIMEOUT;
                }
                DefaultPrinterCheckInterval = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_PRINTER_GETCHK_INTERVAL, ServicetConstants.DEFAULT_PRINTER_GETCHK_INTERVAL);
                if (DefaultPrinterCheckInterval >= DefaultPrinterTimeout)
                {
                    DefaultPrinterCheckInterval = ServicetConstants.DEFAULT_PRINTER_GETCHK_INTERVAL;
                }
                FormCreateRetryNum = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_FORM_RETRY_NUM, ServicetConstants.DEFAULT_FORM_RETRY_NUM);
                FormCreateRetryWaitMsec = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_FORM_RETRYWAIT_MSEC, ServicetConstants.DEFAULT_FORM_RETRYWAIT_MSEC);
                FormCreateTimeoutCheckMsec = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_FORM_TIMEOUT_ROOP_MSEC, ServicetConstants.DEFAULT_FORM_TIMEOUT_ROOP_MSEC);
                FormCreateTimeoutMsec = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_FORM_TIMEOUT_CHECK_MSEC, ServicetConstants.DEFAULT_FORM_TIMEOUT_CHECK_MSEC);
                MaxHistoryNum = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_HISTMAX_KEY, ServicetConstants.DEFAULT_HISTMAX);
                HistoryHoldTime = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_HOLD_TIME_KEY, ServicetConstants.DEFAULT_HOLDTIME);
                KillProcNames = xmlLoader.ReadEntry(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_KILL_PROC_NAMES, ServicetConstants.DEFAULT_KILL_PROC_NAMES);
                KillProcPrintCount = xmlLoader.ReadEntryIntCanMinus(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_KILL_PROCS_COUNT, ServicetConstants.DEFAULT_KILL_PROCS_COUNT);
                LoadRetryNum = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_LOAD_RETRY_NUM, ServicetConstants.DEFAULT_LOAD_RETRY_NUM);
                LoadRetryWaitMsec = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_LOAD_RETRYWAIT_MSEC, ServicetConstants.DEFAULT_LOAD_RETRYWAIT_MSEC);
                PortNo = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_PORT_KEY, ServicetConstants.DEFAULT_PORT);
                string tmp = xmlLoader.ReadEntry(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_PRINT_BY_THREAD, ServicetConstants.DEFAULT_PRINT_BY_THREAD);
                PrintFormByThread = chkThreadMode(tmp);
                PrintFormTimerInterval = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_PRINTFORM_TIMER_INTREVAL, ServicetConstants.DEFAULT_PRINTFORM_TIMER_INTREVAL);
                if (PrintFormTimerInterval < 1)
                {
                    PrintFormTimerInterval = ServicetConstants.DEFAULT_PRINTFORM_TIMER_INTREVAL;
                }
                PrintProcessThreadWaitMsec = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_PRINTTHREAD_WAIT_MSEC, ServicetConstants.DEFAULT_PRINTTHREAD_WAIT_MSEC);
                QueueMaxSize = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_QUEUESIZE_KEY, ServicetConstants.DEFAULT_QUEUESIZE);
                //再起動関連はバッチのみ
                DoRestartAtBatch = xmlLoader.ReadEntryBool(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_RESTART_FLG, ServicetConstants.DEFAULT_RESTART_FLG);
                DoRestartWaitTimeMin = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_RESTART_MIN, ServicetConstants.DEFAULT_RESTART_MIN);
                DoRestartPrintNum = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_RESTART_PRINTNUM, ServicetConstants.DEFAULT_RESTART_PRINTNUM);
                SpoolCheckTimeOut = xmlLoader.ReadEntryInt(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_SPOOLTIMEOUT_KEY, ServicetConstants.DEFAULT_SPOOLTIMEOUT);
                SppPassword = xmlLoader.ReadEntry(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_SPP_PATH_KEY, ServicetConstants.DEFAULT_SPP_PATH);
                TmpFolderPath = xmlLoader.ReadEntry(ServicetConstants.INI_SECTION_APP, ServicetConstants.INI_TMP_FOLDER_PATH_KEY, ServicetConstants.DEFAULT_TMP_FOLDER);
                if (TmpFolderPath.Length == 0)
                {
                    TmpFolderPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\brainsellers\\BatchPrint\\tmp";
                }

            }

            string dbgLog = "";

            if (ServiceType == CommonConstants.MODE_DIRECT)
            {
                dbgLog += "\r\nbrowserset=" + readBrowserset;
                dbgLog += "\r\ncleanup_tmpfolder=" + CleanupTmpFolder;
                dbgLog += "\r\ncpucheckloopmax=" + BusyWaitLoopNumMax;
                dbgLog += "\r\ncpucheckpercentage=" + CPU_Percentage;
                dbgLog += "\r\ndefaultBrowser=" + DefaultBrowserType;
                dbgLog += "\r\ndefaultPrinterCheckInterval=" + DefaultPrinterCheckInterval;
                dbgLog += "\r\ndefaultPrinterTimeout=" + DefaultPrinterTimeout;
                dbgLog += "\r\nexitTimerEnabled=" + ExitByTimerEnabled;
                dbgLog += "\r\nexitTimerLimit=" + ExitTimerLimit;
                dbgLog += "\r\nformCreateRetryNum=" + FormCreateRetryNum;
                dbgLog += "\r\nformCreateRetryWaitMsec=" + FormCreateRetryWaitMsec;
                dbgLog += "\r\nformCreateTimeoutCheckMsec=" + FormCreateTimeoutCheckMsec;
                dbgLog += "\r\nformCreateTimeoutMsec=" + FormCreateTimeoutMsec;
                dbgLog += "\r\nhistMax=" + MaxHistoryNum;
                dbgLog += "\r\nholdTime=" + HistoryHoldTime;
                dbgLog += "\r\nkillProcessNames=" + KillProcNames;
                dbgLog += "\r\nkillProcessPrintCount=" + KillProcPrintCount;
                dbgLog += "\r\nloadRetryNum=" + LoadRetryNum;
                dbgLog += "\r\nloadRetryWaitMsec=" + LoadRetryWaitMsec;
                dbgLog += "\r\nport=" + PortNo;
                dbgLog += "\r\nprintDlgFindTimeOut=" + PrintDialogFindTimeOut;
                dbgLog += "\r\nprintDlgLeaveTimeOut=" + PrintDialogLeaveTimeOut;
                dbgLog += "\r\nprintDlgName=" + AcrobatPrintDialogName;
                dbgLog += "\r\nprintDlgStayingTimeCheck=" + PrintDialogStayingTimeCheck;
                dbgLog += "\r\nprintFormTimerInterval=" + PrintFormTimerInterval;
                dbgLog += "\r\nprintProcessThreadWaitMsec=" + PrintProcessThreadWaitMsec;
                dbgLog += "\r\nprintformbythread=" + TH_MODE[PrintFormByThread];
                dbgLog += "\r\nqueueSize=" + QueueMaxSize;
                dbgLog += "\r\nresponseTemplate=" + ResponseTemplateHtmlFile;
                dbgLog += "\r\nspoolTimeOut=" + SpoolCheckTimeOut;
                dbgLog += "\r\nsppPass=" + SppPassword;
                dbgLog += "\r\ntmpFolderPath=" + TmpFolderPath;
            }
            else {
                dbgLog += "\r\ncleanup_tmpfolder=" + CleanupTmpFolder;
                dbgLog += "\r\ncpucheckloopmax=" + BusyWaitLoopNumMax;
                dbgLog += "\r\ncpucheckpercentage=" + CPU_Percentage;
                dbgLog += "\r\ndefaultPrinterCheckInterval=" + DefaultPrinterCheckInterval;
                dbgLog += "\r\ndefaultPrinterTimeout=" + DefaultPrinterTimeout;
                dbgLog += "\r\nformCreateRetryNum=" + FormCreateRetryNum;
                dbgLog += "\r\nformCreateRetryWaitMsec=" + FormCreateRetryWaitMsec;
                dbgLog += "\r\nformCreateTimeoutCheckMsec=" + FormCreateTimeoutCheckMsec;
                dbgLog += "\r\nformCreateTimeoutMsec=" + FormCreateTimeoutMsec;
                dbgLog += "\r\nhistMax=" + MaxHistoryNum;
                dbgLog += "\r\nholdTime=" + HistoryHoldTime;
                dbgLog += "\r\nkillProcessNames=" + KillProcNames;
                dbgLog += "\r\nkillProcessPrintCount=" + KillProcPrintCount;
                dbgLog += "\r\nloadRetryNum=" + LoadRetryNum;
                dbgLog += "\r\nloadRetryWaitMsec=" + LoadRetryWaitMsec;
                dbgLog += "\r\nport=" + PortNo;
                dbgLog += "\r\nprintFormTimerInterval=" + PrintFormTimerInterval;
                dbgLog += "\r\nprintProcessThreadWaitMsec=" + PrintProcessThreadWaitMsec;
                dbgLog += "\r\nprintformbythread=" + TH_MODE[PrintFormByThread];
                dbgLog += "\r\nqueueSize=" + QueueMaxSize;
                dbgLog += "\r\nrestartflg=" + DoRestartAtBatch;
                dbgLog += "\r\nrestartmin=" + DoRestartWaitTimeMin;
                dbgLog += "\r\nrestartprintnum=" + DoRestartPrintNum;
                dbgLog += "\r\nspoolTimeOut=" + SpoolCheckTimeOut;
                dbgLog += "\r\nsppPass=" + SppPassword;
                dbgLog += "\r\ntmpFolderPath=" + TmpFolderPath;
            }

            LogUtility.OutputLog("013", dbgLog);

            SpoolCheckTimeOut += LoadRetryNum * LoadRetryWaitMsec;

            //印刷スレッド停止無応答タイムアウト判定時間
            NoQueCheckTimeoutMsec = (SpoolCheckTimeOut + (FormCreateRetryNum * FormCreateRetryWaitMsec) + (LoadRetryNum * LoadRetryWaitMsec)) * 2;

            return true;
        }
        ///<summary>
        /// 最終イベント発生時刻にUPDATE
        /// </summary>
        public static void UpdateLatestEvent()
        {
            //現在時刻への書き換え
            lock (TimeLockObj)
            {
                LatestEventTime = DateTime.Now;
            }
        }
        ///<summary>
        /// 最終キューチェック時刻にUPDATE
        /// </summary>
        public static void UpdateLastQueCheck()
        {
            //現在時刻への書き換え
            lock (QueCheckLockObj)
            {
                LastQueCheckTime = DateTime.Now;
            }
        }
        ///<summary>
        /// 設定ファイルから取得したブラウザ名と起動プロセス名のセットを保管する
        /// </summary>
        /// <param name="names">in=outのカンマ区切り配列</param>
        public void setBrowserNames(String names)
        {
            string[] splittedSet = names.Split(',');
            ProcNames = new String[splittedSet.Length];
            BrowserNames = new String[splittedSet.Length];
            int Num = 0;
            foreach (string set in splittedSet)
            {
                string[] bName = set.Split('=');
                if (bName.Length == 2)
                {
                    ProcNames[Num] = bName[0];
                    BrowserNames[Num] = bName[1];
                    Num++;
                }
            }
        }
    }
}
