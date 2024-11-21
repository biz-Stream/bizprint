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
using BizPrintCommon;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace SilentPdfPrinter
{
    /// <summary>
    /// SilentPdfPrinter処理メインクラス
    /// </summary>
    class SilentPdfPrinterMain
    {

        /// <summary>設定マネージャ</summary>
        private static SettingManagerOfSilent SettingMng;
        /// <summary>SPPファイルフルパス</summary>
        private static string SppFilePath = "";
        /// <summary>送信バイトデータ</summary>
        private static byte[] SendByte = null;
        /// <summary>強制終了を行ったかのフラグ</summary>
        private static bool killedFlg = false;


        private static bool messegeShown = false;
        /// <summary>
        /// SilentPdfPrinterメイン
        /// </summary>
        /// <param name="args">起動引数</param>
        static void Main(string[] args)
        {
            //処理継続のためのエラーフラグ
            bool isError = false;


            //ログ出力初期化と開始ログ出力
            string logConfPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + SilentConstants.ConfFolderName + "\\" + SilentConstants.LogConfFileName;
#if DEBUG
            logConfPath = "C:\\ProgramData\\brainsellers\\DirectPrint\\SilentPdfPrinter_logConfig.xml";
#endif
            LogUtility.InitLog4Net(logConfPath);
            LogUtility.OutputStaticLog("SI001", CommonConstants.LOGLEVEL_INFO, SilentConstants.Silent_Static_logString_001 + " Proccess count=" + getSilentProcessNum().ToString());
            Version OSver = Environment.OSVersion.Version;
            //初期化ログ
            LogUtility.OutputStaticLog("SI000", CommonConstants.LOGLEVEL_INFO, CommonConstants.STATIC_LOG_001 + System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion() + " [" + Environment.Version.ToString() + "] " + LogUtility.GetWindowsVersion());
            LogUtility.OutputStaticLog("SI000", CommonConstants.LOGLEVEL_INFO, CommonConstants.STATIC_LOG_002 + System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() + " Excecute File Version=" + LogUtility.GetSelfFileVersion());
            LogUtility.OutputStaticLog("SI000", CommonConstants.LOGLEVEL_INFO, CommonConstants.STATIC_LOG_003 + System.Runtime.InteropServices.RuntimeEnvironment.SystemConfigurationFile);
            //初期化処理
            if (!Init(args))
            {
                //初期化失敗終了
                isError = true;
            }

            //送信ファイル読み込み
            if (!isError && !LoadSppFile())
            {
                //読み込み失敗終了
                isError = true;
            }
            if (!isError)
            {
                while (!isCanConnectNow())
                {
                    if (messegeShown)
                    {
                        //並列起動数オーバーエラー終了
                        isError = true;
                        break;
                    }
                    Thread.Sleep(SettingMng.WaitMsec);
                }
            }
            //DirectPrintService起動確認、起動してなければ起動
            if (!isError && !DirectServiceStartChk(false))
            {
                //サービス起動失敗終了
                isError = true;
            }
            //送信
            if (!isError && !SendSppFile())
            {
                //送信失敗終了
                isError = true;
            }

            //終了処理
            //sppファイル削除
            if (SettingMng.IsDeleteFile)
            {
#if DEBUG
                //読み取り専用解除テスト用
                //File.SetAttributes(sppPath, FileAttributes.ReadOnly);
#endif
                if (!FileDeleter.DeleteFile(SppFilePath))
                {
                    //削除に失敗。(ログは出すが、何もできない)
                    isError = true;
                }
            }
            else
            {
                LogUtility.OutputLog("030");
            }

            //終端ログ
            LogUtility.OutputLog("035");

        }
        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="args">メインの引数をそのまま渡す</param>
        /// <returns>初期化成功時:true</returns>
        static bool Init(string[] args)
        {

            //ログファイル読み込み
            LogUtility.InitLogUtility(SilentConstants.LogIDBaseSilent, SilentConstants.LogConfBaseSilent, SilentConstants.ConfFolderName);

            //設定ファイル読み込み
            SettingMng = new SettingManagerOfSilent();

            if (!SettingMng.LoadSetting())
            {
                //設定ファイル読み込みに失敗
                return false;
            }

            //引数チェック
            if (!CheckArgument(args))
            {
                //引数チェックに失敗
                return false;

            }
            SppFilePath = args[0];

            //起動元プロセス名を取得し、記録する
            SilentConstants.ParentProcessName = ProcessNameChecker.GetParentModuleName();

            LogUtility.OutputLog("016", SilentConstants.ParentProcessName);

            return true;
        }
        /// <summary>
        /// 引数チェック処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static bool CheckArgument(string[] args)
        {
            //引数の数チェック
            if (args.Length != 1)
            {
                LogUtility.OutputLog("015", args.Length.ToString());
                MessageBox.Show(LogUtility.GetLogStr("015", args.Length.ToString()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //引数に渡されたファイルの存在チェック
            if (!System.IO.File.Exists(args[0]))
            {
                //NG 
                //存在しない場合のエラー
                LogUtility.OutputLog("019", args[0]);
                MessageBox.Show(LogUtility.GetLogStr("019", args[0]), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            SppFilePath = args[0];

            return true;

        }
        /// <summary>
        /// SPPファイル読み込み
        /// </summary>
        /// <returns>true:成功、false:失敗</returns>
        private static bool LoadSppFile()
        {
            FileStream fStream = null;
            BinaryReader binReaderr = null;
            try
            {
                fStream = new FileStream(SppFilePath, FileMode.Open, FileAccess.Read);
                binReaderr = new BinaryReader(fStream);
                SendByte = binReaderr.ReadBytes((int)fStream.Length);
            }
            catch (Exception err)
            {
                LogUtility.OutputLog("019", SppFilePath, err.Message);
                return false;
            }
            finally
            {
                if (binReaderr != null) binReaderr.Close();
                if (fStream != null) fStream.Close();
            }
            LogUtility.OutputLog("020", SppFilePath);
            LogUtility.OutputLog("021", SendByte.Length.ToString());
            return true;
        }
        /// <summary>
        /// サービス起動チェックと起動処理
        /// </summary>
        /// <returns>true:成功、false:失敗</returns>
        private static bool DirectServiceStartChk(bool isRetry)
        {

            int processCount = 0;
            //接続先ホストが、localhost or 127.0.0.1 or ::1でない場合、起動はせずに必ずtrueを返す
            if (!(0 == String.Compare(SilentConstants.DEFAULT_DIRECT_PRINT_HOST, SettingMng.DirectPrintHostName, true)) && !SettingMng.DirectPrintHostName.Equals(SilentConstants.DEFAULT_DIRECT_PRINT_IP) && !SettingMng.DirectPrintHostName.Equals(SilentConstants.DEFAULT_DIRECT_PRINT_IPV6))
            {
                return true;
            }

            //アプリケーション起動先のプロセス
            Process pDirectService = null;

            Mutex silentMutex = null;
            try
            {
                silentMutex = new Mutex(false, SilentConstants.SERVICE_MUTEX_START);
                silentMutex.WaitOne();

                try
                {
                    //起動済みチェック
                    processCount = Process.GetProcessesByName(SettingMng.ProcessName).Length;
                }
                catch (Exception err)
                {
                    string errstr = err.ToString();
                    return false;
                }
                if (processCount == 0)
                {
                    LogUtility.OutputLog("010", null);
                    string drectAppPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + SilentConstants.SERVICE_EXE_NAME;
#if DEBUG
                    drectAppPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\brainsellers\\DirectPrint\\" + SilentConstants.SERVICE_EXE_NAME ;
#endif
                    try
                    {
                        pDirectService = Process.Start(drectAppPath);
                    }
                    catch (Exception err)
                    {
                        //起動に失敗
                        LogUtility.OutputLog("014", err.Message);
                        MessageBox.Show(LogUtility.GetLogStr("014", err.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return false;
                    }
                    LogUtility.OutputLog("013", null);
                }
                else
                {
                    //すでに起動済み
                    LogUtility.OutputLog("011", null);
                    //しかし接続できなかったという場合なので、元のプロセスを殺す
                    if (isRetry)
                    {
                        //SilentPdfPrinterが複数いる状況で終了させてはいけない
                        System.Diagnostics.Process[] silentAll = System.Diagnostics.Process.GetProcessesByName("SilentPdfPrinter");
                        if (silentAll.Length < 2)
                        {
                            System.Diagnostics.Process[] dppsAll = System.Diagnostics.Process.GetProcessesByName(SettingMng.ProcessName);
                            foreach (System.Diagnostics.Process ps in dppsAll)
                            {
                                //プロセスを強制的に終了させる
                                ps.Kill();
                            }
                            killedFlg = true;
                        }
                    }
                    else
                    {
                        killedFlg = false;
                    }

                }


            }
            finally
            {
                if (silentMutex != null)
                {
                    silentMutex.ReleaseMutex();
                    silentMutex.Close();
                }

            }
            return true;
        }
        /// <summary>
        ///  ファイルの送信実行
        /// </summary>
        /// <returns>true:成功、false:失敗</returns>
        private static bool SendSppFile()
        {
            bool rtn = true;
            bool isRetry = false;

            string parenStr = "parent=" + SilentConstants.ParentProcessName + "&sppdata=";
            byte[] parentByte = System.Text.Encoding.ASCII.GetBytes(parenStr);

            string printURL = string.Format(SilentConstants.DIRECT_SERVICE_URL, SettingMng.DirectPrintHostName, SettingMng.PortNo.ToString());
            int retryCount = SettingMng.RetryCount + 1;

            LogUtility.OutputLog("022", printURL);

            bool sendSucess = false;

            while (retryCount > 0 && !sendSucess)
            {
                if (retryCount != (SettingMng.RetryCount + 1))
                {
                    Thread.Sleep(SettingMng.RetryInterval);
                }
                //リトライの場合はここで起動する可能性がある。起動失敗の場合はエラー終了
                if (isRetry)
                {
                    if (!DirectServiceStartChk(!killedFlg))
                    {
                        break;
                    }
                }
                HttpWebRequest req = null;
                HttpWebResponse httpres = null;
                try
                {
                    req = HttpWebRequest.Create(printURL) as HttpWebRequest;
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = parentByte.Length + SendByte.Length;
                    req.Timeout = SettingMng.ConnectTimeout;
                    req.ServicePoint.Expect100Continue = false;//チェックのための接続をしない
                    LogUtility.OutputLog("025", req.ContentLength.ToString());
                    using (Stream reqStream = req.GetRequestStream())
                    {
                        LogUtility.OutputLog("026");
                        reqStream.Write(parentByte, 0, parentByte.Length);
                        reqStream.Write(SendByte, 0, SendByte.Length);
                        reqStream.Close();

                        httpres = req.GetResponse() as HttpWebResponse;
                        Stream resStream = httpres.GetResponseStream();
                        using (StreamReader sr = new StreamReader(resStream, Encoding.UTF8))
                        {
                            string html = sr.ReadToEnd();
                            if (httpres.StatusCode == HttpStatusCode.OK)
                            {
                                LogUtility.OutputLog("027", HttpStatusCode.OK.ToString());
                                sendSucess = true;
                            }
                            else
                            {
                                LogUtility.OutputLog("028", httpres.StatusCode.ToString());
                                isRetry = true;
                            }
                            break;
                        }
                    }
                }
                catch (System.Net.WebException we)
                {
                    String errStr = we.Message;
                    if (we.Status == WebExceptionStatus.Timeout)
                    {
                        //タイムアウトエラー
                        if (retryCount == 1)
                        {
                            LogUtility.OutputLog("042", printURL, (retryCount - 1).ToString(), errStr);
                        }
                        else
                        {
                            LogUtility.OutputLog("023", printURL, SettingMng.RetryInterval.ToString(), (retryCount - 1).ToString(), errStr);
                        }
                        rtn = false;
                        isRetry = true;
                    }
                    else
                    {
                        //その他のエラー
                        if (retryCount == 1)
                        {
                            LogUtility.OutputLog("043", printURL, (retryCount - 1).ToString(), errStr);
                        }
                        else
                        {
                            LogUtility.OutputLog("036", printURL, SettingMng.RetryInterval.ToString(), (retryCount - 1).ToString(), errStr);
                        }
                        rtn = false;
                        isRetry = true;
                    }
                }
                catch (Exception e)
                {
                    //それ以外
                    string errStr = e.Message;
                    if (retryCount == 1)
                    {
                        LogUtility.OutputLog("043", printURL, (retryCount - 1).ToString(), errStr);
                    }
                    else
                    {
                        LogUtility.OutputLog("036", printURL, SettingMng.RetryInterval.ToString(), (retryCount - 1).ToString(), errStr);
                    }
                    rtn = false;
                    isRetry = true;
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
                    LogUtility.OutputLog("029", null);
                }

                retryCount--;
            }
            if (retryCount == 0)
            {
                LogUtility.OutputLog("024", printURL);
                MessageBox.Show(LogUtility.GetLogStr("024", printURL), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return rtn;
        }

        /// <summary>
        /// SilentPdfPrinterのプロセスが何個平行起動しているかを確認する
        /// </summary>
        /// <returns></returns>
        private static int getSilentProcessNum()
        {
            string thisName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            System.Diagnostics.Process[] silentAll = System.Diagnostics.Process.GetProcessesByName(thisName);
            return silentAll.Length;
        }

        /// <summary>
        /// 現在同時起動されている同名プロセスをチェックし、自身より前に起動されているものが同時接続数より少ないかを返す
        /// </summary>
        /// <returns></returns>
        private static bool isCanConnectNow()
        {
            //自分のID取得
            Process thisProc = System.Diagnostics.Process.GetCurrentProcess();
            int thisID = thisProc.Id;
            string thisName = thisProc.ProcessName;
            DateTime thisStart = thisProc.StartTime;
            //ID一覧取得
            System.Diagnostics.Process[] silentAll = System.Diagnostics.Process.GetProcessesByName(thisName);
            //全数が同時接続数より少ないのでtrue
            if (silentAll.Length <= SettingMng.ConcurrentConnectionsMax)
            {
                LogUtility.OutputLog("038", thisID.ToString());
                return true;
            }
            if (chkCount == 0 && silentAll.Length > SettingMng.MaxProccessCount && !messegeShown)
            {
                LogUtility.OutputLog("040", SettingMng.MaxProccessCount.ToString());
                MessageBox.Show("SilentPdfPrinterが" + SettingMng.MaxProccessCount.ToString() + "個以上起動しています。処理を中断します。", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                messegeShown = true;
                return false;
            }
            chkCount++;

            int numYoungerP = 0;
            for (int i = 0; i < silentAll.Length; i++) {
                if (silentAll[i].Id == thisID) {
                    continue;
                }
                DateTime pStartTime = silentAll[i].StartTime;
                if (DateTime.Compare(pStartTime,thisStart) < 0) {
                    numYoungerP++;
                }
            }

            //自身よりStartTimeが速いものがConcurrentConnectionsMaxより多い場合false
            if (numYoungerP >= SettingMng.ConcurrentConnectionsMax) {
                LogUtility.OutputLog("039", numYoungerP.ToString(), thisID.ToString());
                return false;
            }

            LogUtility.OutputLog("038", thisID.ToString());
            return true;
        }
        private static int chkCount = 0;

    }
}
