
using BizPrintCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SilentPdfPrinter
{
    /// <summary>
    /// SilentPdfPrinter用設定取得・保持クラス
    /// </summary>
    class SettingManagerOfSilent

    {

        //設定値はすべて「private set」とし、外部から書き換え不可

        /// <summary>サービス起動失敗時の再起動待機時間(ミリ秒)</summary>
        public int RetryInterval { get; private set; }
        /// <summary>サービス接続失敗時のリトライ回数</summary>
        public int RetryCount { get; private set; }
        /// <summary>サービスプロセス名</summary>
        public string ProcessName { get; private set; }
        /// <summary>サービス接続時のタイムアウト（ミリ秒）</summary>
        public int ConnectTimeout { get; private set; }
        /// <summary>引数として与えられたファイルを削除するか</summary>
        public bool IsDeleteFile { get; private set; }
        /// <summary>ダイレクト印刷サービスの動作するホスト名</summary>
        public string DirectPrintHostName { get; private set; }
        /// <summary>ダイレクト印刷サービスのポート番号</summary>
        public int PortNo { get; private set; }

        /// <summary>
        /// SilentPdfPrinterが複数起動していた場合のチェック待機時間msec
        /// </summary>
        public int WaitMsec { get; private set; }

        /// <summary>
        /// SilentPdfPrinterの最大並列起動数
        /// </summary>
        public int MaxProccessCount { get; private set; }

        /// <summary>
        /// Directの最大同時接続数
        /// </summary>
        public int ConcurrentConnectionsMax { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SettingManagerOfSilent()
        {

        }
        /// <summary>
        /// 設定ファイルのロード
        /// </summary>
        /// <returns></returns>
        public bool LoadSetting()
        {
            string configPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + SilentConstants.ConfFolderName + "\\" + SilentConstants.ConfFileName;
#if DEBUG
            configPath = @"C:\ProgramData\brainsellers\DirectPrint\" + SilentConstants.ConfFileName;
#endif
            XMLLoader loader = new XMLLoader();
            try
            {
                loader.LoadFromXMLFile(configPath);
            }
            catch (Exception e)
            {
                //(ログID：Sl041)
                LogUtility.OutputLog("041", configPath, e.Message);
                MessageBox.Show(LogUtility.GetLogStr("041", configPath, e.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            ConcurrentConnectionsMax = loader.ReadEntryInt(SilentConstants.INI_SECTION_APP, SilentConstants.INI_CONC_CONECT_MAX, SilentConstants.DEFAULT_CONC_CONECT_MAX);
            IsDeleteFile = loader.ReadEntryBool(SilentConstants.INI_SECTION_APP, SilentConstants.INI_DELETE_FILE_KEY, SilentConstants.DEFAULT_DELETE_FILE);
            DirectPrintHostName = loader.ReadEntry(SilentConstants.INI_SECTION_APP, SilentConstants.INI_DIRECT_PRINT_HOST_KEY, SilentConstants.DEFAULT_DIRECT_PRINT_IP);
            MaxProccessCount = loader.ReadEntryInt(SilentConstants.INI_SECTION_APP, SilentConstants.INI_MAX_PROC_NUM, SilentConstants.DEFAULT_MAX_PROC_NUM);
            PortNo = loader.ReadEntryInt(SilentConstants.INI_SECTION_APP, SilentConstants.INI_PORT_KEY, SilentConstants.DEFAULT_PORT_KEY);
            ProcessName = loader.ReadEntry(SilentConstants.INI_SECTION_APP, SilentConstants.INI_PROCESSNAME_KEY, SilentConstants.DEFAULT_PROCESSNAME);
            RetryCount = loader.ReadEntryInt(SilentConstants.INI_SECTION_APP, SilentConstants.INI_RETRY_KEY, SilentConstants.DEFAULT_RETRY);
            RetryInterval = loader.ReadEntryInt(SilentConstants.INI_SECTION_APP, SilentConstants.INI_RETRYINTERVAL_KEY, SilentConstants.DEFAULT_RETRYINTERVAL);
            ConnectTimeout = loader.ReadEntryInt(SilentConstants.INI_SECTION_APP, SilentConstants.INI_TIMEOUT_KEY, SilentConstants.DEFAULT_TIMEOUT);
            WaitMsec = loader.ReadEntryInt(SilentConstants.INI_SECTION_APP, SilentConstants.INI_WAIT_MSEC, SilentConstants.DEFAULT_WAIT_MSEC);

            if (ConcurrentConnectionsMax < 1) {
                ConcurrentConnectionsMax = SilentConstants.DEFAULT_CONC_CONECT_MAX;
            }
            //(ログID：Sl009)
            string dbgLog = "";
            dbgLog += "\r\nconcurrentconnectionsmax=" + ConcurrentConnectionsMax;
            dbgLog += "\r\ndeletefile=" + IsDeleteFile;
            dbgLog += "\r\ndirectprinthost=" + DirectPrintHostName;
            dbgLog += "\r\nmaxprocessnum=" + MaxProccessCount;
            dbgLog += "\r\nport=" + PortNo;
            dbgLog += "\r\nprocessname=" + ProcessName;
            dbgLog += "\r\nretry=" + RetryCount;
            dbgLog += "\r\nretryinterval=" + RetryInterval;
            dbgLog += "\r\ntimeout=" + ConnectTimeout;
            dbgLog += "\r\nwaitloopmsec=" + WaitMsec;

            LogUtility.OutputLog("009", dbgLog);

            return true;
        }
    }
}
