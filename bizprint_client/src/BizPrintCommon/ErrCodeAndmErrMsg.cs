using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BizPrintCommon
{
    /// <summary>
    /// エラーコード・エラーメッセージ・エラー定義およびその取得クラス
    /// </summary>
    public static class ErrCodeAndmErrMsg
    {
        /// <summary>
        /// 読み込んだエラーメッセージ詳細を保持するDictionary
        /// </summary>
        private static Dictionary<int, string> ErrorDataDatail = new Dictionary<int, string>();
        /// <summary>
        /// エラーメッセージファイル名ベース
        /// </summary>
        private const string BaseFileName = "ErrorDetail_";

        /// <summary>
        /// 言語設定定数(固定値用)
        /// </summary>
        public const int CULTURE_JP = 0;
        public const int CULTURE_OTHER = 1;
        public static int CultureCode = CULTURE_JP;

        /// <summary>
        /// ファイル読み込み済みフラグ
        /// </summary>
        public static bool IsLoaded = false;
        /// <summary>
        /// 言語確認済みフラグ
        /// </summary>
        public static bool IsCulrueInited = false;

        /// <summary>
        /// エラー詳細文字列をファイルから読み込む
        /// </summary>
        /// <param name="ConfFolderPath">設定ファイルが置かれているフォルダの名前</param>
        /// <returns>STATUS_OK：成功 それ以外：失敗</returns>
        public static int LoadErrorDetailFile(string ConfFolderPath)
        {
            if (IsLoaded)
            {
                return STATUS_OK;
            }
            string filePath = "";
            System.Globalization.CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            string currentName = currentCulture.Name;
            string loadPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + ConfFolderPath + "\\";

#if DEBUG
            loadPath = "..\\..\\Config\\";
#endif
            string CheckPath = loadPath + BaseFileName + currentName + CommonConstants.CONF_EXT;
            if (System.IO.File.Exists(CheckPath))
            {
                filePath = CheckPath;
            }
            else
            {
                //存在しない場合のデフォルト(ja_JP)
                filePath = loadPath + BaseFileName + CommonConstants.JA_JP_DEFAULT + CommonConstants.CONF_EXT;
            }
            LogUtility.OutputLog("150", filePath);
            StreamReader streamRead = null;
            try
            {
                streamRead = new StreamReader(filePath, System.Text.Encoding.UTF8);
                setErrorDetail(streamRead);
                streamRead.Close();
                IsLoaded = true;
            }
            catch (Exception ex)
            {
                //ファイルが存在していない、ディレクトリが無い
                LogUtility.OutputLog("151", filePath, ex.Message);
                return -1;
            }

            return STATUS_OK;
        }
        /// <summary>
        /// ファイルから読み込んだIDと文字列を配置する
        /// </summary>
        /// <param name="streamRead"></param>
        private static void setErrorDetail(StreamReader streamRead)
        {
            int addedNum = 0;
            ErrorDataDatail.Clear();
            while (streamRead.Peek() > 0)
            {
                string stBuffer = streamRead.ReadLine();
                if (stBuffer.Length > 0)
                {
                    string[] splitted = stBuffer.Split(',');
                    if (splitted.Length < 2)
                    {
                        //errordata 少ないのは無視する。
                    }
                    else
                    {
                        int id = 0;
                        try
                        {
                            id = int.Parse(splitted[0]);
                            ErrorDataDatail.Add(id, splitted[1]);
                            addedNum++;
                        }
                        catch (Exception)
                        {
                            //errordata 1項目目がintじゃない場合は無視

                        }

                    }
                }
            }
        }
        /// <summary>
        /// 言語判別を行う
        /// </summary>
        public static void InitCulture()
        {
            //1回だけ行う。
            if (IsCulrueInited)
            {
                return;
            }
            System.Globalization.CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            string cname = currentCulture.Name;
            if (!cname.Equals("ja-JP"))
            {
                CultureCode = CULTURE_OTHER;

            }
            IsCulrueInited = true;

        }

        public const int STATUS_OK = 0x0000;

        //エラーコード 末尾99は終端識別用
        public const int ERR_CODE_0101 = 0101;
        public const int ERR_CODE_0102 = 0102;
        public const int ERR_CODE_0103 = 0103;
        public const int ERR_CODE_0104 = 0104;
        public const int ERR_CODE_0105 = 0105;
        public const int ERR_CODE_0106 = 0106;
        public const int ERR_CODE_0107 = 0107;
        public const int ERR_CODE_0108 = 0108;
        public const int ERR_CODE_0109 = 0109;
        public const int ERR_CODE_0110 = 0110;
        public const int ERR_CODE_0111 = 0111;
        public const int ERR_CODE_0112 = 0112;
        public const int ERR_CODE_0113 = 0113;
        public const int ERR_CODE_0114 = 0114;
        public const int ERR_CODE_0115 = 0115;
        public const int ERR_CODE_0199 = 0199;//各番台終端

        public const int ERR_CODE_0201 = 0201;
        public const int ERR_CODE_0202 = 0202;
        public const int ERR_CODE_0203 = 0203;
        public const int ERR_CODE_0204 = 0204;
        public const int ERR_CODE_0299 = 0299;//各番台終端

        public const int ERR_CODE_0301 = 0301;
        public const int ERR_CODE_0302 = 0302;
        public const int ERR_CODE_0399 = 0399;//各番台終端

        public const int ERR_CODE_0401 = 0401;
        public const int ERR_CODE_0402 = 0402;
        public const int ERR_CODE_0403 = 0403;
        public const int ERR_CODE_0404 = 0404;
        public const int ERR_CODE_0405 = 0405;
        public const int ERR_CODE_0406 = 0406;
        public const int ERR_CODE_0407 = 0407;
        public const int ERR_CODE_0408 = 0408;
        public const int ERR_CODE_0409 = 0409;
        public const int ERR_CODE_0410 = 0410;
        public const int ERR_CODE_0411 = 0411;
        public const int ERR_CODE_0412 = 0412;
        public const int ERR_CODE_0413 = 0413;
        public const int ERR_CODE_0499 = 0499;//各番台終端

        public const int ERR_CODE_0501 = 0501;
        public const int ERR_CODE_0502 = 0502;
        public const int ERR_CODE_0503 = 0503;
        public const int ERR_CODE_0504 = 0504;
        public const int ERR_CODE_0505 = 0505;
        public const int ERR_CODE_0506 = 0506;
        public const int ERR_CODE_0507 = 0507;
        public const int ERR_CODE_0508 = 0508;
        public const int ERR_CODE_0509 = 0509;
        public const int ERR_CODE_0599 = 0599;//各番台終端



        //エラーメッセージ
        public const string ERR_MSG_0000 = "正常終了";

        public const string ERR_MSG_0101 = "sppファイルを取得できない。";
        public const string ERR_MSG_0102 = "sppファイルを解凍できない。";
        public const string ERR_MSG_0103 = "sppファイルのサイズが異常。";
        public const string ERR_MSG_0104 = "sppファイルに印刷データファイルが含まれていない。";
        public const string ERR_MSG_0105 = "sppファイルに印刷パラメータファイルが含まれていない。";
        public const string ERR_MSG_0106 = "プリンタ名の指定が間違っている。";
        public const string ERR_MSG_0107 = "デフォルトプリンタが設定されていない。";
        public const string ERR_MSG_0108 = "プリンタのハンドルを取得できない。";
        public const string ERR_MSG_0109 = "トレイの指定が間違っている。";
        public const string ERR_MSG_0110 = "印刷部数の指定が間違っている。";
        public const string ERR_MSG_0111 = "印刷開始・終了ページ指定が間違っている。";
        public const string ERR_MSG_0112 = "レスポンスURLが間違っている。";
        public const string ERR_MSG_0113 = "ファイル保存パス指定が間違っている。";
        public const string ERR_MSG_0114 = "印刷キュー上限を超えたため、印刷要求を破棄した。";
        public const string ERR_MSG_0115 = "接続できない、または権限が無いプリンタ名。";

        public const string ERR_MSG_0201 = "Acrobat Readerを呼び出すクラスの作成でエラー。";
        public const string ERR_MSG_0202 = "Acrobat ReaderでのPDFファイル読み込みでエラー。";
        public const string ERR_MSG_0203 = "Acrobat Readerへの印刷指示でエラー。";
        public const string ERR_MSG_0204 = "Acrobat Readerを呼び出すクラスの削除でエラー。";

        public const string ERR_MSG_0301 = "ファイル保存時にエラーが発生。";
        public const string ERR_MSG_0302 = "印刷一時ファイル保存時にエラーが発生。";

        public const string ERR_MSG_0401 = "印刷ダイアログ表示の場合の待ちタイムアウト。";
        public const string ERR_MSG_0402 = "印刷ダイアログ表示を検知できない。";
        public const string ERR_MSG_0403 = "スプーラからの情報取得関数が失敗。";
        public const string ERR_MSG_0404 = "スプーラから応答が無い。";
        public const string ERR_MSG_0405 = "複数部数の印刷がタイムアウトまでにスプールし終わらない。";
        public const string ERR_MSG_0406 = "オフライン。";
        public const string ERR_MSG_0407 = "用紙切れ。";
        public const string ERR_MSG_0408 = "不正なドライバ。";
        public const string ERR_MSG_0409 = "プリンタ不良。";
        public const string ERR_MSG_0410 = "印刷受付タイムアウト。";
        public const string ERR_MSG_0411 = "トレイ指定時のプリンタアクセス権限不足。";
        public const string ERR_MSG_0412 = "トレイ指定時にプリンタへの書き込みに失敗。";
        public const string ERR_MSG_0413 = "複数部数出力中にエラーが発生したため、以降の処理を中止。";

        public const string ERR_MSG_0501 = "その他のエラー。";
        public const string ERR_MSG_0502 = "通信エラー。";
        public const string ERR_MSG_0503 = "HTTPプロトコルエラー。";
        public const string ERR_MSG_0504 = "HTTPメソッドエラー。";
        public const string ERR_MSG_0505 = "受信データなし。";
        public const string ERR_MSG_0506 = "JobIDに対応する印刷履歴なし。";
        public const string ERR_MSG_0507 = "印刷履歴なし。";
        public const string ERR_MSG_0508 = "ブラウザの起動に失敗しました。";
        public const string ERR_MSG_0509 = "MicrosoftEdgeの起動に失敗しました。";

        public const string ERR_MSG_ENG_0000 = "It printed successfully.";

        public const string ERR_MSG_ENG_0101 = "Unable to get spp file.";
        public const string ERR_MSG_ENG_0102 = "Unable to decompress spp file";
        public const string ERR_MSG_ENG_0103 = "The size of the spp file is invalid.";
        public const string ERR_MSG_ENG_0104 = "The print data file is not included in the spp file.";
        public const string ERR_MSG_ENG_0105 = "The spp file does not contain a print parameter file.";
        public const string ERR_MSG_ENG_0106 = "The printer name specification is incorrect.";
        public const string ERR_MSG_ENG_0107 = "Default printer not set";
        public const string ERR_MSG_ENG_0108 = "Unable to get printer handle";
        public const string ERR_MSG_ENG_0109 = "The tray specification is incorrect.";
        public const string ERR_MSG_ENG_0110 = "Wrong number of copies specified";
        public const string ERR_MSG_ENG_0111 = "Printing start / end page specified incorrectly";
        public const string ERR_MSG_ENG_0112 = "The response URL is incorrect.";
        public const string ERR_MSG_ENG_0113 = "The file save path specification is incorrect.";
        public const string ERR_MSG_ENG_0114 = "The print request has been discarded because it exceeded the print queue limit.";
        public const string ERR_MSG_ENG_0115 = "This printer can not connect or has no authority.";

        public const string ERR_MSG_ENG_0201 = "Error creating class to call Acrobat Reader.";
        public const string ERR_MSG_ENG_0202 = "Error loading PDF file with Acrobat Reader.";
        public const string ERR_MSG_ENG_0203 = "Error in print instruction to Acrobat Reader.";
        public const string ERR_MSG_ENG_0204 = "Error deleting class calling Acrobat Reader.";

        public const string ERR_MSG_ENG_0301 = "An error occurred when saving the file.";
        public const string ERR_MSG_ENG_0302 = "Error occurred when saving printing temporary file.";

        public const string ERR_MSG_ENG_0401 = "wait timeout for print dialog display";
        public const string ERR_MSG_ENG_0402 = "Print dialog display can not be detected";
        public const string ERR_MSG_ENG_0403 = "Get information from spooler failed.";
        public const string ERR_MSG_ENG_0404 = "There is no response from the spooler.";
        public const string ERR_MSG_ENG_0405 = "Printing of multiple copies does not finish spooling until timeout.";
        public const string ERR_MSG_ENG_0406 = "offline.";
        public const string ERR_MSG_ENG_0407 = "Paper out";

        public const string ERR_MSG_ENG_0408 = "Invalid driver.";
        public const string ERR_MSG_ENG_0409 = "Printer defect.";
        public const string ERR_MSG_ENG_0410 = "print admission timeout.";
        public const string ERR_MSG_ENG_0411 = "Insufficient printer access authority when specifying tray.";
        public const string ERR_MSG_ENG_0412 = "Failed to write to printer when tray specified.";
        public const string ERR_MSG_ENG_0413 = "An error occurred during output of multiple copies, and the subsequent processing is canceled.";

        public const string ERR_MSG_ENG_0501 = "Other error.";
        public const string ERR_MSG_ENG_0502 = "Communication error.";
        public const string ERR_MSG_ENG_0503 = "HTTP protocol error.";
        public const string ERR_MSG_ENG_0504 = "HTTP method error.";
        public const string ERR_MSG_ENG_0505 = "No received data";
        public const string ERR_MSG_ENG_0506 = "No print history corresponding to JobID.";
        public const string ERR_MSG_ENG_0507 = "No print history.";
        public const string ERR_MSG_ENG_0508 = "Browser startup failed";
        public const string ERR_MSG_ENG_0509 = "Failed to startup MicrosoftEdge.";


        public const string ERR_CAUSE_NONE = "";
        public const string ERR_CAUSE_DATA = "DATA";
        public const string ERR_CAUSE_LIBRARY = "LIBRARY";
        public const string ERR_CAUSE_FILE = "FILE";
        public const string ERR_CAUSE_PRINT = "PRINT";
        public const string ERR_CAUSE_OTHER = "OTHER";

        /// <summary>
        /// エラーコードからエラー種別を取得する
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string ChangeCodeToCause(int code)
        {
            string rtn = "";
            if (code == STATUS_OK)
            {
                rtn = ERR_CAUSE_NONE;
            }
            else if (code >= ERR_CODE_0101 && code <= ERR_CODE_0199)
            {
                rtn = ERR_CAUSE_DATA;
            }
            else if (code >= ERR_CODE_0201 && code <= ERR_CODE_0299)
            {
                rtn = ERR_CAUSE_LIBRARY;
            }
            else if (code >= ERR_CODE_0301 && code <= ERR_CODE_0399)
            {
                rtn = ERR_CAUSE_FILE;
            }
            else if (code >= ERR_CODE_0401 && code <= ERR_CODE_0499)
            {
                rtn = ERR_CAUSE_PRINT;
            }
            else if (code >= ERR_CODE_0501 && code <= ERR_CODE_0599)
            {
                rtn = ERR_CAUSE_OTHER;
            }
            return rtn;
        }
        /// <summary>
        /// エラーコードからエラー詳細取得
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string ChangeCodeToDetail(int code)
        {
            //特殊パターン対応。
            if (code == ERR_CODE_0501)
            {
                return SetExErrorMsg;
            }
            //読み込まれていた場合はその値を出力
            if (IsLoaded && ErrorDataDatail.ContainsKey(code))
            {
                return ErrorDataDatail[code];
            }
            //読み込まれていない場合は固定値を出力
            InitCulture();
            if (CultureCode == CULTURE_JP)
            {
                return GetJapaneseDetail(code);
            }
            else
            {
                return GetEnglishDetail(code);
            }
        }
        /// <summary>
        /// 日本語の詳細文字列を取得する
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetJapaneseDetail(int code)
        {
            string rtn = "";
            switch (code)
            {
                case STATUS_OK: rtn = ERR_MSG_0000; break;
                case ERR_CODE_0101: rtn = ERR_MSG_0101; break;
                case ERR_CODE_0102: rtn = ERR_MSG_0102; break;
                case ERR_CODE_0103: rtn = ERR_MSG_0103; break;
                case ERR_CODE_0104: rtn = ERR_MSG_0104; break;
                case ERR_CODE_0105: rtn = ERR_MSG_0105; break;
                case ERR_CODE_0106: rtn = ERR_MSG_0106; break;
                case ERR_CODE_0107: rtn = ERR_MSG_0107; break;
                case ERR_CODE_0108: rtn = ERR_MSG_0108; break;
                case ERR_CODE_0109: rtn = ERR_MSG_0109; break;
                case ERR_CODE_0110: rtn = ERR_MSG_0110; break;
                case ERR_CODE_0111: rtn = ERR_MSG_0111; break;
                case ERR_CODE_0112: rtn = ERR_MSG_0112; break;
                case ERR_CODE_0113: rtn = ERR_MSG_0113; break;
                case ERR_CODE_0114: rtn = ERR_MSG_0114; break;
                case ERR_CODE_0115: rtn = ERR_MSG_0115; break;
                case ERR_CODE_0201: rtn = ERR_MSG_0201; break;
                case ERR_CODE_0202: rtn = ERR_MSG_0202; break;
                case ERR_CODE_0203: rtn = ERR_MSG_0203; break;
                case ERR_CODE_0204: rtn = ERR_MSG_0204; break;
                case ERR_CODE_0301: rtn = ERR_MSG_0301; break;
                case ERR_CODE_0302: rtn = ERR_MSG_0302; break;
                case ERR_CODE_0401: rtn = ERR_MSG_0401; break;
                case ERR_CODE_0402: rtn = ERR_MSG_0402; break;
                case ERR_CODE_0403: rtn = ERR_MSG_0403; break;
                case ERR_CODE_0404: rtn = ERR_MSG_0404; break;
                case ERR_CODE_0405: rtn = ERR_MSG_0405; break;
                case ERR_CODE_0406: rtn = ERR_MSG_0406; break;
                case ERR_CODE_0407: rtn = ERR_MSG_0407; break;
                case ERR_CODE_0408: rtn = ERR_MSG_0408; break;
                case ERR_CODE_0409: rtn = ERR_MSG_0409; break;
                case ERR_CODE_0410: rtn = ERR_MSG_0410; break;
                case ERR_CODE_0411: rtn = ERR_MSG_0411; break;
                case ERR_CODE_0412: rtn = ERR_MSG_0412; break;
                case ERR_CODE_0413: rtn = ERR_MSG_0413; break;
                case ERR_CODE_0501: rtn = SetExErrorMsg; break;
                case ERR_CODE_0502: rtn = ERR_MSG_0502; break;
                case ERR_CODE_0503: rtn = ERR_MSG_0503; break;
                case ERR_CODE_0504: rtn = ERR_MSG_0504; break;
                case ERR_CODE_0505: rtn = ERR_MSG_0505; break;
                case ERR_CODE_0506: rtn = ERR_MSG_0506; break;
                case ERR_CODE_0507: rtn = ERR_MSG_0507; break;
                case ERR_CODE_0508: rtn = ERR_MSG_0508; break;
                case ERR_CODE_0509: rtn = ERR_MSG_0509; break;
                default: break;
            }
            return rtn;
        }
        /// <summary>
        /// 英語の詳細文字列を取得する
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetEnglishDetail(int code)
        {

            string rtn = "";
            switch (code)
            {
                case STATUS_OK: rtn = ERR_MSG_ENG_0000; break;
                case ERR_CODE_0101: rtn = ERR_MSG_ENG_0101; break;
                case ERR_CODE_0102: rtn = ERR_MSG_ENG_0102; break;
                case ERR_CODE_0103: rtn = ERR_MSG_ENG_0103; break;
                case ERR_CODE_0104: rtn = ERR_MSG_ENG_0104; break;
                case ERR_CODE_0105: rtn = ERR_MSG_ENG_0105; break;
                case ERR_CODE_0106: rtn = ERR_MSG_ENG_0106; break;
                case ERR_CODE_0107: rtn = ERR_MSG_ENG_0107; break;
                case ERR_CODE_0108: rtn = ERR_MSG_ENG_0108; break;
                case ERR_CODE_0109: rtn = ERR_MSG_ENG_0109; break;
                case ERR_CODE_0110: rtn = ERR_MSG_ENG_0110; break;
                case ERR_CODE_0111: rtn = ERR_MSG_ENG_0111; break;
                case ERR_CODE_0112: rtn = ERR_MSG_ENG_0112; break;
                case ERR_CODE_0113: rtn = ERR_MSG_ENG_0113; break;
                case ERR_CODE_0114: rtn = ERR_MSG_ENG_0114; break;
                case ERR_CODE_0115: rtn = ERR_MSG_ENG_0115; break;
                case ERR_CODE_0201: rtn = ERR_MSG_ENG_0201; break;
                case ERR_CODE_0202: rtn = ERR_MSG_ENG_0202; break;
                case ERR_CODE_0203: rtn = ERR_MSG_ENG_0203; break;
                case ERR_CODE_0204: rtn = ERR_MSG_ENG_0204; break;
                case ERR_CODE_0301: rtn = ERR_MSG_ENG_0301; break;
                case ERR_CODE_0302: rtn = ERR_MSG_ENG_0302; break;
                case ERR_CODE_0401: rtn = ERR_MSG_ENG_0401; break;
                case ERR_CODE_0402: rtn = ERR_MSG_ENG_0402; break;
                case ERR_CODE_0403: rtn = ERR_MSG_ENG_0403; break;
                case ERR_CODE_0404: rtn = ERR_MSG_ENG_0404; break;
                case ERR_CODE_0405: rtn = ERR_MSG_ENG_0405; break;
                case ERR_CODE_0406: rtn = ERR_MSG_ENG_0406; break;
                case ERR_CODE_0407: rtn = ERR_MSG_ENG_0407; break;
                case ERR_CODE_0408: rtn = ERR_MSG_ENG_0408; break;
                case ERR_CODE_0409: rtn = ERR_MSG_ENG_0409; break;
                case ERR_CODE_0410: rtn = ERR_MSG_ENG_0410; break;
                case ERR_CODE_0411: rtn = ERR_MSG_ENG_0411; break;
                case ERR_CODE_0412: rtn = ERR_MSG_ENG_0412; break;
                case ERR_CODE_0413: rtn = ERR_MSG_ENG_0413; break;
                case ERR_CODE_0501: rtn = SetExErrorMsg; break;
                case ERR_CODE_0502: rtn = ERR_MSG_ENG_0502; break;
                case ERR_CODE_0503: rtn = ERR_MSG_ENG_0503; break;
                case ERR_CODE_0504: rtn = ERR_MSG_ENG_0504; break;
                case ERR_CODE_0505: rtn = ERR_MSG_ENG_0505; break;
                case ERR_CODE_0506: rtn = ERR_MSG_ENG_0506; break;
                case ERR_CODE_0507: rtn = ERR_MSG_ENG_0507; break;
                case ERR_CODE_0508: rtn = ERR_MSG_ENG_0508; break;
                case ERR_CODE_0509: rtn = ERR_MSG_ENG_0509; break;
                default: break;
            }
            return rtn;

        }
        /// <summary>
        /// エクセプションが発生した際にそのメッセージを保持する
        /// </summary>
        public static string SetExErrorMsg { set; get; } = ERR_MSG_0501;
    }
}
