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
using System.Runtime.InteropServices;
using System.Globalization;
using System.Printing;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace BizPrintCommon
{
    /// <summary>
    /// 印刷要求がプリンタに到達したことを検知するクラス
    /// </summary>
    public class PrintRequestMonitor
    {

        #region Win32API DLL Import Functions
        [DllImport("winspool.drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter(String pPrinterName,
            out IntPtr phPrinter,
            Int32 pDefault);


        [DllImport("winspool.drv", EntryPoint = "ClosePrinter",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter
        (Int32 hPrinter);

        [DllImport("winspool.drv", EntryPoint = "FindFirstPrinterChangeNotification", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr FindFirstPrinterChangeNotification
                            ([InAttribute()] IntPtr hPrinter,
                            [InAttribute()] Int32 fwFlags,
                            [InAttribute()] Int32 fwOptions,
                            [InAttribute(), MarshalAs(UnmanagedType.LPStruct)] PRINTER_NOTIFY_OPTIONS pPrinterNotifyOptions);

        [DllImport("winspool.drv", EntryPoint = "FindNextPrinterChangeNotification", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern bool FindNextPrinterChangeNotification
                            ([InAttribute()] IntPtr hChangeObject,
                             [OutAttribute()] out Int32 pdwChange,
                             [InAttribute(), MarshalAs(UnmanagedType.LPStruct)] PRINTER_NOTIFY_OPTIONS pPrinterNotifyOptions,
                            [OutAttribute()] out IntPtr lppPrinterNotifyInfo
                                 );

        [DllImport("winspool.drv", EntryPoint = "FindClosePrinterChangeNotification", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]

        public static extern bool FindClosePrinterChangeNotification([InAttribute()] IntPtr hChangeObject);
        #endregion

        const int PRINTER_NOTIFY_OPTIONS_REFRESH = 1;
        //public event PrintJobStatusChanged OnJobStatusChange;
        #region private variables
        private IntPtr PrinterHandle = IntPtr.Zero;
        private string SpoolerName = "";
        private ManualResetEvent MrEvent = new ManualResetEvent(false);
        private RegisteredWaitHandle WaitHandle = null;
        private IntPtr ChangeHandle = IntPtr.Zero;
        private PRINTER_NOTIFY_OPTIONS NotifyOptions = new PRINTER_NOTIFY_OPTIONS();
        private Dictionary<int, string> ObjJobDictionary = new Dictionary<int, string>();
        private PrintQueue SpoolerQue = null;
        /// <summary>ジョブ投入済みかの判定</summary>
        public bool IsJobSetted { private set; get; } = false;
        /// <summary>コールバック関数が呼ばれたかの判定</summary>
        public bool IsCallbackCalled { private set; get; } = false;
        public int LastError { private set; get; } = ErrCodeAndmErrMsg.STATUS_OK;
        public string LastErrorMsg { private set; get; } = "";
        private bool isStarted = false;
        public SettingManeger SettingMng { set; get; }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="strSpoolName">スプーラ名称</param>
        public PrintRequestMonitor(string strSpoolName, SettingManeger Mng)
        {
            // Let us open the printer and get the printer handle.
            SpoolerName = strSpoolName;
            SettingMng = Mng;

        }


        #region destructor
        /// <summary>
        /// デストラクタ
        /// </summary>
        ~PrintRequestMonitor()
        {
            Stop();
        }
        #endregion
        #region StartMonitoring
        /// <summary>
        /// 監視開始
        /// </summary>
        public void Start()
        {
            string ServerName = "";
            string PrinterName = "";
            bool isUnc = false;
            int type = CommonConstants.chkPrinterType(SpoolerName);
            if (type == CommonConstants.PRINTER_TYPE_UNC)
            {
                //既に判定済みだが一応try/catch
                try
                {
                    Uri urichk = new Uri(SpoolerName);
                    if (urichk.IsUnc)
                    {
                        ServerName = urichk.Host;
                        PrinterName = urichk.AbsolutePath;
                        isUnc = true;
                    }

                }
                catch (Exception e)
                {
                    //ありえない
                    LogUtility.OutputLog("501", "PrintRequestMonitor::chkisUnc()", e.Message);
                }
            }


            try
            {

                LogUtility.OutputLog("138", SpoolerName);
                isStarted = true;
                bool chk = OpenPrinter(SpoolerName, out PrinterHandle, 0);
                if (!chk)
                {
                    int lastError = Marshal.GetLastWin32Error();
                    LogUtility.OutputLog("501", "PrintRequestMonitor::OpenPrinter()", lastError.ToString() + " " + PrinterSetting.getErrMessageFromCode(lastError));
                }
                if (PrinterHandle != IntPtr.Zero)
                {
                    //イベント取得対象のセット
                    ChangeHandle = FindFirstPrinterChangeNotification(PrinterHandle, (int)PRINTER_CHANGES.PRINTER_CHANGE_JOB, 0, NotifyOptions);
                    // We have successfully registered for change notification.  Let us capture the handle...
                    MrEvent.SafeWaitHandle = new SafeWaitHandle(ChangeHandle, true);
                    //Now, let us wait for change notification from the printer queue....
                    WaitHandle = ThreadPool.RegisterWaitForSingleObject(MrEvent, new WaitOrTimerCallback(PrinterNotifyWaitCallback), MrEvent, SettingMng.SpoolCheckTimeOut, true);
                }
                if (isUnc)
                {
                    LogUtility.OutputLog("173", ServerName, SpoolerName);
                    SpoolerQue = new PrintQueue(new PrintServer("\\\\" + ServerName), SpoolerName);
                }
                else
                {
                    LogUtility.OutputLog("174", SpoolerName);
                    SpoolerQue = new PrintQueue(new PrintServer(), SpoolerName);
                }
                foreach (PrintSystemJobInfo psi in SpoolerQue.GetPrintJobInfoCollection())
                {
                    ObjJobDictionary[psi.JobIdentifier] = psi.Name;
                }
            }
            catch (Exception e)
            {
                int lastError = Marshal.GetLastWin32Error();
                LogUtility.OutputLog("501","PrintRequestMonitor::Start()", e.Message + lastError.ToString() + " " + PrinterSetting.getErrMessageFromCode(lastError));
            }
        }
        #endregion

#if DEBUG
        public bool doSleep = false;
#endif
        #region Callback Function
        /// <summary>
        /// イベントを取得し、その内容をチェックするコールバック関数
        /// </summary>
        /// <param name="state"></param>
        /// <param name="timedOut"></param>
        public void PrinterNotifyWaitCallback(Object state, bool timedOut)
        {
            LogUtility.OutputLog("139");
            //IsCallbackCalled = true;


            //タイムアウト終了
            if (timedOut)
            {
                LogUtility.OutputLog("470");
                IsJobSetted = false;
                IsCallbackCalled = true;
                return;
            }
            if (PrinterHandle == IntPtr.Zero)
            {
                LogUtility.OutputLog("140");
                //プリンタClose済み
                LastError = ErrCodeAndmErrMsg.ERR_CODE_0403;
                IsCallbackCalled = true;
                return;
            }
            #region read notification details
            NotifyOptions.Count = 1;
            int pdwChange = 0;
            IntPtr pNotifyInfo = IntPtr.Zero;
            bool bResult = FindNextPrinterChangeNotification(ChangeHandle, out pdwChange, NotifyOptions, out pNotifyInfo);
            //If the Printer Change Notification Call did not give data, exit code
            if (bResult == false)
            {
                LogUtility.OutputLog("141");
                //取得に失敗
                LastError = ErrCodeAndmErrMsg.ERR_CODE_0403;
                IsCallbackCalled = true;
                return;
            }
            else
            {
                LogUtility.OutputLog("175", getPrinterMsg((uint)pdwChange));
            }

            //If the Change Notification was not relgated to job, exit code
            //削除イベントは無視する
            bool IsJobRelatedChange = ((pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_ADD_JOB) == PRINTER_CHANGES.PRINTER_CHANGE_ADD_JOB) ||
                                     ((pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_SET_JOB) == PRINTER_CHANGES.PRINTER_CHANGE_SET_JOB) ||
                                     ((pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_WRITE_JOB) == PRINTER_CHANGES.PRINTER_CHANGE_WRITE_JOB);
#if DEBUG

            LogUtility.OutputLog("990-DEBUG");
            if (doSleep)
            {
                LogUtility.OutputLog("991-StartSleep");
                Thread.Sleep(1000);
                LogUtility.OutputLog("992-EndSleep");
            }
#endif
            if (!IsJobRelatedChange)
            {
                //Do nothing
                LogUtility.OutputLog("176", getPrinterMsg((uint)pdwChange));
            }
            else
            {
                LogUtility.OutputLog("096", getPrinterMsg((uint)pdwChange));

                IsJobSetted = true;
            }
            #endregion

            #region reset the Event and wait for the next event
            //if (!IsJobSetted)
            //{
            //    LogUtility.OutputLog("142");
            //    try
            //    {
            //        MrEvent.Reset();
            //        WaitHandle = ThreadPool.RegisterWaitForSingleObject(MrEvent, new WaitOrTimerCallback(PrinterNotifyWaitCallback), MrEvent, SettingMng.SpoolCheckTimeOut, true);
            //    }
            //    catch (Exception ex)
            //    {
            //        LogUtility.OutputLog("143", ex.Message);

            //        LastError = ErrCodeAndmErrMsg.ERR_CODE_0501;
            //        LastErrorMsg = ex.Message;
            //        ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
            //    }
            //}
            #endregion
            IsCallbackCalled = true;
        }
        #endregion


        #region StopMonitoring
        /// <summary>
        /// 監視停止
        /// </summary>
        public void Stop()
        {
            if (!isStarted)
            {
                return;
            }
            LogUtility.OutputLog("144");
            try
            {
                if (PrinterHandle != IntPtr.Zero)
                {
                    ClosePrinter((int)PrinterHandle);
                    PrinterHandle = IntPtr.Zero;
                }
                if (WaitHandle != null)
                {
                    LogUtility.OutputDebugLog("E144");
                    WaitHandle.Unregister(null);
                }
                if (MrEvent != null)
                {
                    if (!MrEvent.SafeWaitHandle.IsClosed)
                    {
                        //_changeHandleがCloseされているか？
                        bool chk = FindClosePrinterChangeNotification(ChangeHandle);
                    }

                    MrEvent.Close();
                }
                isStarted = false;
            }
            catch (Exception ex)
            {
                LogUtility.OutputDebugLog("E145", ex.Message);
            }
        }
        #endregion
        /// <summary>
        /// プリンタイベント文字列変換処理
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public string getPrinterMsg(uint Code)
        {
            string rtn = "";
            switch (Code)
            {
                case 1:
                    rtn = "PRINTER_CHANGE_ADD_PRINTER";
                    break;
                case 2:
                    rtn = "PRINTER_CHANGE_SET_PRINTER";
                    break;
                case 4:
                    rtn = "PRINTER_CHANGE_DELETE_PRINTER";
                    break;
                case 8:
                    rtn = "PRINTER_CHANGE_FAILED_CONNECTION_PRINTER";
                    break;
                case 0xFF:
                    rtn = "PRINTER_CHANGE_PRINTER";
                    break;
                case 0x100:
                    rtn = "PRINTER_CHANGE_ADD_JOB";
                    break;
                case 0x200:
                    rtn = "PRINTER_CHANGE_SET_JOB";
                    break;
                case 0x400:
                    rtn = "PRINTER_CHANGE_DELETE_JOB";
                    break;
                case 0x800:
                    rtn = "PRINTER_CHANGE_WRITE_JOB";
                    break;
                case 0xFF00:
                    rtn = "PRINTER_CHANGE_JOB";
                    break;
                case 0x10000:
                    rtn = "PRINTER_CHANGE_ADD_FORM";
                    break;
                case 0x20000:
                    rtn = "PRINTER_CHANGE_SET_FORM";
                    break;
                case 0x40000:
                    rtn = "PRINTER_CHANGE_DELETE_FORM";
                    break;
                case 0x70000:
                    rtn = "PRINTER_CHANGE_FORM";
                    break;
                case 0x100000:
                    rtn = "PRINTER_CHANGE_ADD_PORT";
                    break;
                case 0x200000:
                    rtn = "PRINTER_CHANGE_CONFIGURE_PORT";
                    break;
                case 0x400000:
                    rtn = "PRINTER_CHANGE_DELETE_PORT";
                    break;
                case 0x700000:
                    rtn = "PRINTER_CHANGE_PORT";
                    break;
                case 0x1000000:
                    rtn = "PRINTER_CHANGE_ADD_PRINT_PROCESSOR";
                    break;
                case 0x4000000:
                    rtn = "PRINTER_CHANGE_DELETE_PRINT_PROCESSOR";
                    break;
                case 0x7000000:
                    rtn = "PRINTER_CHANGE_PRINT_PROCESSOR";
                    break;
                case 0x10000000:
                    rtn = "PRINTER_CHANGE_ADD_PRINTER_DRIVER";
                    break;
                case 0x20000000:
                    rtn = "PRINTER_CHANGE_SET_PRINTER_DRIVER";
                    break;
                case 0x40000000:
                    rtn = "PRINTER_CHANGE_DELETE_PRINTER_DRIVER";
                    break;
                case 0x70000000:
                    rtn = "PRINTER_CHANGE_PRINTER_DRIVER";
                    break;
                case 0x80000000:
                    rtn = "PRINTER_CHANGE_TIMEOUT";
                    break;
                case 0x7777FFFF:
                    rtn = "PRINTER_CHANGE_ALL";
                    break;
                default:
                    rtn = "PRINTER_CHANGE_UNKNOWN";
                    break;
            }

            return rtn;
        }
    }
    #region Windowsプリンタ定義定数値
    [Flags]
    public enum JOBSTATUS
    {
        JOB_STATUS_PAUSED = 0x00000001,
        JOB_STATUS_ERROR = 0x00000002,
        JOB_STATUS_DELETING = 0x00000004,
        JOB_STATUS_SPOOLING = 0x00000008,
        JOB_STATUS_PRINTING = 0x00000010,
        JOB_STATUS_OFFLINE = 0x00000020,
        JOB_STATUS_PAPEROUT = 0x00000040,
        JOB_STATUS_PRINTED = 0x00000080,
        JOB_STATUS_DELETED = 0x00000100,
        JOB_STATUS_BLOCKED_DEVQ = 0x00000200,
        JOB_STATUS_USER_INTERVENTION = 0x00000400,
        JOB_STATUS_RESTART = 0x00000800,
        JOB_STATUS_COMPLETE = 0x00001000,
        JOB_STATUS_RETAINED = 0x00002000,
        JOB_STATUS_RENDERING_LOCALLY = 0x00004000,
    }
    public class PRINTER_CHANGES
    {
        public const uint PRINTER_CHANGE_ADD_PRINTER = 1;
        public const uint PRINTER_CHANGE_SET_PRINTER = 2;
        public const uint PRINTER_CHANGE_DELETE_PRINTER = 4;
        public const uint PRINTER_CHANGE_FAILED_CONNECTION_PRINTER = 8;
        public const uint PRINTER_CHANGE_PRINTER = 0xFF;
        public const uint PRINTER_CHANGE_ADD_JOB = 0x100;
        public const uint PRINTER_CHANGE_SET_JOB = 0x200;
        public const uint PRINTER_CHANGE_DELETE_JOB = 0x400;
        public const uint PRINTER_CHANGE_WRITE_JOB = 0x800;
        public const uint PRINTER_CHANGE_JOB = 0xFF00;
        public const uint PRINTER_CHANGE_ADD_FORM = 0x10000;
        public const uint PRINTER_CHANGE_SET_FORM = 0x20000;
        public const uint PRINTER_CHANGE_DELETE_FORM = 0x40000;
        public const uint PRINTER_CHANGE_FORM = 0x70000;
        public const uint PRINTER_CHANGE_ADD_PORT = 0x100000;
        public const uint PRINTER_CHANGE_CONFIGURE_PORT = 0x200000;
        public const uint PRINTER_CHANGE_DELETE_PORT = 0x400000;
        public const uint PRINTER_CHANGE_PORT = 0x700000;
        public const uint PRINTER_CHANGE_ADD_PRINT_PROCESSOR = 0x1000000;
        public const uint PRINTER_CHANGE_DELETE_PRINT_PROCESSOR = 0x4000000;
        public const uint PRINTER_CHANGE_PRINT_PROCESSOR = 0x7000000;
        public const uint PRINTER_CHANGE_ADD_PRINTER_DRIVER = 0x10000000;
        public const uint PRINTER_CHANGE_SET_PRINTER_DRIVER = 0x20000000;
        public const uint PRINTER_CHANGE_DELETE_PRINTER_DRIVER = 0x40000000;
        public const uint PRINTER_CHANGE_PRINTER_DRIVER = 0x70000000;
        public const uint PRINTER_CHANGE_TIMEOUT = 0x80000000;
        public const uint PRINTER_CHANGE_ALL = 0x7777FFFF;
    }

    public enum PRINTERPRINTERNOTIFICATIONTYPES
    {
        PRINTER_NOTIFY_FIELD_SERVER_NAME = 0,
        PRINTER_NOTIFY_FIELD_PRINTER_NAME = 1,
        PRINTER_NOTIFY_FIELD_SHARE_NAME = 2,
        PRINTER_NOTIFY_FIELD_PORT_NAME = 3,
        PRINTER_NOTIFY_FIELD_DRIVER_NAME = 4,
        PRINTER_NOTIFY_FIELD_COMMENT = 5,
        PRINTER_NOTIFY_FIELD_LOCATION = 6,
        PRINTER_NOTIFY_FIELD_DEVMODE = 7,
        PRINTER_NOTIFY_FIELD_SEPFILE = 8,
        PRINTER_NOTIFY_FIELD_PRINT_PROCESSOR = 9,
        PRINTER_NOTIFY_FIELD_PARAMETERS = 10,
        PRINTER_NOTIFY_FIELD_DATATYPE = 11,
        PRINTER_NOTIFY_FIELD_SECURITY_DESCRIPTOR = 12,
        PRINTER_NOTIFY_FIELD_ATTRIBUTES = 13,
        PRINTER_NOTIFY_FIELD_PRIORITY = 14,
        PRINTER_NOTIFY_FIELD_DEFAULT_PRIORITY = 15,
        PRINTER_NOTIFY_FIELD_START_TIME = 16,
        PRINTER_NOTIFY_FIELD_UNTIL_TIME = 17,
        PRINTER_NOTIFY_FIELD_STATUS = 18,
        PRINTER_NOTIFY_FIELD_STATUS_STRING = 19,
        PRINTER_NOTIFY_FIELD_CJOBS = 20,
        PRINTER_NOTIFY_FIELD_AVERAGE_PPM = 21,
        PRINTER_NOTIFY_FIELD_TOTAL_PAGES = 22,
        PRINTER_NOTIFY_FIELD_PAGES_PRINTED = 23,
        PRINTER_NOTIFY_FIELD_TOTAL_BYTES = 24,
        PRINTER_NOTIFY_FIELD_BYTES_PRINTED = 25,
    }

    public enum PRINTERJOBNOTIFICATIONTYPES
    {
        JOB_NOTIFY_FIELD_PRINTER_NAME = 0,
        JOB_NOTIFY_FIELD_MACHINE_NAME = 1,
        JOB_NOTIFY_FIELD_PORT_NAME = 2,
        JOB_NOTIFY_FIELD_USER_NAME = 3,
        JOB_NOTIFY_FIELD_NOTIFY_NAME = 4,
        JOB_NOTIFY_FIELD_DATATYPE = 5,
        JOB_NOTIFY_FIELD_PRINT_PROCESSOR = 6,
        JOB_NOTIFY_FIELD_PARAMETERS = 7,
        JOB_NOTIFY_FIELD_DRIVER_NAME = 8,
        JOB_NOTIFY_FIELD_DEVMODE = 9,
        JOB_NOTIFY_FIELD_STATUS = 10,
        JOB_NOTIFY_FIELD_STATUS_STRING = 11,
        JOB_NOTIFY_FIELD_SECURITY_DESCRIPTOR = 12,
        JOB_NOTIFY_FIELD_DOCUMENT = 13,
        JOB_NOTIFY_FIELD_PRIORITY = 14,
        JOB_NOTIFY_FIELD_POSITION = 15,
        JOB_NOTIFY_FIELD_SUBMITTED = 16,
        JOB_NOTIFY_FIELD_START_TIME = 17,
        JOB_NOTIFY_FIELD_UNTIL_TIME = 18,
        JOB_NOTIFY_FIELD_TIME = 19,
        JOB_NOTIFY_FIELD_TOTAL_PAGES = 20,
        JOB_NOTIFY_FIELD_PAGES_PRINTED = 21,
        JOB_NOTIFY_FIELD_TOTAL_BYTES = 22,
        JOB_NOTIFY_FIELD_BYTES_PRINTED = 23,
    }
    #endregion

    #region Windowsプリンタアクセス構造体・関数定義
    [StructLayout(LayoutKind.Sequential)]
    public class PRINTER_NOTIFY_OPTIONS
    {
        public int dwVersion = 2;
        public int dwFlags;
        public int Count = 2;
        public IntPtr lpTypes;

        public PRINTER_NOTIFY_OPTIONS()
        {
            int bytesNeeded = (2 + PRINTER_NOTIFY_OPTIONS_TYPE.JOB_FIELDS_COUNT + PRINTER_NOTIFY_OPTIONS_TYPE.PRINTER_FIELDS_COUNT) * 2;
            PRINTER_NOTIFY_OPTIONS_TYPE pJobTypes = new PRINTER_NOTIFY_OPTIONS_TYPE();
            lpTypes = Marshal.AllocHGlobal(bytesNeeded);
            Marshal.StructureToPtr(pJobTypes, lpTypes, true);
        }
    }

    public enum PRINTERNOTIFICATIONTYPES
    {
        PRINTER_NOTIFY_TYPE = 0,
        JOB_NOTIFY_TYPE = 1
    }

    [StructLayout(LayoutKind.Sequential)]
    public class PRINTER_NOTIFY_OPTIONS_TYPE
    {
        public const int JOB_FIELDS_COUNT = 24;
        public const int PRINTER_FIELDS_COUNT = 23;

        public Int16 wJobType;
        public Int16 wJobReserved0;
        public Int32 dwJobReserved1;
        public Int32 dwJobReserved2;
        public Int32 JobFieldCount;
        public IntPtr pJobFields;
        public Int16 wPrinterType;
        public Int16 wPrinterReserved0;
        public Int32 dwPrinterReserved1;
        public Int32 dwPrinterReserved2;
        public Int32 PrinterFieldCount;
        public IntPtr pPrinterFields;

        private void SetupFields()
        {
            if (pJobFields.ToInt32() != 0)
            {
                Marshal.FreeHGlobal(pJobFields);
            }

            if (wJobType == (short)PRINTERNOTIFICATIONTYPES.JOB_NOTIFY_TYPE)
            {
                JobFieldCount = JOB_FIELDS_COUNT;
                pJobFields = Marshal.AllocHGlobal((JOB_FIELDS_COUNT * 2) - 1);

                Marshal.WriteInt16(pJobFields, 0, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_PRINTER_NAME);
                Marshal.WriteInt16(pJobFields, 2, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_MACHINE_NAME);
                Marshal.WriteInt16(pJobFields, 4, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_PORT_NAME);
                Marshal.WriteInt16(pJobFields, 6, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_USER_NAME);
                Marshal.WriteInt16(pJobFields, 8, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_NOTIFY_NAME);
                Marshal.WriteInt16(pJobFields, 10, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_DATATYPE);
                Marshal.WriteInt16(pJobFields, 12, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_PRINT_PROCESSOR);
                Marshal.WriteInt16(pJobFields, 14, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_PARAMETERS);
                Marshal.WriteInt16(pJobFields, 16, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_DRIVER_NAME);
                Marshal.WriteInt16(pJobFields, 18, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_DEVMODE);
                Marshal.WriteInt16(pJobFields, 20, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_STATUS);
                Marshal.WriteInt16(pJobFields, 22, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_STATUS_STRING);
                Marshal.WriteInt16(pJobFields, 24, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_SECURITY_DESCRIPTOR);
                Marshal.WriteInt16(pJobFields, 26, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_DOCUMENT);
                Marshal.WriteInt16(pJobFields, 28, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_PRIORITY);
                Marshal.WriteInt16(pJobFields, 30, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_POSITION);
                Marshal.WriteInt16(pJobFields, 32, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_SUBMITTED);
                Marshal.WriteInt16(pJobFields, 34, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_START_TIME);
                Marshal.WriteInt16(pJobFields, 36, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_UNTIL_TIME);
                Marshal.WriteInt16(pJobFields, 38, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_TIME);
                Marshal.WriteInt16(pJobFields, 40, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_TOTAL_PAGES);
                Marshal.WriteInt16(pJobFields, 42, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_PAGES_PRINTED);
                Marshal.WriteInt16(pJobFields, 44, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_TOTAL_BYTES);
                Marshal.WriteInt16(pJobFields, 46, (short)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_BYTES_PRINTED);
            }

            if (pPrinterFields.ToInt32() != 0)
            {
                Marshal.FreeHGlobal(pPrinterFields);
            }

            if (wPrinterType == (short)PRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_TYPE)
            {
                PrinterFieldCount = PRINTER_FIELDS_COUNT;
                pPrinterFields = Marshal.AllocHGlobal((PRINTER_FIELDS_COUNT - 1) * 2);

                Marshal.WriteInt16(pPrinterFields, 0, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_SERVER_NAME);
                Marshal.WriteInt16(pPrinterFields, 2, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_PRINTER_NAME);
                Marshal.WriteInt16(pPrinterFields, 4, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_SHARE_NAME);
                Marshal.WriteInt16(pPrinterFields, 6, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_PORT_NAME);
                Marshal.WriteInt16(pPrinterFields, 8, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_DRIVER_NAME);
                Marshal.WriteInt16(pPrinterFields, 10, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_COMMENT);
                Marshal.WriteInt16(pPrinterFields, 12, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_LOCATION);
                Marshal.WriteInt16(pPrinterFields, 14, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_SEPFILE);
                Marshal.WriteInt16(pPrinterFields, 16, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_PRINT_PROCESSOR);
                Marshal.WriteInt16(pPrinterFields, 18, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_PARAMETERS);
                Marshal.WriteInt16(pPrinterFields, 20, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_DATATYPE);
                Marshal.WriteInt16(pPrinterFields, 22, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_ATTRIBUTES);
                Marshal.WriteInt16(pPrinterFields, 24, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_PRIORITY);
                Marshal.WriteInt16(pPrinterFields, 26, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_DEFAULT_PRIORITY);
                Marshal.WriteInt16(pPrinterFields, 28, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_START_TIME);
                Marshal.WriteInt16(pPrinterFields, 30, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_UNTIL_TIME);
                Marshal.WriteInt16(pPrinterFields, 32, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_STATUS_STRING);
                Marshal.WriteInt16(pPrinterFields, 34, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_CJOBS);
                Marshal.WriteInt16(pPrinterFields, 36, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_AVERAGE_PPM);
                Marshal.WriteInt16(pPrinterFields, 38, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_TOTAL_PAGES);
                Marshal.WriteInt16(pPrinterFields, 40, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_PAGES_PRINTED);
                Marshal.WriteInt16(pPrinterFields, 42, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_TOTAL_BYTES);
                Marshal.WriteInt16(pPrinterFields, 44, (short)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_BYTES_PRINTED);
            }
        }

        public PRINTER_NOTIFY_OPTIONS_TYPE()
        {
            wJobType = (short)PRINTERNOTIFICATIONTYPES.JOB_NOTIFY_TYPE;
            wPrinterType = (short)PRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_TYPE;

            SetupFields();
        }
    }

    public class PrintJobChangeEventArgs : EventArgs
    {
        #region private variables
        private int _jobID = 0;
        private string _jobName = "";
        private JOBSTATUS _jobStatus = new JOBSTATUS();
        private PrintSystemJobInfo _jobInfo = null;
        #endregion

        public int JobID { get { return _jobID; } }
        public string JobName { get { return _jobName; } }
        public JOBSTATUS JobStatus { get { return _jobStatus; } }
        public PrintSystemJobInfo JobInfo { get { return _jobInfo; } }
        public PrintJobChangeEventArgs(int intJobID, string strJobName, JOBSTATUS jStatus, PrintSystemJobInfo objJobInfo)
            : base()
        {
            _jobID = intJobID;
            _jobName = strJobName;
            _jobStatus = jStatus;
            _jobInfo = objJobInfo;
        }
    }
    public delegate void PrintJobStatusChanged(object Sender, PrintJobChangeEventArgs e);
    #endregion


}
