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
using System.Threading.Tasks;

namespace SilentPdfPrinter
{
    /// <summary>
    /// SilentPdfPrinterでのみ使用する固定値定義クラス
    /// </summary>
    public static class SilentConstants
    {

        //起動元プロセス名
        public static string ParentProcessName;

        /// AppData/Program Filesより下のフォルダパス
        public const string ConfFolderName = "brainsellers\\DirectPrint";
        /// 設定ファイル名
        public static string ConfFileName = "SilentPdfPrinter.xml";
        public static string LogConfFileName = "SilentPdfPrinter_logConfig.xml";


        public static string LogConfBaseSilent = "SilentPdfPrinter_log_";
        public static string LogIDBaseSilent = "SI";



        /// INIファイル読み込みセクション名(共通)
        public const string INI_SECTION_APP = "Application";

        /// INIファイル読み込みキーとデフォルト値(リトライ間隔)
        public const string INI_RETRYINTERVAL_KEY = "retryinterval";
        public const int DEFAULT_RETRYINTERVAL = 5000;

        /// INIファイル読み込みキーとデフォルト値(リトライ回数)
        public const string INI_RETRY_KEY = "retry";
        public const int DEFAULT_RETRY = 5;

        /// INIファイル読み込みキーとデフォルト値(プロセス名)
        public const string INI_PROCESSNAME_KEY = "processname";
        public const string DEFAULT_PROCESSNAME = "DirectPrintService";

        /// INIファイル読み込みキーとデフォルト値(タイムアウト値)
        public const string INI_TIMEOUT_KEY = "timeout";
        public const int DEFAULT_TIMEOUT= 20000;

        /// INIファイル読み込みキーとデフォルト値(送信リトライ回数)
        public const string INI_SENDRETRY_KEY = "sendretry";
        public const int DEFAULT_SENDRETRY = 0;

        /// INIファイル読み込みキーとデフォルト値(ファイル削除フラグ)
        public const string INI_DELETE_FILE_KEY = "deletefile";
        public const bool DEFAULT_DELETE_FILE = true;

        /// INIファイル読み込みキーとデフォルト値(ダイレクト印刷ホスト名)
        public const string INI_DIRECT_PRINT_HOST_KEY = "directprinthost";
        public const string DEFAULT_DIRECT_PRINT_HOST = "localhost";
        public const string DEFAULT_DIRECT_PRINT_IP = "127.0.0.1";
        public const string DEFAULT_DIRECT_PRINT_IPV6 = "::1";

        /// INIファイル読み込みキーとデフォルト値(ポート番号)
        public const string INI_PORT_KEY = "port";
        public const int DEFAULT_PORT_KEY = 3000;

        /// INIファイル読み込みキーとデフォルト値(多重起動時の待機ループミリ秒)
        public const string INI_WAIT_MSEC = "waitloopmsec";
        public const int DEFAULT_WAIT_MSEC = 1000;

        /// INIファイル読み込みキーとデフォルト値(多重起動時の最大平行起動数)
        public const string INI_MAX_PROC_NUM = "maxprocessnum";
        public const int DEFAULT_MAX_PROC_NUM = 10;

        //ダイレクト印刷サービスexe名、プロセス取得時のMUTEX名
        public const string SERVICE_EXE_NAME = "DirectPrintService.exe";
        public const string SERVICE_MUTEX_START = "StartDirectPrintService";

        //接続先URLひな形({0}:host,{1}:port)
        public const string DIRECT_SERVICE_URL = "http://{0}:{1}/doprint";

        //ログ設定読み込み前に出力する固定ログ内容
        public const string Silent_Static_logString_001 = "SilentPdfPrinter Start.";

        /// INIファイル読み込みキーとデフォルト値(最大同時接続数)
        public const string INI_CONC_CONECT_MAX = "concurrentconnectionsmax";
        public const int DEFAULT_CONC_CONECT_MAX = 5;
    }
}
