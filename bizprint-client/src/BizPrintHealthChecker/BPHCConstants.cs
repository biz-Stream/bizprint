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

namespace BizPrintHealthChecker
{
    /// <summary>
    /// BizPrintHealthCheckerでのみ使用する固定値定義クラス
    /// </summary>

    public static class BPHCConstants
    {
        //対象プロセス名・引数チェック参照値
        public static string PROCESSNAME_BPHC = "BizPrintHealthChecker";
        public static string PROCESSNAME_DIRECT = "DirectPrintService";
        public static string PROCESSNAME_BATCH = "BatchPrintService";

        ///Direct/Batchの切り替えフラグ
        public static int MODE_DIRECT = 0;
        public static int MODE_BATCH = 1;


        /// AppData/Program Filesより下のフォルダパス
        public const string CONF_FOLDER_DIRECT = "brainsellers\\DirectPrint";
        public const string CONF_FOLDER_BATCH = "brainsellers\\BatchPrint";
        /// 設定ファイル名
        public static string CONFFILE_BPHC = "BizPrintHealthChecker.xml";
        public static string LOGCONF_BPHC = "BizPrintHealthChecker_logConfig.xml";

        //ログ日本語文字列ファイル名
        public static string LogConfBaseBPHC = "BizPrintHealthChecker_log_";
        public static string LogIDBaseBPHC = "HC";

        /// INIファイル読み込みキーとデフォルト値(接続リトライ回数)
        public const string INI_CONNECT_RETRY_KEY = "connectRetryNum";
        public const int DEFAULT_CONNECT_RETRY = 5;

        /// INIファイル読み込みキーとデフォルト値(接続リトライ間隔)
        public const string INI_CONNECT_RETRYWAIT_MSEC_KEY = "connectRetryWaitMsec";
        public const int DEFAULT_CONNECT_RETRYWAIT_MSEC = 200;

        /// INIファイル読み込みキーとデフォルト値(接続タイムアウト値)
        public const string INI_CONNECT_TIMEOUT_KEY = "connectTimeout";
        public const int DEFAULT_CONNECT_TIMEOUT = 2000;

        /// INIファイル読み込みキーとデフォルト値(プロセス終了結果チェックリトライ回数)
        public const string INI_KILLEDCHK_RETRY_KEY = "killedCheckRetryNum";
        public const int DEFAULT_KILLEDCHK_RETRY = 5;

        /// INIファイル読み込みキーとデフォルト値(isalive取得サーバアドレス)
        public const string INI_SERVER_ADDRESS_KEY = "serverAddress";
        public const string DEFAULT_SERVER_ADDRESS = "127.0.0.1";

        /// INIファイル読み込みキーとデフォルト値(ポート番号)
        public const string INI_PORTNO_KEY = "port";
        public const int DEFAULT_PORTNO = 3000;

        /// INIファイル読み込みセクション名(共通)
        public const string INI_SECTION_APP = "Application";

        //自身の設定ファイル名
        public const string SETTINGFILE_BPHC = "BizPrintHealthChecker.xml";

        //ダイレクト印刷時のチェック対象ファイル名
        public const string SETTINGFILE_DIRECT = "DirectPrintService.xml";

        //バッチ印刷時のチェック対象ファイル名
        public const string SETTINGFILE_BATCH = "BatchPrintService.xml";

        //ログ設定読み込み前に出力する固定ログ内容
        public const string BPHC_Static_logString_001 = "BizPrintHealthChecker Start.";
        public const string BPHC_Static_logString_002 = "args=";

        public const string BPHC_Static_logString_100 = "BizPrintHealthChecker End.";

    }
}
