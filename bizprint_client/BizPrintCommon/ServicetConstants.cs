using BizPrintCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizPrintCommon
{
    /// <summary>
    /// 固定値定義クラス
    /// </summary>
    public static class ServicetConstants
    {

        #region ダイレクト印刷
        /// AppData/Program Filesより下のフォルダパス
        public const string DirectConfFolderName = "brainsellers\\DirectPrint";
        public const string BatchConfFolderName = "brainsellers\\BatchPrint";

        /// 設定ファイル名
        public static string DPconfFileName = "DirectPrintService.xml";
        public static string BatchconfFileName = "BatchPrintService.xml";
        /// ログ設定ファイル名
        public static string DPlogConfFileName = "DirectPrintService_logConfig.xml";
        public static string BPlogConfFileName = "BatchPrintService_logConfig.xml";


        public static string DPlogConfBaseDirect = "DirectPrintService_log_";
        public static string BPlogConfBaseBatch = "BatchPrintService_log_";
        public static string DPlogIDBase = "DP";
        public static string BPlogIDBaseBatch = "BP";
        #endregion

        #region バッチ印刷
        /// AppData/Program Filesより下のフォルダパス
        public const string BAcompFolderName = "brainsellers\\BatchPrint";

        /// 設定ファイル名
        public static string BAconfFileName = "BatchPrintService.xml";
        /// ログ設定ファイル名
        public static string BAlogConfFileName = "BatchPrintService_logConfig.xml";


        public static string BAlogConfBaseBatch = "BatchPrintService_log_";
        public static string BAlogIDBase = "BP";
        #endregion

        //ログ設定読み込み前に出力する固定ログ内容
        public const string Direct_Static_logString_001 = "DirectPrintService Start.";

        /// INIファイル読み込みセクション名(共通)
        public const string INI_SECTION_APP = "Application";

        /// INIファイル読み込みキーとデフォルト値()
        public const string INI_SPOOLTIMEOUT_KEY = "spoolTimeOut";
        public const int DEFAULT_SPOOLTIMEOUT = 10000;

        /// INIファイル読み込みキーとデフォルト値()
        public const string INI_SPP_PATH_KEY = "sppPass";
        public const string DEFAULT_SPP_PATH = "";

        /// INIファイル読み込みキーとデフォルト値()
        public const string INI_HISTMAX_KEY = "histMax";
        public const int DEFAULT_HISTMAX = 100;

        /// INIファイル読み込みキーとデフォルト値()
        public const string INI_PORT_KEY = "port";
        public const int DEFAULT_PORT = 3000;

        /// INIファイル読み込みキーとデフォルト値()
        public const string INI_QUEUESIZE_KEY = "queueSize";
        public const int DEFAULT_QUEUESIZE = 100;

        /// INIファイル読み込みキーとデフォルト値()
        public const string INI_HOLD_TIME_KEY = "holdTime";
        public const int DEFAULT_HOLDTIME = 86400;//sec = 1440min = 24h

        /// INIファイル読み込みキーとデフォルト値()
        public const string INI_EXIT_TIME_ENABLE_KEY = "exitTimerEnabled";
        public const bool DEFAULT_EXIT_TIME_ENABLE = true;

        /// INIファイル読み込みキーとデフォルト値()
        public const string INI_EXIT_TIME_LIMIT_KEY = "exitTimerLimit";
        public const int DEFAULT_EXIT_TIME_LIMIT = 30;

 
        /// INIファイル読み込みキーとデフォルト値()
        public const string INI_RESPONCE_TEMPLATE_KEY = "responseTemplate";
        public const string DEFAULT_RESPONCE_TEMPLATE = "";

        /// INIファイル読み込みキーとデフォルト値()
        public const string INI_DEFAULT_BROWSER_KEY = "defaultBrowser";
        public const string DEFAULT_BROWSER = "iexplore";

        /// INIファイル読み込みキーとデフォルト値()
        public const string INI_TMP_FOLDER_PATH_KEY = "tmpFolderPath";
        public const string DEFAULT_TMP_FOLDER = "";

        /// INIファイル読み込みキーとデフォルト値()
        public const string INI_CLEANUP_TMP_KEY = "cleanup_tmpfolder";
        public const bool DEFAULT_CLEANUP_TMP = true;

        /// INIファイル読み込みキーとデフォルト値(印刷ダイアログタイトル)
        public const string INI_PRINT_DLG_NAME_KEY = "printDlgName";
        public const string DEFAULT_PRINT_DLG_NAME = "印刷|進行状況";

        /// INIファイル読み込みキーとデフォルト値(起動プロセス名と返信先ブラウザのセット)
        public const string INI_BROWSER_SET_KEY = "browserset";
        public const string DEFAULT_BROWSER_SET = "iexplore=iexplore,firefox=firefox,chrome=chrome,browser_broker=Edge,RuntimeBroker=Edge,explorer=Edge,sihost=Edge,msedge=msedge";

        /// INIファイル読み込みキーとデフォルト値(キュー取り出しCPUチェックパーセント)
        public const string INI_CPU_CHK_PERCENTAGE = "cpucheckpercentage";
        public const int DEFAULT_CPU_CHK_PERCENTAGE = 70;

        /// INIファイル読み込みキーとデフォルト値(キュー取り出しCPUチェック最大回数)
        public const string INI_CPU_CHK_LOOPMAX = "cpucheckloopmax";
        public const int DEFAULT_CPU_CHK_LOOPMAX = 0;

        /// INIファイル読み込みキーとデフォルト値(バッチ印刷自動再起動フラグ)
        public const string INI_RESTART_FLG = "restartflg";
        public const bool DEFAULT_RESTART_FLG = true;

        /// INIファイル読み込みキーとデフォルト値(バッチ印刷自動再起動部数)
        public const string INI_RESTART_PRINTNUM = "restartprintnum";
        public const int DEFAULT_RESTART_PRINTNUM = 128;

        /// INIファイル読み込みキーとデフォルト値(バッチ印刷自動再起動時間)
        public const string INI_RESTART_MIN = "restartmin";
        public const int DEFAULT_RESTART_MIN = 20;

        /// INIファイル読み込みキーとデフォルト値(印刷フォームのスレッド起動)
        public const string INI_PRINT_BY_THREAD = "printformbythread";
        public const string DEFAULT_PRINT_BY_THREAD = "auto";

        /// INIファイル読み込みキーとデフォルト値(印刷フォームのタイマインターバル)
        public const string INI_PRINTFORM_TIMER_INTREVAL = "printFormTimerInterval";
        public const int DEFAULT_PRINTFORM_TIMER_INTREVAL = 100;

        /// INIファイル読み込みキーとデフォルト値(印刷スレッドのループ待機時間)
        public const string INI_PRINTTHREAD_WAIT_MSEC = "printProcessThreadWaitMsec";
        public const int DEFAULT_PRINTTHREAD_WAIT_MSEC = 100;

        /// INIファイル読み込みキーとデフォルト値(ダイアログを発見出来ない時のタイムアウト)
        public const string INI_PRINTDLG_FIND = "printDlgFindTimeOut";
        public const int DEFAULT_PRINTDLG_FIND = 10000;

        /// INIファイル読み込みキーとデフォルト値(一定時間以上ダイアログが表示されたままの時のログ出力までの時間)
        public const string INI_PRINTDLG_STAYING = "printDlgStayingTimeCheck";
        public const int DEFAULT_PRINTDLG_STAYING = 60000;

        /// INIファイル読み込みキーとデフォルト値(ダイアログが閉じてからフォームが終了するまでのタイムアウト)
        public const string INI_PRINTDLG_LEAVE = "printDlgLeaveTimeOut";
        public const int DEFAULT_PRINTDLG_LEAVE = 3;
        public const int DEFAULT_PRINTDLG_LEAVE_MIN = 1;


        /// INIファイル読み込みキーとデフォルト値(ロード失敗時のリトライ回数)
        public const string INI_LOAD_RETRY_NUM = "loadRetryNum";
        public const int DEFAULT_LOAD_RETRY_NUM = 5;

        /// INIファイル読み込みキーとデフォルト値(ロード失敗時のリトライ待機時間)
        public const string INI_LOAD_RETRYWAIT_MSEC = "loadRetryWaitMsec";
        public const int DEFAULT_LOAD_RETRYWAIT_MSEC = 1000;


        /// フォーム作成処理タイムアウト検知時間(msec)
        public const string INI_FORM_TIMEOUT_CHECK_MSEC = "formCreateTimeoutMsec";
        public const int DEFAULT_FORM_TIMEOUT_CHECK_MSEC = 2500;
        /// フォーム作成処理タイムアウトチェックループ時間(msec)
        public const string INI_FORM_TIMEOUT_ROOP_MSEC = "formCreateTimeoutCheckMsec";
        public const int DEFAULT_FORM_TIMEOUT_ROOP_MSEC = 100;
        /// フォーム作成処理最大リトライ回数
        public const string INI_FORM_RETRY_NUM = "formCreateRetryNum";
        public const int DEFAULT_FORM_RETRY_NUM = 5;
        /// フォーム作成処理リトライ待機時間(msec)
        public const string INI_FORM_RETRYWAIT_MSEC = "formCreateRetryWaitMsec";
        public const int DEFAULT_FORM_RETRYWAIT_MSEC = 500;

        /// Kill対象プロセス名
        public const string INI_KILL_PROC_NAMES = "killProcessNames";
        public const string DEFAULT_KILL_PROC_NAMES = "AcroRd32,Acrobat,RdrCEF";

        /// Kill実行までの同じプリンタでの印刷回数
        public const string INI_KILL_PROCS_COUNT = "killProcessPrintCount";
        public const int DEFAULT_KILL_PROCS_COUNT = 0;

        /// デフォルトプリンタ取得タイムアウト(msec)
        public const string INI_PRINTER_GET_TIMEOUT = "defaultPrinterTimeout";
        public const int DEFAULT_PRINTER_GET_TIMEOUT = 500;
        public const int DEFAULT_PRINTER_GET_MIN = 100;

        /// デフォルトプリンタ取得タイムアウトチェックインターバル(msec)
        public const string INI_PRINTER_GETCHK_INTERVAL = "defaultPrinterCheckInterval";
        public const int DEFAULT_PRINTER_GETCHK_INTERVAL = 33;


    }
}
