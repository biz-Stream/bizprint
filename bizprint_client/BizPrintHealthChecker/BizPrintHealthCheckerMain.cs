using BizPrintCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BizPrintHealthChecker
{
    class BizPrintHealthCheckerMain
    {
        /// <summary>設定マネージャ</summary>
        private static BPHCSettingManager SettingMng;

        static void Main(string[] args)
        {
            //同名同引数プロセス終了処理の呼び出し
            SameProcessChecker spchecker = new SameProcessChecker();
            spchecker.doCheckAndKill(args);


            //ProgramDataフォルダの取得
            string logConfPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            //引数のチェック
            int mode = -1;
            if (args.Length != 1)
            {
                //Error
            }
            else
            {
                if (args[0].Equals(BPHCConstants.PROCESSNAME_BATCH))
                {
                    mode = BPHCConstants.MODE_BATCH;
                    logConfPath += "\\" + BPHCConstants.CONF_FOLDER_BATCH + "\\" + BPHCConstants.LOGCONF_BPHC;
                }
                else if (args[0].Equals(BPHCConstants.PROCESSNAME_DIRECT))
                {
                    mode = BPHCConstants.MODE_DIRECT;
                    logConfPath += "\\" + BPHCConstants.CONF_FOLDER_DIRECT + "\\" + BPHCConstants.LOGCONF_BPHC;
                }
                else
                {
                    //Error
                }
            }
            //argment error!
            if (mode == -1)
            {
                string errorMsg = "Argument Error. num:" + args.Length.ToString() + ",argument:";
                for (int i = 0; i < args.Length; i++)
                {
                    errorMsg += args[i] + " ";
                }
                MessageBox.Show(errorMsg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //ログ4net初期化
            LogUtility.InitLog4Net(logConfPath);
            LogUtility.OutputStaticLog("HC001", CommonConstants.LOGLEVEL_INFO, BPHCConstants.BPHC_Static_logString_001);
            LogUtility.OutputStaticLog("HC002", CommonConstants.LOGLEVEL_DEBUG, BPHCConstants.BPHC_Static_logString_002 + args[0]);


            if (!Init(mode, spchecker))
            {
                //初期化エラー
                //Initialize Error occurred.
                MessageBox.Show("Initialize Error occurred.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                doFinish();
                return;
            }
            //対象プロセス存在チェック
            if (!isProcAlive(0, 0))
            {
                if (mode == BPHCConstants.MODE_DIRECT)
                {
                    //正常終了
                }
                else
                {
                    //BatchPrintService起動
                    //失敗時のMessageBoxはstartBatchPrintService内で表示
                    startBatchPrintService();
                }
                doFinish();
                return;
            }
            //alive問い合わせ
            if (getIsaliveStatus())
            {
                //成功なのでそのまま終了
                doFinish();
                return;
            }
            //無応答だったプロセスを殺すのに失敗したらそこで終わり
            if (!killTargetProcessWithRoop())
            {
                MessageBox.Show(LogUtility.GetLogStr("072", SettingMng.ServiceName()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                doFinish();
                return;
            }
            //プロセス終了後、ダイレクトならそのまま終了
            if (mode == BPHCConstants.MODE_DIRECT)
            {
                //正常終了
                doFinish();
                return;
            }
            else
            {
                //BatchPrintService起動
                //失敗時のMessageBoxはstartBatchPrintService内で表示
                startBatchPrintService();
                doFinish();
                return;
            }
        }

        /// <summary>
        /// 終了ログ出力処理
        /// </summary>
        private static void doFinish()
        {
            LogUtility.OutputStaticLog("HC100", CommonConstants.LOGLEVEL_INFO, BPHCConstants.BPHC_Static_logString_100);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="args"></param>
        static bool Init(int mode, SameProcessChecker checker)
        {

            bool rtn = false;
            //存在チェック
            if (!checkFileExists(mode))
            {
                return false;
            }
            rtn = true;
            //ログファイル読み込み、引数チェック結果ログ出力
            if (mode == BPHCConstants.MODE_DIRECT)
            {
                LogUtility.InitLogUtility(BPHCConstants.LogIDBaseBPHC, BPHCConstants.LogConfBaseBPHC, BPHCConstants.CONF_FOLDER_DIRECT);
                LogUtility.OutputLog("020", BPHCConstants.PROCESSNAME_DIRECT);
            }
            else
            {
                LogUtility.InitLogUtility(BPHCConstants.LogIDBaseBPHC, BPHCConstants.LogConfBaseBPHC, BPHCConstants.CONF_FOLDER_BATCH);
                LogUtility.OutputLog("021", BPHCConstants.PROCESSNAME_BATCH);
            }

            //先行プロセスをkillしていた場合のログ出力
            if (checker.isKilled)
            {
                LogUtility.OutputLog("040", checker.killedID, checker.killedName, checker.killedArg);
            }


            //設定読み込み
            SettingMng = new BPHCSettingManager(mode);
            if (!SettingMng.LoadSetting())
            {
                return false;
            }
            if (!SettingMng.LoadPortNo())
            {
                return false;
            }
            return rtn;
        }

        /// <summary>
        /// 必須ファイルの存在チェック処理
        /// </summary>
        /// <returns></returns>
        private static bool checkFileExists(int mode)
        {
            bool rtn = true;
            string execpath = System.AppDomain.CurrentDomain.BaseDirectory;
            if (mode == BPHCConstants.MODE_DIRECT)
            {
                //exe、設定ファイル
                execpath += BPHCConstants.PROCESSNAME_DIRECT + ".exe";
                string directsettingpath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + BPHCConstants.CONF_FOLDER_DIRECT + "\\" + BPHCConstants.PROCESSNAME_DIRECT + ".xml"; ;
                string ownsettingpath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + BPHCConstants.CONF_FOLDER_DIRECT + "\\" + BPHCConstants.CONFFILE_BPHC;
                if (!System.IO.File.Exists(execpath))
                {
                    rtn = false;
                }
                else if (!System.IO.File.Exists(directsettingpath))
                {
                    rtn = false;
                }
                else if (!System.IO.File.Exists(ownsettingpath))
                {
                    rtn = false;
                }
            }
            else
            {
                execpath += BPHCConstants.PROCESSNAME_BATCH + ".exe";
                string batchtsettingpath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + BPHCConstants.CONF_FOLDER_BATCH + "\\" + BPHCConstants.PROCESSNAME_BATCH + ".xml"; ;
                string ownsettingpath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + BPHCConstants.CONF_FOLDER_BATCH + "\\" + BPHCConstants.CONFFILE_BPHC;
                if (!System.IO.File.Exists(execpath))
                {
                    rtn = false;
                }
                else if (!System.IO.File.Exists(batchtsettingpath))
                {
                    rtn = false;
                }
                else if (!System.IO.File.Exists(ownsettingpath))
                {
                    rtn = false;
                }
            }
            return rtn;
        }
        /// <summary>
        /// ステータス取得対象プロセス存在確認処理
        /// </summary>
        /// <param name="n">ループから呼ばれた際の回数</param>
        /// <param name="m">ループから呼ばれた際の総回数</param>
        /// <returns></returns>
        private static bool isProcAlive(int n, int m)
        {
            string logarg = "";
            if (n == 0 && m == 0)
            {
            }
            else
            {
                logarg += "(" + n + "/" + m + ")";
            }
            string procname = SettingMng.ServiceName();
            int procnum = System.Diagnostics.Process.GetProcessesByName(procname).Length;
            if (procnum > 0)
            {
                LogUtility.OutputLog("050", procname, logarg);
                return true;
            }
            else
            {
                LogUtility.OutputLog("051", procname, logarg);
                return false;
            }
        }
        /// <summary>
        /// 対象プロセス強制終了処理
        /// </summary>
        /// <returns></returns>
        private static bool killTargetProcessWithRoop()
        {
            bool rtn = false;
            string procname = SettingMng.ServiceName();
            LogUtility.OutputLog("070", procname);

            Process[] killAllProcs = Process.GetProcessesByName(procname);
            //プロセスを殺すループ
            foreach (Process killp in killAllProcs)
            {
                try
                {
                    SameProcessChecker.killProcById(killp.Id);
                }
                catch { }
            }
            //チェックリトライループ
            for (int i = 0; i < SettingMng.KilledCheckRetryNum + 1; i++)
            {
                if (i != 0) {
                    Thread.Sleep(110);
                }
                //残プロセスがあるかチェック
                if (!isProcAlive(i + 1, SettingMng.KilledCheckRetryNum + 1))
                {
                    rtn = true;
                    break;
                }
            }
            //繰り返し回数最後までプロセスが残っていたら失敗
            if (rtn)
            {
                LogUtility.OutputLog("071", procname);
            }
            else
            {
                LogUtility.OutputLog("072", procname);
            }
            return rtn;
        }
        /// <summary>
        /// バッチ印刷起動処理
        /// </summary>
        /// <returns></returns>
        private static bool startBatchPrintService()
        {
            bool rtn = false;
            bool exceptionoccured = false;

            //アプリケーション起動先のプロセス
            Process pBatchService = null;
            Mutex bphcMutex = null;
            string execpath = System.AppDomain.CurrentDomain.BaseDirectory + BPHCConstants.PROCESSNAME_BATCH + ".exe";

            try
            {
                bphcMutex = new Mutex(false, "StartBatchPrintServiceByBPHC");
                bphcMutex.WaitOne();
                try
                {
                    pBatchService = Process.Start(execpath);
                }
                catch (Exception err)
                {
                    //起動に失敗
                    LogUtility.OutputLog("081", err.Message);
                    MessageBox.Show(LogUtility.GetLogStr("081", err.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    exceptionoccured = true;
                }
                if (!exceptionoccured)
                {
                    LogUtility.OutputLog("080");
                    rtn = true;
                }

            }
            finally
            {
                if (bphcMutex != null)
                {
                    bphcMutex.ReleaseMutex();
                    bphcMutex.Close();
                }

            }

            return rtn;
        }
        /// <summary>
        /// isaliveに接続しステータスを取得する
        /// </summary>
        /// <returns></returns>
        private static bool getIsaliveStatus()
        {
            string connectURL = "http://" + SettingMng.ServerAddress + ":" + SettingMng.PortNo.ToString() + "/isalive";
            LogUtility.OutputLog("060", connectURL);
            bool rtn = false;
            bool isSendSuccessed = false;
            for (int i = 0; i < SettingMng.ConnectRetryNum + 1; i++)
            {
                if (i != 0) {
                    Thread.Sleep(SettingMng.ConnectRetryWaitMsec);
                }
                HttpWebRequest req = null;
                HttpWebResponse httpres = null;

                string sendStr = "isalive";
                byte[] sendByte = System.Text.Encoding.ASCII.GetBytes(sendStr);


                try
                {
                    req = HttpWebRequest.Create(connectURL) as HttpWebRequest;
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = sendByte.Length;
                    req.Timeout = SettingMng.ConnectTimeout;
                    req.ServicePoint.Expect100Continue = false;//チェックのための接続をしない
                    using (Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(sendByte, 0, sendByte.Length);
                        reqStream.Close();
                        httpres = req.GetResponse() as HttpWebResponse;
                        HttpStatusCode rtnCode = httpres.StatusCode;
                        //ここまで来て例外が発生しなければ200で成功
                        Stream resStream = httpres.GetResponseStream();
                        using (StreamReader sr = new StreamReader(resStream, Encoding.UTF8))
                        {
                            //バッファ内部をクリアする為
                            string rcvstr = sr.ReadToEnd();
                        }
                        LogUtility.OutputLog("061", rtnCode.ToString());
                        rtn = true;
                        isSendSuccessed = true;
                    }
                }
                catch (System.Net.WebException we)
                {
                    String msg = we.Message;
                    if (we.Status == WebExceptionStatus.Timeout)
                    {
                        //TimeoutError
                        if (SettingMng.ConnectRetryNum - i == 0)
                        {
                            LogUtility.OutputLog("067", connectURL,  (SettingMng.ConnectRetryNum - i).ToString());
                        }
                        else {
                            LogUtility.OutputLog("064", connectURL, SettingMng.ConnectRetryWaitMsec.ToString(), (SettingMng.ConnectRetryNum - i).ToString());
                        }
                    }
                    else if (we.Status == WebExceptionStatus.ProtocolError)
                    {
                        //500が帰って来た
                        isSendSuccessed = true;
                        LogUtility.OutputLog("062", "500");
                    }
                    else
                    {
                        //その他のステータス
                        if (SettingMng.ConnectRetryNum - i == 0)
                        {
                            LogUtility.OutputLog("068", connectURL, (SettingMng.ConnectRetryNum - i).ToString(), msg);
                        }
                        else { 
                            LogUtility.OutputLog("065", connectURL, SettingMng.ConnectRetryWaitMsec.ToString(), (SettingMng.ConnectRetryNum - i).ToString(), msg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //その他のエラー
                    string msg = ex.GetType().ToString();
                    msg += ":" + ex.Message;
                    if (SettingMng.ConnectRetryNum - i == 0)
                    {
                        LogUtility.OutputLog("068", connectURL, (SettingMng.ConnectRetryNum - i).ToString(), msg);
                    }
                    else
                    {
                        LogUtility.OutputLog("065", connectURL, SettingMng.ConnectRetryWaitMsec.ToString(), (SettingMng.ConnectRetryNum - i).ToString(), msg);
                    }
                }
                finally
                {
                    //基本的には消す必要は無いはずだが保険
                    if (req != null)
                    {
                        req.Abort();
                    }
                    if (httpres != null)
                    {
                        httpres.Close();
                    }
                    LogUtility.OutputLog("063", connectURL);
                }

                if (isSendSuccessed)
                {
                    break;
                }
            }
            //ループ終了時送信成功していなければリトライ最大まで失敗と判断
            if (!isSendSuccessed)
            {
                LogUtility.OutputLog("066", connectURL);
            }

            return rtn;
        }
    }
}
