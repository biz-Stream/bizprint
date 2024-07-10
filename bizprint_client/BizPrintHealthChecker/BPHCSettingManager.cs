using BizPrintCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BizPrintHealthChecker
{
    class BPHCSettingManager
    {
        public int ServiceType = -1;
        /// <summary>サービス起動失敗時の再起動待機時間(ミリ秒)</summary>
        public int ConnectRetryWaitMsec { get; private set; }
        /// <summary>サービス接続失敗時のリトライ回数</summary>
        public int ConnectRetryNum { get; private set; }
        /// <summary>サービス接続時のタイムアウト（ミリ秒）</summary>
        public int ConnectTimeout { get; private set; }
        /// <summary>プロセス強制終了時の終了チェックリトライ回数</summary>
        public int KilledCheckRetryNum { get; private set; }

        /// <summary>ダイレクト印刷orバッチ印刷サービスサーバのアドレス</summary>
        public string ServerAddress { get; private set; }

        /// <summary>ダイレクト印刷orバッチ印刷サービスのポート番号</summary>
        public int PortNo { get; private set; }

       public BPHCSettingManager(int Type) {
            ServiceType = Type;
        }

        public string ServiceName() {
            if (ServiceType == BPHCConstants.MODE_DIRECT) {
                return BPHCConstants.PROCESSNAME_DIRECT;
            }else
            {
                return BPHCConstants.PROCESSNAME_BATCH;
            }
        }
        /// <summary>
        /// 設定ファイルのロード
        /// </summary>
        public bool LoadSetting()
        {
            string configPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\";
            if (ServiceType == BPHCConstants.MODE_BATCH)
            {
                configPath += BPHCConstants.CONF_FOLDER_BATCH+"\\"+ BPHCConstants.SETTINGFILE_BPHC;
            }
            else {
                configPath += BPHCConstants.CONF_FOLDER_DIRECT + "\\" + BPHCConstants.SETTINGFILE_BPHC;
            }
            XMLLoader loader = new XMLLoader();
            try
            {
                loader.LoadFromXMLFile(configPath);
            }
            catch (Exception e)
            {
                //(ログID：014)
                LogUtility.OutputLog("014", configPath, e.Message);
                return false;
            }

            ConnectRetryNum = loader.ReadEntryInt(BPHCConstants.INI_SECTION_APP, BPHCConstants.INI_CONNECT_RETRY_KEY, BPHCConstants.DEFAULT_CONNECT_RETRY);
            ConnectRetryWaitMsec = loader.ReadEntryInt(BPHCConstants.INI_SECTION_APP, BPHCConstants.INI_CONNECT_RETRYWAIT_MSEC_KEY, BPHCConstants.DEFAULT_CONNECT_RETRYWAIT_MSEC);
            ConnectTimeout = loader.ReadEntryInt(BPHCConstants.INI_SECTION_APP, BPHCConstants.INI_CONNECT_TIMEOUT_KEY, BPHCConstants.DEFAULT_CONNECT_TIMEOUT);
            KilledCheckRetryNum = loader.ReadEntryInt(BPHCConstants.INI_SECTION_APP, BPHCConstants.INI_KILLEDCHK_RETRY_KEY, BPHCConstants.DEFAULT_KILLEDCHK_RETRY);
            ServerAddress = loader.ReadEntry(BPHCConstants.INI_SECTION_APP, BPHCConstants.INI_SERVER_ADDRESS_KEY, BPHCConstants.DEFAULT_SERVER_ADDRESS);

            //(ログID：HC013)
            string dbgLog = "";
            dbgLog += "\r\nconnectRetryNum=" + ConnectRetryNum;
            dbgLog += "\r\nconnectRetryWaitMsec=" + ConnectRetryWaitMsec;
            dbgLog += "\r\nconnectTimeout=" + ConnectTimeout;
            dbgLog += "\r\nkilledCheckRetryNum=" + KilledCheckRetryNum;
            dbgLog += "\r\nserverAddress=" + ServerAddress;

            LogUtility.OutputLog("013", dbgLog);

            return true;
        }
        /// <summary>
        /// 接続対象設定ファイルからポート番号をロード
        /// </summary>
        public bool LoadPortNo()
        {
            string configPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\";
            if (ServiceType == BPHCConstants.MODE_BATCH)
            {
                configPath += BPHCConstants.CONF_FOLDER_BATCH + "\\" + BPHCConstants.SETTINGFILE_BATCH;
            }
            else
            {
                configPath += BPHCConstants.CONF_FOLDER_DIRECT + "\\" + BPHCConstants.SETTINGFILE_DIRECT;
            }
            LogUtility.OutputLog("030", configPath);
            XMLLoader loader = new XMLLoader();
            try
            {
                loader.LoadFromXMLFile(configPath);
            }
            catch (Exception e)
            {
                //(ログID：012)
                LogUtility.OutputLog("012", configPath, e.Message);
                return false;
            }

            PortNo = loader.ReadEntryInt(BPHCConstants.INI_SECTION_APP, BPHCConstants.INI_PORTNO_KEY, BPHCConstants.DEFAULT_PORTNO);
            LogUtility.OutputLog("031", PortNo.ToString());

            return true;
        }

    }
}
