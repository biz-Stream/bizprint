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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BizPrintCommon
{
    /// <summary>
    /// 共通DLL・および全体で使用する固定データ定義
    /// </summary>
    public static class CommonConstants
    {
        /// <summary>ログレベル</summary>
        public const int LOGLEVEL_FATAL = 0x05;
        public const int LOGLEVEL_ERROR = 0x04;
        public const int LOGLEVEL_WARN = 0x03;
        public const int LOGLEVEL_INFO = 0x02;
        public const int LOGLEVEL_DEBUG = 0x01;

        /// <summary>固定ログ文字列定義</summary>
        public const string Static_logString_002 = "CurrentCulture.Name is {0}.Read file name ={1}.";
        public const string Static_logString_005 = "Read Log String File is {0}";
        public const string Static_logString_006 = "file {0} can not read. only ID will out.";

        public const string Static_logString_004 = "ログID個数：{0}個";

        public static string ERR_MSG_LOGSETTING = "log setting file [{0}] not found. \r\ncan't write log file.";

        public static string INFO_MSG_LOGSETTING = "log setting file [{0}] not found. \r\ncan't write log file.";

        /// <summary>起動元として許可されるブラウザのプロセス名</summary>
        public static string[] browserProcessnames = { "browser_broker", "iexplore", "firefox", "chrome", "RuntimeBroker" };

        ///URL種別
        /// <summary>印刷指示要求</summary>
        public const string URL_PRINTSTART = @"/doprint";
        /// <summary>印刷指示要求</summary>
        public const string URL_PRTSTATGET = @"/getstatus";
        /// <summary>印刷指示応答</summary>
        public const string URL_DORESPONSE = @"/doresponse";
        /// <summary>印刷指示応答</summary>
        public const string URL_FAVICON = @"/favicon.ico";
        /// <summary>生存状態確認</summary>
        public const string URL_ISALIVE = @"/isalive";

        /// <summary>URL種別数値定義</summary>
        public const int URLTYPE_PRINTSTART = 1;
        public const int URLTYPE_PRTSTATGET = 2;
        public const int URLTYPE_DORESPONSE = 3;
        public const int URLTYPE_FAVICON = 4;
        public const int URLTYPE_OTHER = 5;
        public const int URLTYPE_ISALIVE = 6;

        /// <summary>受信ソケット読み取り単位 </summary>
        public const int RECV_LEN = 1024;

        /// <summary>Direct/Batchの切り替えフラグ</summary>
        public static int MODE_DIRECT = 0;
        public static int MODE_BATCH = 1;
        /// <summary>排他タイムアウト</summary>
        public const int LOCK_TIMEOUT = 30000;  //msec

        /// <summary>SilentPdfPrinterからDirectに送信する際の区切り文字(NNN=YYYY&)</summary>
        public const string SEND_PARANT_NAME = "parent=";
        public const string SEND_DATA_BYTES = "sppdata=";

        public const string SPPPATH_BEFORE = "___RANDOM_STRINGS1___";
        public const string SPPPATH_AFTER = "___RANDOM_STRINGS2___";

        /// <summary>SPPファイル内の印刷パラメータ取り出し用文字列</summary>
        public const string PRINT_PARAM_PRINTERNAME = "printerName";
        public const string PRINT_PARAM_NUM_OF_COPY = "numberOfCopy";
        public const string PRINT_PARAM_SELECTED_TRAY = "selectedTray";
        public const string PRINT_PARAM_JOBNAME = "jobName";
        public const string PRINT_PARAM_DOFIT = "doFit";
        public const string PRINT_PARAM_RESP_URL = "responseURL";
        public const string PRINT_PARAM_SAVE_FILENAME = "saveFileName";
        public const string PRINT_PARAM_TARGET = "target";
        public const string PRINT_PARAM_PRINT_DLG = "printDialog";
        public const string PRINT_PARAM_FROMPAGE = "fromPage";
        public const string PRINT_PARAM_TOPAGE = "toPage";


        /// <summary>HTTP通信内の固定文字列</summary>
        public const string HTTP_POST = "POST";
        public const string HTTP_GET = "GET";
        public const string HTTP_HTTP = "HTTP";
        public const string HTTP_ = "";


        #region 印刷要求返信固定文字列
        public const string RESULT = "RESULT";
        public const string ERROR_CODE = "ERROR_CODE";
        public const string ERROR_CAUSE = "ERROR_CAUSE";
        public const string ERROR_DETAILS = "ERROR_DETAILS";
        public const string JOBID = "jobID";
        #endregion

        #region 印刷要求返信結果
        /// <summary>成功</summary>
        public const string SUCCESS = "SUCCESS";
        /// <summary>失敗</summary>
        public const string FAIL = "FAIL";


        /// <summary>接続異常</summary>
        public const string NOT_CONNECT = "NOT_CONNECT";
        /// <summary>印刷容量異常</summary>
        public const string QUEUE_FULL = "QUEUE_FULL";
        /// <summary>内部異常</summary>
        public const string SERVER_ERROR = "SERVER_ERROR";

        #endregion

        /// <summary>トレイ識別子</summary>
        public const string FIRST = "FIRST";
        public const string UPPER = "UPPER";
        public const string ONLYONE = "ONLYONE";
        public const string LOWER = "LOWER";
        public const string MIDDLE = "MIDDLE";
        public const string MANUAL = "MANUAL";
        public const string ENVELOPE = "ENVELOPE";
        public const string ENVMANUAL = "ENVMANUAL";
        public const string AUTO = "AUTO";
        public const string TRACTOR = "TRACTOR";
        public const string SMALLFMT = "SMALLFMT";
        public const string LARGEFMT = "LARGEFMT";
        public const string LARGECAPACITY = "LARGECAPACITY";
        public const string CASETTE = "CASSETTE";
        public const string FORMSOURCE = "FORMSOURCE";
        public const string LAST = "LAST";
        public const string CUSTOM = "CUSTOM";

        /// <summary>トレイ数値</summary>
        public const long DMBIN_NONE = 0;
        public const long DMBIN_FIRST = 1;
        public const long DMBIN_UPPER = 1;
        public const long DMBIN_ONLYONE = 1;
        public const long DMBIN_LOWER = 2;
        public const long DMBIN_MIDDLE = 3;
        public const long DMBIN_MANUAL = 4;
        public const long DMBIN_ENVELOPE = 5;
        public const long DMBIN_ENVMANUAL = 6;
        public const long DMBIN_AUTO = 7;
        public const long DMBIN_TRACTOR = 8;
        public const long DMBIN_SMALLFMT = 9;
        public const long DMBIN_LARGEFMT = 10;
        public const long DMBIN_LARGECAPACITY = 11;
        public const long DMBIN_CASETTE = 14;
        public const long DMBIN_FORMSOURCE = 15;
        public const long DMBIN_LAST = 15;
        public const long DMBIN_CUSTOM = 257;

        /// <summary>プリンタ名から判定されるプリンタ種別</summary>
        public const int PRINTER_TYPE_OTHER = 0;
        public const int PRINTER_TYPE_LOCAL = 1;
        public const int PRINTER_TYPE_UNC = 2;
        public const int PRINTER_TYPE_IPP = 3;
        /// <summary>
        /// プリンタ名からプリンタ種別を返す
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        public static int chkPrinterType(string printerName)
        {
            int rtn = PRINTER_TYPE_OTHER;
            try
            {
                Uri urichk = new Uri(printerName);
                if (urichk.IsUnc)
                {
                    rtn = PRINTER_TYPE_UNC;
                }

            }
            catch (Exception e)
            {
                if (printerName.StartsWith("\\\\") && printerName.IndexOf("http") > 0)
                {
                    rtn = PRINTER_TYPE_IPP;
                }
                else
                {
                    rtn = PRINTER_TYPE_LOCAL;
                }
            }

            return rtn;
        }

        /// <summary>固定ログ定義</summary>
        public const string STATIC_LOG_001 = "Running CLR Version：";
        public const string STATIC_LOG_002 = "Running CLR Directory:";
        public const string STATIC_LOG_003 = "System Configuration File:";

        public const string JA_JP_DEFAULT = "ja-JP";
        public const string CONF_EXT = ".txt";


        #region 印刷履歴のステータスコード
        /// <summary>
        /// 印刷指示受信直後(作成時)(未使用)
        /// </summary>
        public const int JOB_STATUS_RECIEVED = 0x00;
        /// <summary>
        /// 印刷指示受付(印刷順待ち)状態 キューにある状態
        /// </summary>
        public const int JOB_STATUS_ACCEPT = 0x02;
        /// <summary>
        /// <summary>
        /// 印刷中。キューから取り出されて、印刷処理が実行中
        /// </summary>
        public const int JOB_STATUS_PRINTING = 0x04;
        /// <summary>
        /// 印刷要求をプリンタに送信し終わった状態
        /// </summary>
        public const int JOB_STATUS_SUCCESS_FINISH = 0x06;
        /// <summary>
        /// 印刷処理がエラー終了した状態
        /// </summary>
        public const int JOB_STATUS_ERROR_FINISH = 0x08;
        /// <summary>
        /// 印刷処理がタイムアウトした状態
        /// </summary>
        public const int JOB_STATUS_TIMEOUT = 0x10;
        #endregion

        #region 印刷履歴のステータス文字列(日本語・英語)
        public const string JOB_STATUS_STRING_RECIEVED = "印刷指示受信";
        public const string JOB_STATUS_STRING_ACCEPT = "印刷指示受付";
        public const string JOB_STATUS_STRING_PRINTING = "印刷中";
        public const string JOB_STATUS_STRING_SUCCESS_FINISH = "印刷要求送信完了";
        public const string JOB_STATUS_STRING_ERROR_FINISH = "印刷異常終了";
        public const string JOB_STATUS_STRING_TIMEOUT = "印刷要求送信タイムアウト";

        public const string JOB_STATUS_ENG_RECIEVED = "Receive Print Request";
        public const string JOB_STATUS_ENG_ACCEPT = "Print Request Acceptance";
        public const string JOB_STATUS_ENG_PRINTING = "Printing in Progress";
        public const string JOB_STATUS_ENG_SUCCESS_FINISH = "Print Request Transmission Complete";
        public const string JOB_STATUS_ENG_ERROR_FINISH = "Print Abnormal Termination";
        public const string JOB_STATUS_ENG_TIMEOUT = "Print Request Transmission Timeout";
        #endregion

        /// <summary>
        /// ステータスコードから文字列への変換処理
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string ChangeStatusCodeToString(int code)
        {
            System.Globalization.CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            string rtn = "";
            if (currentCulture.Name.Equals(JA_JP_DEFAULT))
            {
                switch (code)
                {
                    case JOB_STATUS_RECIEVED:
                        rtn = JOB_STATUS_STRING_RECIEVED;
                        break;
                    case JOB_STATUS_ACCEPT:
                        rtn = JOB_STATUS_STRING_ACCEPT;
                        break;
                    case JOB_STATUS_PRINTING:
                        rtn = JOB_STATUS_STRING_PRINTING;
                        break;
                    case JOB_STATUS_SUCCESS_FINISH:
                        rtn = JOB_STATUS_STRING_SUCCESS_FINISH;
                        break;
                    case JOB_STATUS_ERROR_FINISH:
                        rtn = JOB_STATUS_STRING_ERROR_FINISH;
                        break;
                    case JOB_STATUS_TIMEOUT:
                        rtn = JOB_STATUS_STRING_TIMEOUT;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (code)
                {
                    case JOB_STATUS_RECIEVED:
                        rtn = JOB_STATUS_ENG_RECIEVED;
                        break;
                    case JOB_STATUS_ACCEPT:
                        rtn = JOB_STATUS_ENG_ACCEPT;
                        break;
                    case JOB_STATUS_PRINTING:
                        rtn = JOB_STATUS_ENG_PRINTING;
                        break;
                    case JOB_STATUS_SUCCESS_FINISH:
                        rtn = JOB_STATUS_ENG_SUCCESS_FINISH;
                        break;
                    case JOB_STATUS_ERROR_FINISH:
                        rtn = JOB_STATUS_ENG_ERROR_FINISH;
                        break;
                    case JOB_STATUS_TIMEOUT:
                        rtn = JOB_STATUS_ENG_TIMEOUT;
                        break;
                    default:
                        break;
                }
            }

            return rtn;
        }
    }
}
