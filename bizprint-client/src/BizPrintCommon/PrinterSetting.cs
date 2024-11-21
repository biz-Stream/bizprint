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
using System.ComponentModel;

namespace BizPrintCommon
{
    /// <summary>
    /// プリンタデフォルトトレイ操作クラス
    /// </summary>
    public class PrinterSetting
    {
        #region "Private Variables"
        private IntPtr hPrinter = new System.IntPtr();
        private PRINTER_DEFAULTS PrinterValues = new PRINTER_DEFAULTS();
        private PRINTER_INFO_2 pinfo = new PRINTER_INFO_2();
        private DEVMODE devMode;
        private IntPtr ptrDM = IntPtr.Zero;
        private IntPtr ptrPrinterInfo = IntPtr.Zero;
        private int sizeOfDevMode = 0;
        private int lastError = 0;
        private int nBytesNeeded;
        private long nRet;
        private int intError;
        private System.Int32 nJunk;
        private IntPtr yDevModeData;

        #endregion
        #region "Win API Def"
        [DllImport("kernel32.dll", EntryPoint = "GetLastError", SetLastError = false,
        ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern Int32 GetLastError();
        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true,
        ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool ClosePrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", EntryPoint = "DocumentPropertiesA", SetLastError = true,
        ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int DocumentProperties(IntPtr hwnd, IntPtr hPrinter,
        [MarshalAs(UnmanagedType.LPStr)] string pDeviceNameg,
        IntPtr pDevModeOutput, ref IntPtr pDevModeInput, int fMode);
        [DllImport("winspool.Drv", EntryPoint = "GetPrinterA", SetLastError = true,
            CharSet = CharSet.Ansi, ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        private static extern bool GetPrinter(IntPtr hPrinter, Int32 dwLevel,
        IntPtr pPrinter, Int32 dwBuf, out Int32 dwNeeded);
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA",
            SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool
            OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter,
            out IntPtr hPrinter, ref PRINTER_DEFAULTS pd);
        [DllImport("winspool.drv", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern bool SetPrinter(IntPtr hPrinter, int Level, IntPtr
        pPrinter, int Command);
        [DllImport("kernel32.dll")]
        static extern uint FormatMessage(
        uint dwFlags, IntPtr lpSource,
        uint dwMessageId, uint dwLanguageId,
        StringBuilder lpBuffer, int nSize,
        IntPtr Arguments);

        #endregion
        #region "Data structure"
        [StructLayout(LayoutKind.Sequential)]
        public struct PRINTER_DEFAULTS
        {
            //[MarshalAs(UnmanagedType.LPStr)]
            public IntPtr pDatatype;
            public IntPtr pDevMode;
            public int DesiredAccess;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct PRINTER_INFO_2
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pServerName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pPrinterName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pShareName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pPortName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDriverName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pComment;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pLocation;
            public IntPtr pDevMode;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pSepFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pPrintProcessor;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDatatype;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pParameters;
            public IntPtr pSecurityDescriptor;
            public Int32 Attributes;
            public Int32 Priority;
            public Int32 DefaultPriority;
            public Int32 StartTime;
            public Int32 UntilTime;
            public Int32 Status;
            public Int32 cJobs;
            public Int32 AveragePPM;
        }
        private const short CCDEVICENAME = 32;
        private const short CCFORMNAME = 32;
        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCDEVICENAME)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public short dmOrientation;
            public short dmPaperSize;
            public short dmPaperLength;
            public short dmPaperWidth;
            public short dmScale;
            public short dmCopies;
            public short dmDefaultSource;
            public short dmPrintQuality;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCFORMNAME)]
            public string dmFormName;
            public short dmUnusedPadding;
            public short dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
        }
        public class PrinterData
        {
            public PrinterData()
            {
                Duplex = 1;
                source = 1;
                Orientation = 1;
                Size = 1;
            }

            public int Duplex { get; set; }
            public int source { get; set; }
            public int Orientation { get; set; }
            public int Size { get; set; }
        }
        #endregion
        #region "Constants"
        private const int DM_DUPLEX = 0x1000;
        private const int DM_IN_BUFFER = 8;
        private const int DM_OUT_BUFFER = 2;
        private const int PRINTER_ACCESS_ADMINISTER = 0x4;
        private const int PRINTER_ACCESS_USE = 0x8;
        private const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        private const int PRINTER_ALL_ACCESS =
            (STANDARD_RIGHTS_REQUIRED | PRINTER_ACCESS_ADMINISTER
            | PRINTER_ACCESS_USE);
        private const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
        #endregion
        private int LastErrorCode = ErrCodeAndmErrMsg.STATUS_OK;
        /// <summary>
        /// エラーコードからメッセージへの変換関数
        /// </summary>
        /// <param name="ErrCode"></param>
        /// <returns></returns>
        public static string getErrMessageFromCode(int ErrCode)
        {
            StringBuilder message = new StringBuilder(255);
            FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, (uint)ErrCode, 0, message, message.Capacity, IntPtr.Zero);
            return message.ToString();
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PrinterSetting()
        {

        }
        /// <summary>
        /// デフォルトトレイの設定
        /// </summary>
        /// <param name="PrinterName"></param>
        /// <param name="TrayNum"></param>
        /// <returns>0:成功、それ以外：失敗</returns>
        public int ChangeDefaultTray(string PrinterName, int TrayNum, string TrayName)
        {
            int rtn = ErrCodeAndmErrMsg.STATUS_OK;

            try
            {
                devMode = this.GetPrinterDevmode(PrinterName);
                int setRet = 0;
                if (LastErrorCode == ErrCodeAndmErrMsg.ERR_CODE_0411)
                {
                    //OpenError
                    LogUtility.OutputLog("146", lastError.ToString(), getErrMessageFromCode(lastError));
                    rtn = ErrCodeAndmErrMsg.ERR_CODE_0411;
                    nRet = 1;
                }
                else
                {
                    devMode.dmDefaultSource = (short)TrayNum;
                    Marshal.StructureToPtr(devMode, yDevModeData, true);
                    pinfo.pDevMode = yDevModeData;
                    pinfo.pSecurityDescriptor = IntPtr.Zero;

                    Marshal.StructureToPtr(pinfo, ptrPrinterInfo, false);
                    lastError = Marshal.GetLastWin32Error();
                    setRet = Convert.ToInt16(SetPrinter(hPrinter, 2, ptrPrinterInfo, 0));
                    if (setRet == 0)
                    {
                        //Unable to set shared printer settings.
                        lastError = Marshal.GetLastWin32Error();
                        rtn = ErrCodeAndmErrMsg.ERR_CODE_0412;
                        LogUtility.OutputDebugLog("E412", lastError.ToString());

                        LogUtility.OutputLog("147", PrinterName, TrayNum.ToString() + ":" + TrayName, lastError.ToString(), getErrMessageFromCode(lastError));

                    }
                    else
                    {
                        LogUtility.OutputLog("148", PrinterName, TrayNum.ToString() + ":" + TrayName);
                    }
                }
                if (hPrinter != IntPtr.Zero)
                {
                    ClosePrinter(hPrinter);
                }
            }
            catch (Exception ex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                LogUtility.OutputLog("149", ex.Message);
            }
            finally
            {
                //ptrDM
                if (ptrDM != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptrDM);
                }
                if (ptrPrinterInfo != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptrPrinterInfo);
                }
            }

            return rtn;
        }


        /// <summary>
        /// デフォルト印刷部数の設定
        /// </summary>
        /// <param name="PrinterName">プリンタ名</param>
        /// <param name="Copies">部数</param>
        /// <returns>0:成功、それ以外：失敗</returns>
        public int ChangeDefaultCopies(string PrinterName, int Copies)
        {
            int rtn = ErrCodeAndmErrMsg.STATUS_OK;

            try
            {
                devMode = this.GetPrinterDevmode(PrinterName);
                int setRet = 0;
                if (LastErrorCode == ErrCodeAndmErrMsg.ERR_CODE_0411)
                {
                    //OpenError
                    LogUtility.OutputLog("166", lastError.ToString(), getErrMessageFromCode(lastError));
                    rtn = ErrCodeAndmErrMsg.ERR_CODE_0411;
                    nRet = 1;
                }
                else
                {
                    devMode.dmCopies = (short)Copies;
                    Marshal.StructureToPtr(devMode, yDevModeData, true);
                    pinfo.pDevMode = yDevModeData;
                    pinfo.pSecurityDescriptor = IntPtr.Zero;

                    Marshal.StructureToPtr(pinfo, ptrPrinterInfo, false);
                    lastError = Marshal.GetLastWin32Error();
                    setRet = Convert.ToInt16(SetPrinter(hPrinter, 2, ptrPrinterInfo, 0));
                    if (setRet == 0)
                    {
                        //Unable to set shared printer settings.
                        lastError = Marshal.GetLastWin32Error();
                        rtn = ErrCodeAndmErrMsg.ERR_CODE_0412;
                        LogUtility.OutputDebugLog("E413", lastError.ToString());

                        LogUtility.OutputLog("167", PrinterName, Copies.ToString(), lastError.ToString(), getErrMessageFromCode(lastError));

                    }
                    else
                    {
                        LogUtility.OutputLog("168", PrinterName, Copies.ToString());
                    }
                }

                if (hPrinter != IntPtr.Zero)
                {
                    ClosePrinter(hPrinter);
                }
            }
            catch (Exception ex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                LogUtility.OutputLog("169", ex.Message);
            }
            finally
            {
                //ptrDM
                if (ptrDM != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptrDM);
                }
                if (ptrPrinterInfo != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptrPrinterInfo);
                }
            }

            return rtn;
        }


        /// <summary>
        /// DEVMODE構造体の取得
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        private DEVMODE GetPrinterDevmode(string printerName)
        {
            PrinterData printerData = new PrinterData();
            DEVMODE rtnDevMode = new DEVMODE();
            const int PRINTER_ACCESS_ADMINISTER = 0x4;
            const int PRINTER_ACCESS_USE = 0x8;

            try
            {
                PrinterValues.pDatatype = IntPtr.Zero;
                PrinterValues.pDevMode = IntPtr.Zero;
                PrinterValues.DesiredAccess = STANDARD_RIGHTS_REQUIRED | PRINTER_ACCESS_USE | PRINTER_ACCESS_ADMINISTER;
                try
                {
                    nRet = Convert.ToInt32(OpenPrinter(printerName,
                                   out hPrinter, ref PrinterValues));
                }
                catch (Exception ex)
                {
                    lastError = ex.HResult;
                    LastErrorCode = ErrCodeAndmErrMsg.ERR_CODE_0411;
                    LogUtility.OutputDebugLog("E411", ex.Message, getErrMessageFromCode(lastError));

                    return rtnDevMode;
                }
                if (nRet == 0)
                {
                    lastError = Marshal.GetLastWin32Error();
                    LastErrorCode = ErrCodeAndmErrMsg.ERR_CODE_0411;
                    LogUtility.OutputDebugLog("E411", "OpenPrinter Error", getErrMessageFromCode(lastError));
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                GetPrinter(hPrinter, 2, IntPtr.Zero, 0, out nBytesNeeded);
                if (nBytesNeeded <= 0)
                {
                    throw new System.Exception("Unable to allocate memory");
                }
                else
                {
                    // Allocate enough space for PRINTER_INFO_2... 
                    {
                        Marshal.AllocCoTaskMem(nBytesNeeded);
                    };
                    ptrPrinterInfo = Marshal.AllocHGlobal(nBytesNeeded);
                    // The second GetPrinter fills in all the current settings, so all you 
                    // need to do is modify what you're interested in...
                    nRet = Convert.ToInt32(GetPrinter(hPrinter, 2,
                        ptrPrinterInfo, nBytesNeeded, out nJunk));
                    if (nRet == 0)
                    {
                        lastError = Marshal.GetLastWin32Error();
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                    pinfo = (PRINTER_INFO_2)Marshal.PtrToStructure(ptrPrinterInfo,
                                                          typeof(PRINTER_INFO_2));
                    IntPtr Temp = new IntPtr();
                    if (pinfo.pDevMode == IntPtr.Zero)
                    {
                        // If GetPrinter didn't fill in the DEVMODE, try to get it by calling
                        // DocumentProperties...
                        IntPtr ptrZero = IntPtr.Zero;
                        //get the size of the devmode structure
                        sizeOfDevMode = DocumentProperties(IntPtr.Zero, hPrinter,
                                           printerName, ptrZero, ref ptrZero, 0);

                        ptrDM = Marshal.AllocCoTaskMem(sizeOfDevMode);
                        int i;
                        i = DocumentProperties(IntPtr.Zero, hPrinter, printerName, ptrDM,
                        ref ptrZero, DM_OUT_BUFFER);
                        if ((i < 0) || (ptrDM == IntPtr.Zero))
                        {
                            //Cannot get the DEVMODE structure.
                            throw new System.Exception("Cannot get DEVMODE data");
                        }
                        pinfo.pDevMode = ptrDM;
                    }
                    intError = DocumentProperties(IntPtr.Zero, hPrinter,
                              printerName, IntPtr.Zero, ref Temp, 0);

                    yDevModeData = Marshal.AllocHGlobal(intError);
                    intError = DocumentProperties(IntPtr.Zero, hPrinter,
                             printerName, yDevModeData, ref Temp, 2);
                    rtnDevMode = (DEVMODE)Marshal.PtrToStructure(yDevModeData, typeof(DEVMODE));
                    //nRet = DocumentProperties(IntPtr.Zero, hPrinter, sPrinterName, yDevModeData
                    // , ref yDevModeData, (DM_IN_BUFFER | DM_OUT_BUFFER));
                    if ((nRet == 0) || (hPrinter == IntPtr.Zero))
                    {
                        lastError = Marshal.GetLastWin32Error();
                        //string myErrMsg = GetErrorMessage(lastError);
                        throw new Win32Exception(lastError);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
            }
            return rtnDevMode;
        }



    }


}

