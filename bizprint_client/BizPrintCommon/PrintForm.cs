using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BizPrintCommon
{
    /// <summary>
    /// 印刷実行フォーム
    /// </summary>
    public partial class PrintForm : Form
    {

        public SettingManeger SettingMng { set; get; }
        /// <summary>印刷パラメータ</summary>
        private PrintParameter PrintParam { set; get; } = null;

        /// <summary>デフォルトプリンタ名</summary>
        private string DefaultPrinterName { set; get; } = String.Empty;
        /// <summary>デフォルトトレイ名称</summary>
        public string DefaultTrayName { private set; get; } = String.Empty;
        /// <summary>デフォルトトレイ識別番号</summary>
        public int DefaultTrayNum { private set; get; } = (int)CommonConstants.DMBIN_AUTO;//AUTO
        /// <summary>デフォルト印刷部数</summary>
        public int DefaultNumOfCopies { private set; get; } = 1;

        /// <summary>印刷対象プリンタの変更フラグ</summary>
        private bool IsPrinterDefault = true;
        /// <summary>印刷対象トレイの変更フラグ</summary>
        private bool IsTrayDefault = true;
        /// <summary>印刷部数の変更フラグ</summary>
        private bool IsNumCopiesDefault = true;
        /// <summary>
        /// 印刷一時ファイル
        /// </summary>
        TmpPDFFile TmpPrintFile;
        /// <summary>エラーメッセージ</summary>
        public string LastErrMsg { private set; get; } = String.Empty;
        /// <summary>エラー識別子</summary>
        public int LastHResult { private set; get; } = ErrCodeAndmErrMsg.STATUS_OK;
        /// <summary>印刷結果として返すステータス</summary>
        public int PrintStatusReturn { private set; get; } = ErrCodeAndmErrMsg.STATUS_OK;
        /// <summary></summary>
        bool IsDlgOverTimeMsgOut = false;

        /// <summary>タイマー呼び出し回数カウンター</summary>
        private int NumTimerCalled = 0;
        /// <summary>印刷要求がプリンタに到達したかを監視するクラス</summary>
        private PrintRequestMonitor PrintReqMOnitor = null;
        /// <summary>印刷ダイアログ表示時に表示中チェックを行うクラス</summary>
        private AcrobatPrintDialogMonitor AcrobatDlgMonitor = null;
        /// <summary>印刷要求送信済みチェック結果</summary>
        private bool IsPrintReqSend = false;
        /// <summary>印刷ダイアログ発見時のカウント数を保持する</summary>
        private int IsDlgFoundCount = 0;
        /// <summary>印刷開始時刻</summary>
        private DateTime startPrintTime;

        /// <summary>トレイ変更の成功フラグ</summary>
        private bool IsTrayChangeSuccessed = false;
        /// <summary>部数変更の成功フラグ</summary>
        private bool IsCopiesChangeSuccessed = false;
        /// <summary>
        /// 自身の処理から閉じる際にtrueにする
        /// </summary>
        private Boolean closeOwnFlg = false;

        /// <summary>自身の親スレッド </summary>
        private PrintReqProcesser parentTH = null;
        public void setParentTH(PrintReqProcesser parent) {
            parentTH = parent;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="param">印刷パラメータ</param>
        /// <param name="mng">設定マネージャ</param>
        public PrintForm(PrintParameter param, SettingManeger mng)
        {
            PrintParam = param;
            SettingMng = mng;

            InitializeComponent();
            NumTimerCalled = 0;
            GetDefaultSetting();
        }
        /// <summary>
        /// タイマコンポーネントの外部開始関数
        /// </summary>
        public void StartTimer()
        {
            this.TimerPrtForm.Enabled = true;
            LogUtility.OutputLog("061", PrintParam.JobID);
        }
        /// <summary>
        /// パラメタ指定により、ファイルセーブを行う
        /// </summary>
        /// <returns>ErrCodeAndmErrMsg.STATUS_OK:成功　それ以外：失敗</returns>
        public int SavePdfFile()
        {
            if (PrintParam == null)
            {
                LogUtility.OutputDebugLog("E308");
                return ErrCodeAndmErrMsg.ERR_CODE_0301;
            }
            if (!PrintParam.SaveFileName.Equals(""))
            {
                LogUtility.OutputLog("059", PrintParam.SaveFileName);
                try
                {
                    //ファイル名、拡張子、パス名取得(Windowsのパスとして正しい事はパラメタ読み込み時にチェック済み)
                    string saveDir = Path.GetDirectoryName(PrintParam.SaveFileName);
                    string fileName = Path.GetFileNameWithoutExtension(PrintParam.SaveFileName);
                    string fileExt = Path.GetExtension(PrintParam.SaveFileName);

                    //保存先フォルダパス存在確認
                    if (!Directory.Exists(saveDir))
                    {
                        //フォルダが無ければ作成
                        Directory.CreateDirectory(saveDir);
                    }
                    //ファイル保存
                    //既に同名ファイルがあるかチェック
                    string checkFileName = saveDir + "\\" + fileName + fileExt;
                    if (File.Exists(checkFileName))
                    {
                        //ある場合、拡張子の前に(n)をカウントアップしていき、独自にする
                        int plus = 1;
                        while (File.Exists(checkFileName))
                        {
                            checkFileName = saveDir + "\\" + fileName + "(" + plus + ")" + fileExt;
                            plus++;
                        }
                    }
                    //保存実行
                    FileStream newFile = new FileStream(checkFileName, FileMode.Create);
                    BinaryWriter binWriter = new BinaryWriter(newFile);
                    binWriter.Write(PrintParam.PdfDocumentByte);
                    binWriter.Close();
                    newFile.Close();

                }
                catch (Exception ex)
                {
                    //log060 保存時のエラー
                    LogUtility.OutputLog("060", PrintParam.JobID, ex.Message);
                    LastErrMsg = ex.Message;
                    LastHResult = ex.HResult;
                    ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                    //保存時エラーに集約
                    return ErrCodeAndmErrMsg.ERR_CODE_0301;
                }
            }
            return ErrCodeAndmErrMsg.STATUS_OK;
        }
        /// <summary>
        /// ダイアログ無しの印刷処理
        /// </summary>
        /// <returns>0：成功、それ以外：エラーコード</returns>
        public int PrintPDFNoDialog()
        {
            AxAcroPDFLib.AxAcroPDF AxPdfOcx;
            int rtn = ErrCodeAndmErrMsg.STATUS_OK;
            if (PrintParam == null)
            {
                LogUtility.OutputDebugLog("E601");
                return -1;
            }
            //一時ファイル作成
            TmpPrintFile = new TmpPDFFile();
            if (!TmpPrintFile.CreateTmpFile(SettingMng.TmpFolderPath, PrintParam.JobID, PrintParam.PdfDocumentByte))
            {
                //一時ファイル作成失敗でdoresponce
                LogUtility.OutputDebugLog("E302", "PrintPDFNoDialog");
                PrintHistoryManager.UpdatePrintStatusByID(PrintParam.JobID, CommonConstants.JOB_STATUS_ERROR_FINISH);

                return ErrCodeAndmErrMsg.ERR_CODE_0302;
            }
            //コンポーネントの初期化
            try
            {
                AxPdfOcx = this.pdfOcx;
                AxPdfOcx.Visible = false;

            }
            catch (Exception ex)
            {
                LastErrMsg = ex.Message;
                LastHResult = ex.HResult;
                TmpPrintFile.DeleteTmpFile();
                LogUtility.OutputLog("087", "PrintPDFNoDialog");
                PrintHistoryManager.UpdatePrintStatusByID(PrintParam.JobID, CommonConstants.JOB_STATUS_ERROR_FINISH);
                return ErrCodeAndmErrMsg.ERR_CODE_0201;
            }

            bool loadFlg = false;
            //ファイルロードリトライループ
            for (int i = 0; i < SettingMng.LoadRetryNum + 1; i++)
            {
                bool exceptionFlg = false;
                if (i != 0)
                {
                    Thread.Sleep(SettingMng.LoadRetryWaitMsec);
                }
#if DEBUG
                if (i == 0 && TmpPrintFile.PrintFilePath.IndexOf("_0002") > 0)
                {
                    TmpPrintFile.DeleteTmpFile();
                }
#endif
                //ファイル存在チェック
                if (!File.Exists(TmpPrintFile.PrintFilePath))
                {
                    LogUtility.OutputLog("401", TmpPrintFile.PrintFilePath);
                    PrintHistoryManager.UpdatePrintStatusByID(PrintParam.JobID, CommonConstants.JOB_STATUS_ERROR_FINISH);
                    return ErrCodeAndmErrMsg.ERR_CODE_0302;
                }
                else
                {
                    LogUtility.OutputLog("402", TmpPrintFile.PrintFilePath);
                }
                //ファイルロード
                try
                {
                    loadFlg = AxPdfOcx.LoadFile(TmpPrintFile.PrintFilePath);
#if DEBUG
                    if (i == 0 && TmpPrintFile.PrintFilePath.IndexOf("_0004") > 0)
                    {
                        loadFlg = false;
                    }
                    else if (i == 0 && TmpPrintFile.PrintFilePath.IndexOf("_0006") > 0)
                    {
                        throw new Exception("TestDebug");
                    }
                    else if (TmpPrintFile.PrintFilePath.IndexOf("_0005") > 0)
                    {
                        loadFlg = false;
                    }

#endif
                }
                catch (Exception ex)
                {
                    LogUtility.OutputLog("403", TmpPrintFile.PrintFilePath, ex.Message);
                    LastErrMsg = ex.Message;
                    LastHResult = ex.HResult;
                    loadFlg = false;
                    exceptionFlg = true;
                }

                if (loadFlg)
                {
                    //成功
                    LogUtility.OutputLog("406", TmpPrintFile.PrintFilePath);
                    break;
                }
                else if (exceptionFlg)
                {
                    //例外による失敗
                }
                else
                {
                    //load関数の失敗
                    LogUtility.OutputLog("404", TmpPrintFile.PrintFilePath);
                }
                if (i < SettingMng.LoadRetryNum)
                {
                    int num = SettingMng.LoadRetryNum - i;
                    LogUtility.OutputLog("405", TmpPrintFile.PrintFilePath, SettingMng.LoadRetryWaitMsec.ToString(), num.ToString(), SettingMng.LoadRetryNum.ToString());
                }
            }
            //リトライ回数までチャレンジしたがロードできなかった場合
            if (!loadFlg)
            {
                LogUtility.OutputLog("407", SettingMng.LoadRetryNum.ToString());
                LogUtility.OutputLog("089", TmpPrintFile.PrintFilePath);
                TmpPrintFile.DeleteTmpFile();
                PrintHistoryManager.UpdatePrintStatusByID(PrintParam.JobID, CommonConstants.JOB_STATUS_ERROR_FINISH);
                return ErrCodeAndmErrMsg.ERR_CODE_0202;
            }

            //loadに成功したので、印刷ファイル名をロードしたファイル名にUpdate
            PrintHistoryManager.UpdatePrintFileNameById(PrintParam.JobID, TmpPrintFile.PrintFileNameWithExt);
            LogUtility.OutputLog("088", TmpPrintFile.PrintFilePath);
            try
            {
                LogUtility.OutputLog("093", PrintParam.FromPage.ToString() + "," + PrintParam.ToPage.ToString(), PrintParam.DoFit.ToString());

                //ページ範囲指定があるならそれに従う
                if (PrintParam.FromPage != 0 || PrintParam.ToPage != -1)
                {
                    //サイズを合わせて印刷
                    if (PrintParam.DoFit)
                    {
                        LogUtility.OutputDebugLog("E001");
                        AxPdfOcx.printPagesFit(PrintParam.FromPage, PrintParam.ToPage, true);
                    }
                    else
                    {
                        LogUtility.OutputDebugLog("E002");
                        AxPdfOcx.printPages(PrintParam.FromPage, PrintParam.ToPage);
                    }
                }
                //すべて印刷
                else
                {
                    //サイズを合わせて印刷
                    if (PrintParam.DoFit)
                    {
                        LogUtility.OutputDebugLog("E003");
                        AxPdfOcx.printAllFit(true);
                    }
                    else
                    {
                        LogUtility.OutputDebugLog("E004");
                        AxPdfOcx.printAll();
                    }
                }

            }
            catch (Exception ex)
            {
                LastErrMsg = ex.Message;
                LastHResult = ex.HResult;
                LogUtility.OutputLog("095", LastErrMsg);
                PrintHistoryManager.UpdatePrintStatusByID(PrintParam.JobID, CommonConstants.JOB_STATUS_ERROR_FINISH);
                rtn = ErrCodeAndmErrMsg.ERR_CODE_0203;
            }
            finally
            {

            }

            //結果を返す
            return rtn;
        }

        /// <summary>
        /// 印刷ダイアログ表示ありの印刷処理
        /// </summary>
        /// <returns>0：成功、それ以外：エラーコード</returns>
        public int PrintPDFWithDialog()
        {
            int rtn = ErrCodeAndmErrMsg.STATUS_OK;

            if (PrintParam == null)
            {
                return -1;
            }
            //一時ファイル作成
            TmpPrintFile = new TmpPDFFile();
            if (!TmpPrintFile.CreateTmpFile(SettingMng.TmpFolderPath, PrintParam.JobID, PrintParam.PdfDocumentByte))
            {
                //一時ファイル作成失敗でdoresponce
                PrintHistoryManager.UpdatePrintStatusByID(PrintParam.JobID, CommonConstants.JOB_STATUS_ERROR_FINISH);
                LogUtility.OutputDebugLog("E302");
                return ErrCodeAndmErrMsg.ERR_CODE_0302;
            }
            //コンポーネントの初期化
            AxAcroPDFLib.AxAcroPDF AxPdfOcx;
            try
            {
                AxPdfOcx = this.pdfOcx;
                AxPdfOcx.Visible = false;

            }
            catch (Exception ex)
            {
                LastErrMsg = ex.Message;
                LastHResult = ex.HResult;
                LogUtility.OutputLog("087", "PrintPDFWithDialog");
                LogUtility.OutputDebugLog("E201", "PrintPDFWithDialog", ex.Message);
                PrintHistoryManager.UpdatePrintStatusByID(PrintParam.JobID, CommonConstants.JOB_STATUS_ERROR_FINISH);
                return ErrCodeAndmErrMsg.ERR_CODE_0201;
            }
            bool loadFlg = false;
            //ファイルロードリトライループ
            for (int i = 0; i < SettingMng.LoadRetryNum + 1; i++)
            {
                bool exceptionFlg = false;
                if (i != 0)
                {
                    Thread.Sleep(SettingMng.LoadRetryWaitMsec);
                }
#if DEBUG
                if (i == 0 && TmpPrintFile.PrintFilePath.IndexOf("_0002") > 0)
                {
                    TmpPrintFile.DeleteTmpFile();
                }
#endif
                //ファイル存在チェック
                if (!File.Exists(TmpPrintFile.PrintFilePath))
                {
                    LogUtility.OutputLog("401", TmpPrintFile.PrintFilePath);
                    PrintHistoryManager.UpdatePrintStatusByID(PrintParam.JobID, CommonConstants.JOB_STATUS_ERROR_FINISH);
                    return ErrCodeAndmErrMsg.ERR_CODE_0302;
                }
                else
                {
                    LogUtility.OutputLog("402", TmpPrintFile.PrintFilePath);
                }
                //ファイルロード
                try
                {
                    loadFlg = AxPdfOcx.LoadFile(TmpPrintFile.PrintFilePath);
#if DEBUG
                    if (i == 0 && TmpPrintFile.PrintFilePath.IndexOf("_0004") > 0)
                    {
                        loadFlg = false;
                    }
                    else if (i == 0 && TmpPrintFile.PrintFilePath.IndexOf("_0006") > 0)
                    {
                        throw new Exception("TestDebug");
                    }
                    else if (TmpPrintFile.PrintFilePath.IndexOf("_0005") > 0)
                    {
                        loadFlg = false;
                    }

#endif
                }
                catch (Exception ex)
                {
                    LogUtility.OutputLog("403", TmpPrintFile.PrintFilePath, ex.Message);
                    LastErrMsg = ex.Message;
                    LastHResult = ex.HResult;
                    loadFlg = false;
                    exceptionFlg = true;
                }

                if (loadFlg)
                {
                    //成功
                    LogUtility.OutputLog("406", TmpPrintFile.PrintFilePath);
                    break;
                }
                else if (exceptionFlg)
                {
                    //例外による失敗
                }
                else
                {
                    //load関数の失敗
                    LogUtility.OutputLog("404", TmpPrintFile.PrintFilePath);
                }
                if (i < SettingMng.LoadRetryNum)
                {
                    int num = SettingMng.LoadRetryNum - i;
                    LogUtility.OutputLog("405", TmpPrintFile.PrintFilePath, SettingMng.LoadRetryWaitMsec.ToString(), num.ToString(), SettingMng.LoadRetryNum.ToString());
                }
            }
            //リトライ回数までチャレンジしたがロードできなかった場合
            if (!loadFlg)
            {
                LogUtility.OutputLog("407", SettingMng.LoadRetryNum.ToString());
                LogUtility.OutputLog("089", TmpPrintFile.PrintFilePath);
                TmpPrintFile.DeleteTmpFile();
                PrintHistoryManager.UpdatePrintStatusByID(PrintParam.JobID, CommonConstants.JOB_STATUS_ERROR_FINISH);
                return ErrCodeAndmErrMsg.ERR_CODE_0202;
            }

            //loadに成功したので、印刷ファイル名をロードしたファイル名にUpdate
            PrintHistoryManager.UpdatePrintFileNameById(PrintParam.JobID, TmpPrintFile.PrintFileNameWithExt);
            //印刷ダイアログは監視されている
            try
            {
                //印刷ダイアログ表示
                AxPdfOcx.printWithDialog();
            }
            catch (Exception ex)
            {
                LogUtility.OutputLog("095", LastErrMsg);
                LastErrMsg = ex.Message;
                LastHResult = ex.HResult;
                PrintHistoryManager.UpdatePrintStatusByID(PrintParam.JobID, CommonConstants.JOB_STATUS_ERROR_FINISH);
                return ErrCodeAndmErrMsg.ERR_CODE_0203;
            }

            return rtn;
        }
        /// <summary>
        /// 現在設定されているプリンタ・トレイを取得し、設定から読み取ったものと違っていた場合に設定する
        /// </summary>
        /// <returns></returns>
        private int SetPrinterAndTray()
        {
            if (PrintParam == null)
            {
                return -1;
            }
            bool isRegWin = chkLegacyDefaultPrinterMode();//レジストリに該当の項目があって、かつ0ならtrue
            string usePrinterName;
            //プリンタが指定されていて、かつ「Windowsで通常使うプリンタを管理するをON」になってるなら必ずセット
            if (!PrintParam.PrinterName.Equals("") && isRegWin)
            {
                IsPrinterDefault = true;//書き戻ししない
                SetDefaultPrinterByName(PrintParam.PrinterName);
                usePrinterName = PrintParam.PrinterName;
            }
            //プリンタが指定されていて、かつ、デフォルトプリンタと違う場合、セット
            else if (!PrintParam.PrinterName.Equals("") && !PrintParam.PrinterName.Equals(DefaultPrinterName))
            {
                IsPrinterDefault = false;
                SetDefaultPrinterByName(PrintParam.PrinterName);
                usePrinterName = PrintParam.PrinterName;
            }
            else
            {
                usePrinterName = DefaultPrinterName;
                if (PrintParam.PrinterName.Equals(""))
                {
                    LogUtility.OutputLog("065", DefaultPrinterName);
                }
            }
            //プリンタがデフォルトではない場合は、トレイがどうなるか不明なので、デフォルトから変わってるのと同じ扱い
            //トレイが指定されていて、かつデフォルトトレイと違う場合セット
            if (!PrintParam.SelectedTray.Equals("") && PrintParam.SelectedTrayNum != DefaultTrayNum)
            {
                IsTrayDefault = false;
                SetDefaultTrayByNo(usePrinterName, PrintParam.SelectedTrayNum, PrintParam.SelectedTray);
            }
            else if (isRegWin && !PrintParam.SelectedTray.Equals(""))
            {
                IsTrayDefault = false;
                SetDefaultTrayByNo(usePrinterName, PrintParam.SelectedTrayNum, PrintParam.SelectedTray);
            }
            if (PrintParam.SelectedTray.Equals(""))
            {
                LogUtility.OutputLog("091", DefaultTrayName);
            }
            if (PrintParam.NumberOfCopy != DefaultNumOfCopies && !PrintParam.IsPrintDialog)
            {
                SetDefaultCopiesNo(usePrinterName, PrintParam.NumberOfCopy);
                IsNumCopiesDefault = false;
            }

            //印刷履歴のプリンタ名を更新する
            PrintHistoryManager.UpdatePrinterNameById(PrintParam.JobID, usePrinterName);

            return ErrCodeAndmErrMsg.STATUS_OK;
        }
        /// <summary>
        /// 印刷終了後、プリンタ・トレイが書き換えられていたら元に戻す
        /// </summary>
        /// <returns></returns>
        public int ResetPrinterAndTray()
        {
            if (PrintParam == null)
            {
                return -1;
            }
            //トレイを先に戻す
            if (!IsTrayDefault && IsTrayChangeSuccessed)
            {
                SetDefaultTrayByNo("", DefaultTrayNum, DefaultTrayName);
            }
            //部数を戻す
            if (!IsNumCopiesDefault && IsCopiesChangeSuccessed)
            {
                SetDefaultCopiesNo("", DefaultNumOfCopies);
            }
            //プリンタをデフォルトに戻す
            if (!IsPrinterDefault)
            {
                SetDefaultPrinterByName(DefaultPrinterName);
            }
            return ErrCodeAndmErrMsg.STATUS_OK;
        }
        /// <summary>
        /// デフォルトプリンタとデフォルトトレイとデフォルト印刷部数の取得
        /// </summary>
        /// <returns></returns>
        public void GetDefaultSetting()
        {

            //現在のプリンタを取得(印刷パラメタチェック後なので、ここで時間が掛る事は無い)
            System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
            DefaultPrinterName = pd.PrinterSettings.PrinterName;
#if DEBUG
            //取得方法によって現在のデフォルトプリンタ名に差があるかをチェック
            LocalPrintServer lPrintServer = new LocalPrintServer();
            String lpDefaultPrinterName = lPrintServer.DefaultPrintQueue.FullName;
            LogUtility.OutputLog("905", "DefaultPrinterName=" + DefaultPrinterName + ",lpFullName" + lpDefaultPrinterName);
#endif
            bool isRegWin = chkLegacyDefaultPrinterMode();//レジストリに該当の項目があって、かつ0ならtrue

            //プリンタが未指定ならデフォルトのデフォルトトレイ名と番号を取得
            if (PrintParam.PrinterName.Length == 0)
            {
                //現在のトレイを取得
                DefaultTrayName = pd.PrinterSettings.DefaultPageSettings.PaperSource.Kind.ToString();
                //Customの場合、257以上の値が返ってくることがある。
                DefaultTrayNum = pd.PrinterSettings.DefaultPageSettings.PaperSource.RawKind;
                //現在の印刷部数を取得
                DefaultNumOfCopies = pd.PrinterSettings.Copies;
            }
            //LegacyDefaultPrinterModeがfalseで、指定されたのとデフォルトが同じなら
            else if (!isRegWin && DefaultPrinterName.Equals(PrintParam.PrinterName))
            {
                //現在のトレイを取得
                DefaultTrayName = pd.PrinterSettings.DefaultPageSettings.PaperSource.Kind.ToString();
                //Customの場合、257以上の値が返ってくることがある。
                DefaultTrayNum = pd.PrinterSettings.DefaultPageSettings.PaperSource.RawKind;
                //現在の印刷部数を取得
                DefaultNumOfCopies = pd.PrinterSettings.Copies;
            }
            else//(isRegWin || !DefaultPrinterName.Equals(PrintParam.PrinterName))
            {
                //プリンターを使用するものに切り替え(プリンターがある事はチェック済み)
                pd.PrinterSettings.PrinterName = PrintParam.PrinterName;
                //変更先プリンタの現在のトレイを取得
                DefaultTrayName = pd.PrinterSettings.DefaultPageSettings.PaperSource.Kind.ToString();
                //Customの場合、257以上の値が返ってくることがある。
                DefaultTrayNum = pd.PrinterSettings.DefaultPageSettings.PaperSource.RawKind;
                //変更先プリンタの現在の印刷部数を取得
                DefaultNumOfCopies = pd.PrinterSettings.Copies;
            }

            return;
        }
        /// <summary>
        /// デフォルトプリンタの設定
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        ///

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int LoadLibraryA([MarshalAs(UnmanagedType.LPStr)] string DllName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int FreeLibrary(int hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetProcAddress(int hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetDesktopWindow();

        public delegate void PrintUIEntryW(Int32 hwnd, Int32 hinst, [MarshalAs(UnmanagedType.LPWStr)] string text, Int32 nCmdShow);
        public static int SetDefaultPrinterByName(string printerName)
        {
            try
            {
                int hModule = LoadLibraryA("printui.dll");
#if DEBUG
//                hModule = LoadLibraryA("printuiAAA.dll");//DebugA
#endif
                if (hModule == 0) {
                    LogUtility.OutputLog("213", "LoadLibrary");
                    LogUtility.OutputLog("172", printerName, "LoadLibrary");
                    return -1;
                }
                IntPtr ptr;
                ptr = (IntPtr)GetProcAddress(hModule, "PrintUIEntryW");
#if DEBUG
//                ptr = (IntPtr)GetProcAddress(hModule, "AAAPrintUIEntry");//DebugB
#endif
                if (ptr != IntPtr.Zero)
                {
                    PrintUIEntryW func1 = (PrintUIEntryW)Marshal.GetDelegateForFunctionPointer(ptr, typeof(PrintUIEntryW));
                    func1(GetDesktopWindow(), hModule, "/q /y /n \"" + printerName + "\"", 5);
                }
                else
                {
                    LogUtility.OutputLog("213", "GetProcAddress");
                    LogUtility.OutputLog("172", printerName, "GetProcAddress");
                    return -1;
                }
                FreeLibrary(hModule);
                // 本当に切り替わったのかチェック
                DefaultPrinterGetter dpg = new DefaultPrinterGetter();
                String nowPrinter="";
                if (dpg.getDefaultPrinter())
                {
                    nowPrinter = dpg.PrinterName;
                }

#if DEBUG
//                printerName = "ErrorOnDebug";//DebugC
#endif
                //現在のプリンタを取得
                if (!nowPrinter.Equals(printerName))
                {
                    // 切り替えに失敗している場合
                    LogUtility.OutputLog("172", printerName, "DefaultPrinter=" + nowPrinter);

                    return -1;
                }

                LogUtility.OutputLog("171", printerName);

#if DEBUG
//                throw new Exception("DebugD");
#endif
                return 0;
            }
            catch (Exception ex)
            {
                //例外で失敗
                LogUtility.OutputLog("172", printerName, ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// デフォルトトレイの設定
        /// </summary>
        /// <param name="printerName">プリンタ名</param>
        /// <param name="TrayNum">トレイ名</param>
        public void SetDefaultTrayByNo(string printerName, int TrayNum, string trayName)
        {
            PrinterSetting prtSet = new PrinterSetting();
            int rtn = ErrCodeAndmErrMsg.STATUS_OK;
            //プリンタ名が指定されていない場合、現在のデフォルトプリンタのトレイを書き換える
            if (printerName.Length == 0)
            {
                DefaultPrinterGetter dpg = new DefaultPrinterGetter();
                String nowPrinter = "";
                if (dpg.getDefaultPrinter())
                {
                    nowPrinter = dpg.PrinterName;
                    rtn = prtSet.ChangeDefaultTray(nowPrinter, TrayNum, trayName);
                }

            }
            else
            {
                //指定されているのでその名前を使う
                rtn = prtSet.ChangeDefaultTray(printerName, TrayNum, trayName);
            }
            if (rtn != ErrCodeAndmErrMsg.STATUS_OK)
            {
                //失敗したので、現在の設定のまま印刷するWARNING
                LogUtility.OutputLog("153", printerName, TrayNum.ToString() + ":" + trayName);
                //設定権限が無いので、印刷終了後はデフォルト書き戻ししない
                IsTrayChangeSuccessed = false;
            }
            else
            {
                IsTrayChangeSuccessed = true;
            }

        }
        /// <summary>
        /// 印刷部数のデフォルト値を書き換える
        /// </summary>
        /// <param name="printerName"></param>
        /// <param name="numCopies"></param>
        public void SetDefaultCopiesNo(string printerName, int numCopies)
        {
            PrinterSetting prtSet = new PrinterSetting();
            int rtn = ErrCodeAndmErrMsg.STATUS_OK;
            //プリンタ名が指定されていない場合、現在のデフォルトプリンタのトレイを書き換える
            if (printerName.Length == 0)
            {
                DefaultPrinterGetter dpg = new DefaultPrinterGetter();
                String nowPrinter = "";
                if (dpg.getDefaultPrinter())
                {
                    nowPrinter = dpg.PrinterName;
                }

                //System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
                rtn = prtSet.ChangeDefaultCopies(nowPrinter, numCopies);
            }
            else
            {
                //指定されているのでその名前を使う
                rtn = prtSet.ChangeDefaultCopies(printerName, numCopies);
            }
            if (rtn != ErrCodeAndmErrMsg.STATUS_OK)
            {
                //失敗したので、現在の設定のまま印刷するWARNING
                LogUtility.OutputLog("170", printerName, numCopies.ToString());
                //設定権限が無いので、印刷終了後はデフォルト書き戻ししない
                IsCopiesChangeSuccessed = false;
            }
            else
            {
                IsCopiesChangeSuccessed = true;
            }

        }

        /// <summary>
        /// タイマにより呼び出される、このフォームでのループ処理。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerPrtForm_Tick(object sender, EventArgs e)
        {
            //タイマを一旦停止
            this.TimerPrtForm.Enabled = false;
            //処理中は、最新イベント時刻をアップデートし続ける
            SettingManeger.UpdateLatestEvent();
            //印刷ダイアログ表示の場合、ダイアログ表示されてるかを監視し、ダイアログがなくなったら自身を閉じる
            if (PrintParam.IsPrintDialog)
            {
                //ダイアログ表示監視処理初期化
                if (NumTimerCalled == 0)
                {
                    IsDlgOverTimeMsgOut = false;
                    AcrobatDlgMonitor = new AcrobatPrintDialogMonitor(SettingMng.AcrobatPrintDialogName);
                    IsPrintReqSend = false;
                }
                else if (NumTimerCalled == 1)
                {
                    //ダイアログ表示を行う場合
                    startPrintTime = DateTime.Now;
                    //プリンタ・トレイの設定変更
                    SetPrinterAndTray();
                    //印刷実行
                    PrintStatusReturn = PrintPDFWithDialog();
                }
                else
                {
                    if (!IsPrintReqSend)
                    {
                        if (AcrobatDlgMonitor.IsPrintDlgClosed() == false)
                        {
                            //印刷ダイアログを1回発見後、閉じるまでこの処理で待機
                        }
                        else
                        {
                            IsDlgFoundCount = NumTimerCalled;
                            IsPrintReqSend = true;
                        }
                    }
                    if (IsPrintReqSend && (IsDlgFoundCount + SettingMng.PrintDialogLeaveTimeOut * 2 < NumTimerCalled))
                    {

                        LogUtility.OutputLog("121");
                        TmpPrintFile.DeleteTmpFile();
                        //デフォルトプリンタの書き戻し
                        ResetPrinterAndTray();
                        PrintHistoryManager.UpdatePrintStatusByID(PrintParam.JobID, CommonConstants.JOB_STATUS_SUCCESS_FINISH);
                        //クローズされたので、このフォームを閉じる
                        closeOwnFlg = true;
                        this.Close();
                        return;
                    }
                    else if (PrintStatusReturn == ErrCodeAndmErrMsg.ERR_CODE_0202)
                    {
                        // ロードリトライ上限エラーの場合の処理
                        this.TimerPrtForm.Enabled = false;

                        //一時ファイルの削除
                        TmpPrintFile.DeleteTmpFile();
                        //デフォルトプリンタの書き戻し
                        ResetPrinterAndTray();
                        //自身のクローズ
                        closeOwnFlg = true;
                        this.Close();
                        return;
                    }
                    //タイムアウトチェックまでダイアログが発見されなかった場合の処理
                    TimeSpan timeDiffer = DateTime.Now - startPrintTime;
                    if (timeDiffer.TotalMilliseconds > SettingMng.PrintDialogFindTimeOut)
                    {
                        //印刷ダイアログ表示を検知できないままのタイムアウト 402
                        if (!AcrobatDlgMonitor.IsDlgOpend)
                        {
                            this.TimerPrtForm.Enabled = false;
                            PrintStatusReturn = ErrCodeAndmErrMsg.ERR_CODE_0402;
                            LogUtility.OutputLog("109", SettingMng.PrintDialogFindTimeOut.ToString());
                            TmpPrintFile.DeleteTmpFile();
                            //デフォルトプリンタの書き戻し
                            ResetPrinterAndTray();
                            PrintHistoryManager.UpdatePrintStatusByID(PrintParam.JobID, CommonConstants.JOB_STATUS_TIMEOUT);
                            closeOwnFlg = true;
                            this.Close();
                            return;
                        }
                        else
                        {
                            //印刷ダイアログ表示をしたままの場合はタイムアウトしない
                        }
                    }
                    //印刷ダイアログ操作が一定時間されない場合の処理
                    if (timeDiffer.TotalMilliseconds > SettingMng.PrintDialogStayingTimeCheck && !IsDlgOverTimeMsgOut && (AcrobatDlgMonitor.IsPrintDlgClosed() == false))
                    {
                        if (AcrobatDlgMonitor.IsDlgOpend)
                        {
                            //印刷ダイアログ表示をしたままPrintDialogStayingTimeCheck過ぎた場合1度だけメッセージログを出す。
                            if (!IsDlgOverTimeMsgOut)
                            {
                                LogUtility.OutputLog("110", SettingMng.PrintDialogStayingTimeCheck.ToString());
                                IsDlgOverTimeMsgOut = true;
                            }

                        }
                    }

                }
            }
            else
            {
                //印刷ダイアログを表示しない場合、プリンタイベント監視
                if (NumTimerCalled == 0)
                {
                    //イベント監視のセット
                    if (PrintReqMOnitor == null)
                    {
                        string prtName = PrintParam.PrinterName;
                        if (prtName.Length == 0)
                        {
                            prtName = DefaultPrinterName;
                        }
                        PrintReqMOnitor = new PrintRequestMonitor(prtName, SettingMng);
#if DEBUG
                        if (PrintParam.JobID.IndexOf("_0001") >=  0 || PrintParam.JobID.IndexOf("_0003") >= 0) {
                            PrintReqMOnitor.doSleep = true;
                        }
#endif
                        PrintReqMOnitor.Start();
                    }
                    //デフォルトプリンタ・トレイの置き換え
                }
                else if (NumTimerCalled == 1)
                {
                    startPrintTime = DateTime.Now;
                    //プリンタ・トレイの設定変更
                    SetPrinterAndTray();
                    //印刷実行
                    PrintStatusReturn = PrintPDFNoDialog();
                }
                else
                {
                    //チェックして、イベントがプリンタに渡っているのを感知したらそこからカウント開始
                    if (PrintReqMOnitor != null)
                    {
                        if (!IsPrintReqSend && PrintReqMOnitor.IsJobSetted)
                        {
                            IsPrintReqSend = true;
                            PrintReqMOnitor.Stop();
                            IsDlgFoundCount = NumTimerCalled;
                        }
                    }
                    //自身のClose
                    if ((IsPrintReqSend == true) && (IsDlgFoundCount < NumTimerCalled))
                    {
                        this.TimerPrtForm.Enabled = false;
                        //デフォルトプリンタの書き戻し
                        ResetPrinterAndTray();
                        TmpPrintFile.DeleteTmpFile();

                        //自身のクローズ
                        closeOwnFlg = true;
                        this.Close();
                        return;
                    }
                    //タイムアウトチェックまでイベントが検知されなかった場合の処理
                    TimeSpan timeDiffer = DateTime.Now - startPrintTime;
                    //                    if (IsPrintReqSend==false && ( NumTimerCalled > 5) && timeDiffer.TotalMilliseconds > SettingMng.SpoolCheckTimeOut)
                    if (IsPrintReqSend == false && (((NumTimerCalled > 5) && PrintReqMOnitor.IsCallbackCalled && !PrintReqMOnitor.IsJobSetted) || (timeDiffer.TotalMilliseconds > SettingMng.SpoolCheckTimeOut * 2)))
                    {
                        this.TimerPrtForm.Enabled = false;
                        PrintStatusReturn = ErrCodeAndmErrMsg.ERR_CODE_0405;
                        //デフォルトプリンタの書き戻し
                        ResetPrinterAndTray();
                        //監視を止める
                        PrintReqMOnitor.Stop();
                        TmpPrintFile.DeleteTmpFile();
                        LogUtility.OutputLog("099", PrintParam.JobID, ErrCodeAndmErrMsg.ERR_CODE_0405.ToString());

                        //自身のクローズ
                        closeOwnFlg = true;
                        this.Close();
                        return;
                    }
                    else if (PrintStatusReturn == ErrCodeAndmErrMsg.ERR_CODE_0202)
                    {
                        // ロードリトライ上限エラーの場合の処理
                        this.TimerPrtForm.Enabled = false;

                        //デフォルトプリンタの書き戻し
                        ResetPrinterAndTray();
                        //監視を止める
                        PrintReqMOnitor.Stop();
                        TmpPrintFile.DeleteTmpFile();
                        //自身のクローズ
                        closeOwnFlg = true;
                        this.Close();
                        return;
                    }
                }
            }

            NumTimerCalled++;
            this.TimerPrtForm.Enabled = true;
        }

        private void PrintForm_Activated(object sender, EventArgs e)
        {
            Rectangle r = GetTotalBound();
            this.Location = new Point(r.X - this.Size.Width - 1, r.Y - this.Size.Height - 1);
#if DEBUG
            LogUtility.OutputLog("901", "PF X0,Y0=" + r.X.ToString() + "," + r.Y.ToString() + " this.Location=" + this.Location.X + "," + this.Location.Y);
#endif

        }
        private void setPrintEnd() {
            if (parentTH != null)
            {
                parentTH.setIsDoPrinting(false);
            }
        }
        /// <summary>
        /// 全てのスクリーンの描画領域を内包する最大領域を取得します。
        /// </summary>
        /// <returns>最大領域</returns>
        public static Rectangle GetTotalBound()
        {
            int x, y, w, h;
            int minX = 0, maxX = 0, minY = 0, maxY = 0;
            foreach (Screen s in Screen.AllScreens)
            {
                x = s.Bounds.X;
                y = s.Bounds.Y;
                w = s.Bounds.X + s.Bounds.Width;
                h = s.Bounds.Y + s.Bounds.Height;
                if (x < minX)
                    minX = x;
                if (y < minY)
                    minY = y;
                if (maxX < w)
                    maxX = w;
                if (maxY < h)
                    maxY = h;
            }
            Rectangle r = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            return r;
        }

        private void PrintForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!closeOwnFlg)
            {
                e.Cancel = true;
            }
            else { 
                setPrintEnd();
            }
        }

        //「Windows で通常使うプリンターを管理する」のレジストリキー
        const string REG_LASTPRINTER_USE = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Windows";
        const string REG_LASTPRINTER_VALUE = "LegacyDefaultPrinterMode";

        /// <summary>
        /// レジストリから「Windows で通常使うプリンターを管理する」の設定を取得する
        /// </summary>
        /// <returns>true:有効 false:設定無し、または、無効</returns>
        public static bool chkLegacyDefaultPrinterMode()
        {
            bool rtn = false;
            int regValue = (int)Microsoft.Win32.Registry.GetValue(REG_LASTPRINTER_USE, REG_LASTPRINTER_VALUE, -1);
            if (regValue == 0)
            {
                rtn = true;
            }
#if DEBUG
            LogUtility.OutputLog("906", " LegacyDefaultPrinterMode = " + rtn);
#endif

            return rtn;
        }
    }
}
