using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;
using System.Printing;

namespace BizPrintCommon
{
    /// <summary>
    /// 印刷パラメータ保持・解析クラス
    /// </summary>
    public class PrintParameter
    {
        /// <summary>jobID</summary>
        public string JobID { get; set; } = string.Empty;
        /// <summary>プリンタ名</summary>
        public string PrinterName { get; private set; } = string.Empty;
        /// <summary>印刷部数</summary>
        public int NumberOfCopy { get; private set; } = 1;
        /// <summary>出力トレイ</summary>
        public string SelectedTray { get; private set; } = string.Empty;
        /// <summary>出力トレイを数値で表したもの</summary>
        public int SelectedTrayNum { get; private set; } = (int)CommonConstants.DMBIN_NONE;
        /// <summary>印刷識別子</summary>
        public string JobName { get; private set; } = string.Empty;
        /// <summary>開始ページ番号</summary>
        public int FromPage { get; private set; } = 0;
        /// <summary>終了ページ番号</summary>
        public int ToPage { get; private set; } = -1;
        /// <summary>PDFコンテンツ</summary>
        public byte[] PdfDocumentByte { get; set; } = null;
        public string PdfFileName { get; set; }

        /// <summary> 印刷ダイアログ表示 </summary>
        public bool IsPrintDialog { get; private set; } = false;

        /// <summary>印刷応答URL</summary>
        public string ResponseURL { get; private set; } = string.Empty;

        /// <summary>ファイル保存パス</summary>
        public string SaveFileName { get; private set; } = string.Empty;

        /// <summary>ブラウザターゲット</summary>
        public string TargetFrame { get; private set; } = string.Empty;

        /// <summary>用紙サイズに合わせて印刷</summary>
        public bool DoFit { get; private set; } = false;


        /// <summary>要求受信時刻</summary>
        public DateTime RequestedTime { get; private set; }

        /// <summary>SilentPdf起動元ブラウザのプロセス名</summary>
        public string BrowserProcessname { get; set; } = "";


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PrintParameter()
        {
            //newされたタイミングを受付時刻とする
            RequestedTime = DateTime.Now;
        }

        /// <summary>
        /// バイトデータからファイルとして全データを読み込み、パラメータをセットする
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public int ReadParamFile(byte[] fileData)
        {
            int rtn = 0;
            StreamReader sr = null;
            try
            {
                //全ての行を読み込んで設定値をセット
                MemoryStream memParam = new MemoryStream(fileData);
                sr = new StreamReader(memParam);
                string stResult = string.Empty;
                int paramchk = 0;
                while (sr.Peek() > 0)
                {

                    paramchk = SetParamLine(sr.ReadLine());
                    if (paramchk != 0)
                    {
                        rtn = paramchk;
                    }
                }
                //パラメーターとしてプリンタ名が設定されていない場合はデフォルトチェック
                if (rtn == 0 && PrinterName == String.Empty) {
                    if (!IsDefaultPrinterSetted())
                    {
                        LogUtility.OutputLog("054", "printerName", "");
                        rtn= ErrCodeAndmErrMsg.ERR_CODE_0107;
                    }
                }
            }
            catch (Exception ex)
            {
                string errstr = ex.Message;
                LogUtility.OutputLog("053", ex.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }


            LogUtility.OutputLog("052", Encoding.UTF8.GetString(fileData));
            return rtn;
        }
        /// <summary>
        /// param.txtファイルの内容を解析してパラメタをセットする処理の1行分
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public int SetParamLine(string param)
        {
            string[] splitted = param.Split('=');
            if (splitted.Length < 2)
            {
                //指定なしなのでデフォルト値を使用。エラーではない
                return 0;
            }
            if (splitted[1].Length == 0)
            {
                //指定なしなのでデフォルト値を使用。エラーではない
                return 0;
            }
            switch (splitted[0])
            {
                case CommonConstants.PRINT_PARAM_PRINTERNAME:
                    if (splitted[1].Length > 0 && !IsInstalledPrinterName(splitted[1]))
                    {
                        //そのマシンのプリンタとしてとして正しくない文字列
                        LogUtility.OutputLog("054", splitted[0], splitted[1]);
                        return ErrCodeAndmErrMsg.ERR_CODE_0106;
                    }
                    if (splitted[1].Length > 0 && !IsSafePrinterName(splitted[1]))
                    {
                        //マシンにインストールされているが有効ではないプリンタ
                        LogUtility.OutputLog("054", splitted[0], splitted[1]);
                        return ErrCodeAndmErrMsg.ERR_CODE_0115;
                    }
                    //プリンタが指定されていない、かつ、マシンにデフォルトプリンタが存在しない
                    if (splitted[1].Length == 0)
                    {
                        if (!IsDefaultPrinterSetted())
                        {
                            LogUtility.OutputLog("054", splitted[0], splitted[1]);
                            return ErrCodeAndmErrMsg.ERR_CODE_0107;
                        }
                    }
                    //プリンタ切り替えを行う場合はデフォルトプリンタが有効かチェック
                    if (splitted[1].Length > 0 )
                    {
                        //現在のプリンタを取得
                        String defaultPrinter = "";
                        DefaultPrinterGetter dpg = new DefaultPrinterGetter();
                        if (dpg.getDefaultPrinter())
                        {
                            defaultPrinter = dpg.PrinterName;
                        }

                        //デフォルトプリンタが有効ではないプリンタ
                        if (defaultPrinter.Length== 0 ||!IsSafePrinterName(defaultPrinter)) {
                            LogUtility.OutputLog("054", splitted[0], defaultPrinter);
                            return ErrCodeAndmErrMsg.ERR_CODE_0115;
                        }
                    }


                    PrinterName = splitted[1];
                    break;
                case CommonConstants.PRINT_PARAM_NUM_OF_COPY:
                    NumberOfCopy = StringToIntWithDefault(splitted[1], NumberOfCopy);
                    //送信側でチェック済みなので、ない筈
                    if (NumberOfCopy == 0)
                    {
                        LogUtility.OutputLog("054", splitted[0], splitted[1]);
                        return ErrCodeAndmErrMsg.ERR_CODE_0110;
                    }
                    break;
                case CommonConstants.PRINT_PARAM_SELECTED_TRAY:
                    if (!IsSafeTrayName(splitted[1]))
                    {
                        //トレイ名として正しくない文字列
                        LogUtility.OutputLog("054", splitted[0], splitted[1]);
                        return ErrCodeAndmErrMsg.ERR_CODE_0109;
                    }
                    SelectedTray = splitted[1];
                    //変換できるのは確認済みなので、数値化
                    SelectedTrayNum = SelectedTrayNum = ChangeTrayNameToNum(splitted[1]);
                    break;
                case CommonConstants.PRINT_PARAM_JOBNAME:
                    JobName = splitted[1];
                    break;
                case CommonConstants.PRINT_PARAM_DOFIT:
                    if (String.Compare(splitted[1], "true", true) == 0)
                    {
                        DoFit = true;
                    }
                    break;
                case CommonConstants.PRINT_PARAM_RESP_URL:
                    if (splitted[1].Length > 0 && !IsSafeURLString(splitted[1]))
                    {
                        //URLとして正しくない文字列
                        LogUtility.OutputLog("054", splitted[0], splitted[1]);
                        return ErrCodeAndmErrMsg.ERR_CODE_0112;
                    }
                    ResponseURL = splitted[1];
                    break;
                case CommonConstants.PRINT_PARAM_SAVE_FILENAME:
                    if (splitted[1].Length > 0 && !IsSafeWindowsPath(splitted[1]))
                    {
                        //Pathとして正しくない文字列
                        LogUtility.OutputLog("054", splitted[0], splitted[1]);
                        return ErrCodeAndmErrMsg.ERR_CODE_0113;
                    }
                    SaveFileName = splitted[1];
                    //フォルダ区切りが/だった場合は\\に置換しておく
                    SaveFileName = SaveFileName.Replace("/", "\\");
                    break;
                case CommonConstants.PRINT_PARAM_TARGET:
                    TargetFrame = splitted[1];
                    break;
                case CommonConstants.PRINT_PARAM_PRINT_DLG:
                    if (String.Compare(splitted[1], "true", true) == 0)
                    {
                        IsPrintDialog = true;
                    }
                    break;
                case CommonConstants.PRINT_PARAM_FROMPAGE:
                    FromPage = StringToIntWithDefault(splitted[1], FromPage);
                    break;
                case CommonConstants.PRINT_PARAM_TOPAGE:
                    ToPage = StringToIntWithDefault(splitted[1], ToPage);
                    break;
            }

            return 0;
        }
        /// <summary>
        /// 引数文字列を数値に変換。変換できない物はデフォルト値を採用
        /// </summary>
        /// <param name="str">引数文字列</param>
        /// <param name="def">デフォルト値</param>
        /// <returns>設定値</returns>
        private static int StringToIntWithDefault(string str, int def)
        {
            int rtn = def;
            try
            {
                rtn = int.Parse(str);
            }
            catch (Exception ex)
            {
                //デフォルト値になる

                LogUtility.OutputLog("040", str, def.ToString(), ex.Message);
            }


            return rtn;
        }

        /// <summary>
        /// マシンにインストールされているプリンタ名に一致するものがあるかチェック
        /// </summary>
        /// <param name="name">指定されたプリンタ名</param>
        /// <returns>true:OK false:NG</returns>
        private bool IsInstalledPrinterName(string name)
        {
            bool rtn = false;
            string printerNames = "";
            foreach (string s in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                printerNames += "\r\n" + s;
                if (name.Equals(s))
                {
                    rtn = true;
                }

            }
            LogUtility.OutputLog("216", printerNames);
            return rtn;
        }
        /// <summary>
        /// 指定されたプリンタ名が有効かを判定する
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns>ture:有効 false:無効</returns>
        private bool IsSafePrinterName(string printerName)
        {
            bool rtn = true;
            PrintQueue SpoolerQue = null;
            try
            {
                int type = CommonConstants.chkPrinterType(printerName);
                if (type == CommonConstants.PRINTER_TYPE_UNC)
                {
                    Uri urichk = new Uri(printerName);
                    string ServerName = urichk.Host;
                    string PrinterName = urichk.AbsolutePath;
                    SpoolerQue = new PrintQueue(new PrintServer("\\\\" + ServerName), printerName);
                }
                else
                {
                    SpoolerQue = new PrintQueue(new PrintServer(), printerName);
                }
            }
            catch (Exception ex)
            {
                LogUtility.OutputLog("212", printerName, ex.Message);
                rtn = false;
            }
            if (SpoolerQue == null) {
                rtn = false;
            }
            return rtn;
        }
        /// <summary>
        /// 実行マシンにデフォルトプリンタが設定されているかをSpoolerQueの取得でチェック
        /// </summary>
        /// <returns>true:OK false:NG</returns>
        public static bool IsDefaultPrinterSetted()
        {
            bool rtn = true;
            String defaultName = "";
            DefaultPrinterGetter dpg = new DefaultPrinterGetter();
            if (dpg.getDefaultPrinter())
            {
                defaultName = dpg.PrinterName;
            }
            else {
                return false;
            }
            PrintQueue SpoolerQue = null;
            try
            {
                int type = CommonConstants.chkPrinterType(defaultName);
                if (type == CommonConstants.PRINTER_TYPE_UNC)
                {
                    Uri urichk = new Uri(defaultName);
                    string ServerName = urichk.Host;
                    string PrinterName = urichk.AbsolutePath;
                    SpoolerQue = new PrintQueue(new PrintServer("\\\\" + ServerName), defaultName);
                }
                else
                {
                    SpoolerQue = new PrintQueue(new PrintServer(), defaultName);
                }
            }
            catch (Exception ex)
            {
                rtn = false;
            }
            if (SpoolerQue == null)
            {
                rtn = false;
            }
            return rtn;
        }
        /// <summary>
        /// URLとして正しい文字列かチェック
        /// </summary>
        /// <param name="url"></param>
        /// <returns>true:OK false:NG</returns>
        public static bool IsSafeURLString(string url)
        {
            bool rtn = false;
            if ((url == null) || (url.Length <= 0))
            {
                return rtn;
            }
            //正規表現でURLとして正しいかチェック
            return System.Text.RegularExpressions.Regex.IsMatch(url, @"\As?https?://[-_.!~*'()a-zA-Z0-9;/?:@&=+$,%#]+\z");

        }
        /// <summary>
        /// ファイル保存先として指定された文字列が、Windowsのパスとして正しい形式かチェック
        /// フォルダの存在確認などはしない
        /// </summary>
        /// <param name="path">true:OK false:NG</param>
        /// <returns></returns>
        public static bool IsSafeWindowsPath(string path)
        {
            bool rtn = false;
            if ((path == null) || (path.Length <= 0))
            {
                return rtn;
            }
            //"/"を\\に置き換え
            string chk = path.Replace("/", "\\");
            //除外文字列
            string invChars = new string(System.IO.Path.GetInvalidFileNameChars());
            //区切り文字列
            string sepChar = System.Text.RegularExpressions.Regex.Escape(System.IO.Path.DirectorySeparatorChar.ToString());
            //正規表現でPathとして正しいかチェック
            return System.Text.RegularExpressions.Regex.IsMatch(chk, string.Format(@"\A(?:[a-z]:|{1})(?:{1}[^{0}]+)+\z", invChars, sepChar), System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        }
        /// <summary>
        /// トレイ名から数値への変換が可能な文字列かチェック
        /// </summary>
        /// <param name="tray"></param>
        /// <returns>true:OK false:NG</returns>
        public bool IsSafeTrayName(string tray)
        {
            //既にプリンタ名がセットされていたら、そのプリンタの持ってるトレイ名に一致するかチェック
            return (0 == IsPrinterHasTrayName(PrinterName, tray));
        }
        /// <summary>
        /// トレイ変換処理
        /// </summary>
        /// <param name="strTray">トレイ文字列</param>
        /// <returns>トレイ数値</returns>
        public static int ChangeTrayNameToNum(string strTray)
        {
            long lTray = CommonConstants.DMBIN_NONE;
            //すべて大文字で比較
            string upperd = strTray.ToUpper();
            switch (upperd)
            {
                case CommonConstants.FIRST:
                    lTray = CommonConstants.DMBIN_FIRST;
                    break;
                case CommonConstants.UPPER:
                    lTray = CommonConstants.DMBIN_UPPER;
                    break;
                case CommonConstants.ONLYONE:
                    lTray = CommonConstants.DMBIN_ONLYONE;
                    break;
                case CommonConstants.LOWER:
                    lTray = CommonConstants.DMBIN_LOWER;
                    break;
                case CommonConstants.MIDDLE:
                    lTray = CommonConstants.DMBIN_MIDDLE;
                    break;
                case CommonConstants.MANUAL:
                    lTray = CommonConstants.DMBIN_MANUAL;
                    break;
                case CommonConstants.ENVELOPE:
                    lTray = CommonConstants.DMBIN_ENVELOPE;
                    break;
                case CommonConstants.ENVMANUAL:
                    lTray = CommonConstants.DMBIN_ENVMANUAL;
                    break;
                case CommonConstants.AUTO:
                    lTray = CommonConstants.DMBIN_AUTO;
                    break;
                case CommonConstants.TRACTOR:
                    lTray = CommonConstants.DMBIN_TRACTOR;
                    break;
                case CommonConstants.SMALLFMT:
                    lTray = CommonConstants.DMBIN_SMALLFMT;
                    break;
                case CommonConstants.LARGEFMT:
                    lTray = CommonConstants.DMBIN_LARGEFMT;
                    break;
                case CommonConstants.LARGECAPACITY:
                    lTray = CommonConstants.DMBIN_LARGECAPACITY;
                    break;
                case CommonConstants.CASETTE:
                    lTray = CommonConstants.DMBIN_CASETTE;
                    break;
                case CommonConstants.FORMSOURCE:
                    lTray = CommonConstants.DMBIN_FORMSOURCE;
                    break;
                case CommonConstants.LAST:
                    lTray = CommonConstants.DMBIN_LAST;
                    break;
                case CommonConstants.CUSTOM:
                    lTray = CommonConstants.DMBIN_CUSTOM;
                    break;
                case "":
                    lTray = CommonConstants.DMBIN_AUTO;
                    break;
                default:

                    break;
            }
            return (int)lTray;
        }

        /// <summary>
        /// 指定したプリンタに指定したTrayNameがあるかをチェック
        /// </summary>
        /// <param name="PrinterName"></param>
        /// <param name="TrayName"></param>
        /// <returns>0:OK -1:NG</returns>
        public static int IsPrinterHasTrayName(string PrinterName, string TrayName)
        {
            int rtn = -1;
            //トレイ名が指定されていないか、AUTOは無条件でOK
            if (TrayName == null || TrayName.Length == 0 || ChangeTrayNameToNum(TrayName) == CommonConstants.DMBIN_AUTO)
            {
                return 0;
            }
            int trayNum = ChangeTrayNameToNum(TrayName);
            //プリンタ名が指定されていない場合は取得したデフォルトプリンタを使う。未指定で取れなければNGで返す

            if (PrinterName == null || PrinterName.Length == 0)
            {
                DefaultPrinterGetter dpg = new DefaultPrinterGetter();
                if (!dpg.getDefaultPrinter())
                {
                    return -1;
                }
            }

            //前段でチェックしてるので、デフォルトプリンタが取れない事は無い
            System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();

            if (PrinterName == null || PrinterName.Length == 0)
            {

            }
            else
            {
                pd.PrinterSettings.PrinterName = PrinterName;
            }
            foreach (PaperSource ps in pd.PrinterSettings.PaperSources)
            {
                LogUtility.OutputDebugLog("E122", PrinterName, ps.Kind.ToString());
                if ((int)ps.Kind == trayNum)
                {
                    rtn = 0;
                }
                else if ((trayNum == CommonConstants.DMBIN_CUSTOM) && (int)ps.Kind >= CommonConstants.DMBIN_CUSTOM)
                {
                    //指定がCUSTOMの場合、KindがCustomのものは256以上の任意の値なので
                    rtn = 0;

                }
            }


            return rtn;
        }

    }
}
