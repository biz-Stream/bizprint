using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BizPrintCommon
{
    /// <summary>
    /// Acrobatが表示する印刷ダイアログの監視を行うクラス
    /// </summary>
    public class AcrobatPrintDialogMonitor
    {
        /// <summary>
        /// 印刷ダイアログの名前
        /// </summary>
        public string DialogName { set; get; }
        /// <summary>
        ///一度でも発見されたかのフラグ
        /// </summary>
        public bool IsDlgOpend { private set; get; } = false;
        /// <summary>
        ///現在ダイアログが表示中かのフラグ 
        /// </summary>
        public bool IsDlgFoundNow { private set; get; } = false;

        /// <summary>
        ///ダイアログのクローズログ出力完了フラグ
        /// </summary>
        private bool IsCloseLogOut  = false;

        /// <summary>
        ///  主ウィンドウのハンドルリスト
        /// </summary>
        /// <typeparam name="IntPtr"></typeparam>
        /// <param name=""></param>
        /// <returns></returns>
        List<IntPtr> HandleList;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="DlgName">印刷ダイアログ名</param>
        public AcrobatPrintDialogMonitor(string DlgName)
        {
            DialogName = DlgName;
            HandleList = new List<IntPtr>();
            LogUtility.OutputLog("111");
        }
        /// <summary>
        /// 印刷ダイアログが閉じられたかを検知する
        /// </summary>
        /// <returns></returns>
        public bool IsPrintDlgClosed()
        {
            bool isNow = IsPrintDialogNow();
            //未発見時はfalse
            if (!IsDlgOpend)
            {
                return false;

            }
            else
            {
                //1回発見後、今表示中ならfalse
                if (isNow)
                {
                    return false;
                }
                else
                {
                    if (!IsCloseLogOut) { 
                        LogUtility.OutputLog("112");
                        IsCloseLogOut = true;
                    }
                    //一度発見してから閉じたなら終了検知
                    return true;
                }
            }
        }

        #region インポート定義

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool EnumWindowsProcDelegate(IntPtr windowHandle, IntPtr lParam);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(
             [MarshalAs(UnmanagedType.FunctionPtr)] EnumWindowsProcDelegate enumProc,
             IntPtr lParam);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(
        IntPtr handle,
        [MarshalAs(UnmanagedType.FunctionPtr)] EnumWindowsProcDelegate enumProc,
        IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd,
        StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd,
            StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(
       IntPtr hWnd, out int lpdwProcessId);
        #endregion

        /// <summary>
        /// 現在、自プロセスから立ち上がった、コンストラクタで指定された名称のダイアログが存在するかをチェック
        /// </summary>
        /// <returns></returns>
        public bool IsPrintDialogNow()
        {
            //このターン分の初期化
            IsDlgFoundNow = false;
            HandleList.Clear();

            //メインウィンドウのハンドルを全取得
            EnumWindows(EnumWindowProc, default(IntPtr));
            //その子ウィンドウをすべてチェック
            foreach (IntPtr hMain in HandleList)
            {
                EnumChildWindows(hMain, EnumChiledProc, default(IntPtr));
            }
            return IsDlgFoundNow;
        }
        /// <summary>
        /// すべてのメインウィンドウに対する処理関数
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        internal bool EnumWindowProc(IntPtr handle, IntPtr lParam)
        {
            HandleList.Add(handle);
            int textLength = GetWindowTextLength(handle);
            if (0 < textLength)
            {
                //ウィンドウのタイトルを取得する
                StringBuilder TempStrBuilder = new StringBuilder(textLength + 1);
                GetWindowText(handle, TempStrBuilder, TempStrBuilder.Capacity);
                string title = TempStrBuilder.ToString();
                if (System.Text.RegularExpressions.Regex.IsMatch(title, DialogName))
                {
                    //プロセスIDから親プロセスのIDを探索
                    int processID = 0;
                    try
                    {
                        GetWindowThreadProcessId(handle, out processID);
                        int thisID = System.Diagnostics.Process.GetCurrentProcess().Id;
                        if (thisID == processID)
                        {
                            if (!IsDlgOpend)
                            {
                                LogUtility.OutputDebugLog("E152");
                                IsDlgOpend = true;
                            }
                            IsDlgFoundNow = true;
                            return true;
                        }
                        //自身と一致しないなら一段上を探す
                        uint parentID = GetParentProcessID(processID);
                        //動かしてる側とプロセスIDも一致したら、このプロセスから起動したAcrobatの印刷ダイアログ
                        if (parentID == (uint)thisID)
                        {
                            if (!IsDlgOpend)
                            {
                                LogUtility.OutputDebugLog("E152");
                                IsDlgOpend = true;
                            }
                            IsDlgFoundNow = true;
                            return true;
                        }
                        //もう一段回上でもチェック
                        uint parentOfParentID = GetParentProcessID((int)parentID);
                        if (parentOfParentID == (uint)thisID)
                        {
                            if (!IsDlgOpend)
                            {
                                LogUtility.OutputDebugLog("E153");
                                IsDlgOpend = true;
                            }
                            IsDlgFoundNow = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtility.OutputLog("503", "EnumWindowProc processID=" + processID, ex.Message);
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// すべての子ダイアログに対する関数
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        internal bool EnumChiledProc(IntPtr handle, IntPtr lParam)
        {
            int textLength = GetWindowTextLength(handle);
            if (0 < textLength)
            {
                //ウィンドウのタイトルを取得する
                StringBuilder tsb = new StringBuilder(textLength + 1);
                GetWindowText(handle, tsb, tsb.Capacity);
                string title = tsb.ToString();
                if (System.Text.RegularExpressions.Regex.IsMatch(title, DialogName))
                {
                    int processID = 0;
                    try
                    {
                        GetWindowThreadProcessId(handle, out processID);
                        //プロセスIDから親プロセスのIDを探索
                        int thisID = System.Diagnostics.Process.GetCurrentProcess().Id;
                        if (thisID == processID)
                        {
                            if (!IsDlgOpend)
                            {
                                LogUtility.OutputDebugLog("E152");
                                IsDlgOpend = true;
                            }
                            IsDlgFoundNow = true;
                            return true;
                        }

                        //一つ上はAcrobat32.exeなので、さらにその上
                        uint parentID = GetParentProcessID(processID);
                        if (parentID == (uint)thisID)
                        {
                            if (!IsDlgOpend)
                            {
                                LogUtility.OutputDebugLog("E154");
                                IsDlgOpend = true;
                            }
                            IsDlgFoundNow = true;
                            return true;
                        }

                        uint parentOfParentID = GetParentProcessID((int)parentID);
                        //動かしてる側とプロセスIDも一致したら、このプロセスから起動したAcrobatの印刷ダイアログ
                        //もう一段回上でもチェック
                        if (parentOfParentID == (uint)thisID)
                        {
                            if (!IsDlgOpend)
                            {
                                LogUtility.OutputDebugLog("E155");
                                IsDlgOpend = true;
                            }
                            IsDlgFoundNow = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtility.OutputLog("503", "EnumChiledProc processID=" + processID, ex.Message);
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 親プロセスのIDを取得する関数
        /// </summary>
        /// <param name="MyProcId"></param>
        /// <returns></returns>
        private uint GetParentProcessID(int MyProcId)
        {
            //var myProcId = System.Diagnostics.Process.GetCurrentProcess().Id;
            var queryString = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", MyProcId);

            using (var searcher = new System.Management.ManagementObjectSearcher(@"root\CIMV2", queryString))
            //クエリから結果を取得
            using (var results = searcher.Get().GetEnumerator())
            {

                if (!results.MoveNext()) throw new ApplicationException("Couldn't Get ParrentProcessId.");

                var QueryResult = results.Current;
                //親プロセスのPIDを取得
                return (uint)QueryResult["ParentProcessId"];
            }

        }
    }
}
