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
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

namespace BizPrintCommon
{
    /// <summary>
    /// ログ出力クラス。Log4Netを使用して静的出力を行う。
    /// </summary>
    public class LogUtility
    {
        /// <summary>
        /// 内容ファイルロード済みフラグ
        /// </summary>
        public static bool IsLoaded { private set; get; } = false;
        /// <summary>
        /// Log4Net宣言
        /// </summary>
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LogUtility(string confpath)
        {
            if (!IsLoaded)
            {
                //ログ設定ファイルを指定して読み込む。これをしないと、App.config固定か、AssemblyInfo.csで指定した固定パスになるため。
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(confpath));
                if (logger.Logger.Repository.Configured)
                {
                    IsLoaded = true;
                }
                else
                {
                    //設定ファイル読み込みに失敗し、初期化できずにログ出力できない状態。
                    //ログ出力できませんよ、というメッセージ以外に取れる手段がない。
                    MessageBox.Show(String.Format(CommonConstants.ERR_MSG_LOGSETTING, confpath), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        /// <summary>
        /// Log4Netの初期化
        /// </summary>
        /// <param name="confpath">Log4Netのコンフィグファイルパス</param>
        public static void InitLog4Net(string confpath)
        {
            if (!IsLoaded)
            {
                //ログ設定ファイルを指定して読み込む。これをしないと、App.config固定か、AssemblyInfo.csで指定した固定パスになるため。
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(confpath));
                if (logger.Logger.Repository.Configured)
                {
                    IsLoaded = true;
                }
                else
                {
                    //設定ファイル読み込みに失敗し、初期化できずにログ出力できない状態。
                    //ログ出力できませんよ、というメッセージ以外に取れる手段がない。
                    MessageBox.Show(String.Format(CommonConstants.ERR_MSG_LOGSETTING, confpath), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
        /// <summary>
        /// ログデータ構造体
        /// </summary>
        private struct logData
        {
            public int logLevel;
            public string logStr;

        }
        /// <summary>使用するモジュールごとのヘッダベース文字列</summary>
        private static string LogIdBase = "";
        /// <summary>ログデータファイルパス</summary>
        private static string LogConfFilePath = "";
        /// <summary>読み込んだログデータを保持するDictionary</summary>
        private static Dictionary<string, logData> LogDatas = new Dictionary<string, logData>();
        /// <summary>
        /// ログ出力初期化。固定ログはこれを行わなくても出力可能
        /// </summary>
        /// <param name="idBase">各ログの先頭2文字</param>
        /// <param name="baseFileName">固定名</param>
        /// <param name="compName">中間フォルダパス</param>
        public static void InitLogUtility(string idBase, string baseFileName, string compName)
        {
            //初期化
            LogIdBase = idBase;
            LogConfFilePath = GetlogDataFileFullPath(baseFileName, compName);
            LoadLogData();

        }
        /// <summary>
        /// 言語設定を取得して、読み込むログファイル名を決定する
        /// </summary>
        /// <param name="baseFileName"></param>
        /// <param name="compName"></param>
        /// <returns></returns>
        private static string GetlogDataFileFullPath(string baseFileName, string compName)
        {
            //言語設定の取得
            string filePath = "";
            System.Globalization.CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            string cultureName = currentCulture.Name;
            string loadPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + compName + "\\";

            string chkPath = loadPath + baseFileName + cultureName + CommonConstants.CONF_EXT;
            OutputStaticLog(LogIdBase + "004", CommonConstants.LOGLEVEL_DEBUG, string.Format(CommonConstants.Static_logString_002, cultureName, chkPath));
            //言語設定に対応したファイルの存在チェック
            if (System.IO.File.Exists(chkPath))
            {
                filePath = chkPath;
            }
            else
            {
                //存在しない場合のデフォルト(ログID：007)
                filePath = loadPath + baseFileName + CommonConstants.JA_JP_DEFAULT + CommonConstants.CONF_EXT;
                OutputStaticLog(LogIdBase + "007", CommonConstants.LOGLEVEL_WARN, "file " + chkPath + " can not read. use ja-JP.");
            }
            OutputStaticLog(LogIdBase + "005", CommonConstants.LOGLEVEL_DEBUG, string.Format(CommonConstants.Static_logString_005, filePath));
            return filePath;
        }
        /// <summary>
        /// ログ設定ファイルからログ内容を読み込み、保持する
        /// </summary>
        private static void LoadLogData()
        {

            // Mutexで他からのアクセスをさせない
            String strMutexName = LogConfFilePath.Replace(System.IO.Path.DirectorySeparatorChar, '_');
            System.Threading.Mutex LogMutex;

            LogMutex = new System.Threading.Mutex(false, strMutexName);
            LogMutex.WaitOne();
            StreamReader streamRead;
            try
            {
                streamRead = new StreamReader(LogConfFilePath, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                //ファイルが存在していない、ディレクトリが無い
                string errstr = ex.Message;
                OutputStaticLog(LogIdBase + "008", CommonConstants.LOGLEVEL_WARN, string.Format(CommonConstants.Static_logString_006, LogConfFilePath));

                return;
            }
            SetLogData(streamRead);
            streamRead.Close();

            LogMutex.ReleaseMutex();
            LogMutex.Close();
        }
        /// <summary>
        /// ログ情報をディクショナリに分解して保持する
        /// </summary>
        /// <param name="sr"></param>
        private static void SetLogData(StreamReader sr)
        {
            int addedNum = 0;
            LogDatas.Clear();
            while (sr.Peek() > 0)
            {
                string stBuffer = sr.ReadLine();
                if (stBuffer.Length > 0)
                {
                    string[] splitted = stBuffer.Split(',');
                    if (splitted.Length < 3)
                    {
                        //errordata 少ないのは無視する。4つめ以降の区切られたデータも無視
                    }
                    else
                    {
                        logData newData;
                        newData.logLevel = chgLogLevel(splitted[1]);
                        newData.logStr = splitted[2];
                        LogDatas.Add(splitted[0], newData);
                        addedNum++;
                    }
                }
            }
            if (addedNum == 0)
            {
                //全てのログ出力が空白文字、LEVEL INFO 
                OutputStaticLog(LogIdBase + "008", CommonConstants.LOGLEVEL_WARN, string.Format(CommonConstants.Static_logString_006, LogConfFilePath));
            }
            else
            {
                //読み込んだ数を出力
                OutputStaticLog(LogIdBase + "006", CommonConstants.LOGLEVEL_DEBUG, string.Format(CommonConstants.Static_logString_004, addedNum));
            }

        }
        /// <summary>
        ///  大文字小文字を区別せず、ログレベルを数値にして返す
        /// </summary>
        /// <param name="leve;">レベル文字列</param>
        /// <return>ログレベル</return>
        private static int chgLogLevel(string level)
        {
            int rtn;
            if (String.Compare(level, "FATAL", true) == 0)
            {
                rtn = CommonConstants.LOGLEVEL_FATAL;
            }
            else if (String.Compare(level, "ERROR", true) == 0)
            {
                rtn = CommonConstants.LOGLEVEL_ERROR;
            }
            else if (String.Compare(level, "WARN", true) == 0)
            {
                rtn = CommonConstants.LOGLEVEL_WARN;
            }
            else if (String.Compare(level, "INFO", true) == 0)
            {
                rtn = CommonConstants.LOGLEVEL_INFO;
            }
            else if (String.Compare(level, "DEBUG", true) == 0)
            {
                rtn = CommonConstants.LOGLEVEL_DEBUG;
            }
            else
            {
                rtn = CommonConstants.LOGLEVEL_INFO;
            }
            return rtn;
        }
        /// <summary>
        /// 与えられたIDから、ログ内容を取得する。対応するIDが無かった場合は、INFO/空文字
        /// </summary>
        /// <param name="logID"></param>
        /// <returns></returns>
        private static logData getlogData(string logID)
        {
            logData gettedLogData = new logData();
            gettedLogData.logLevel = CommonConstants.LOGLEVEL_INFO;
            gettedLogData.logStr = "";
            //対応するIDが存在しない場合は、INFO/空文字で出力する
            if (LogDatas.ContainsKey(logID))
            {
                gettedLogData = LogDatas[logID];
            }

            return gettedLogData;
        }

        /// <summary>
        /// IDからログ文字列を検索して、置換結果の文字列を対応したログレベルで出力する
        /// </summary>
        /// <param name="logID"></param>
        /// <param name="chgstr"></param>
        public static void OutputLog(string logID, params string[] chgstr)
        {
            if (!IsLoaded)
            {
                return;
            }

            logData logData = getlogData(LogIdBase + logID);
            string afterchged = logData.logStr;
            if (chgstr != null && chgstr.Length != 0)
            {
                //文字列{n}の置き換え(formatを使用しないのは、隠しでエラーthrow結果を出せる箇所を作るため)
                for (int i = 0; i < chgstr.Length; i++)
                {
                    string oldSet = "{" + i.ToString() + "}";
                    afterchged = afterchged.Replace(oldSet, chgstr[i]);
                }
            }
            OutputStaticLog(LogIdBase + logID, logData.logLevel, afterchged);
        }
        /// <summary>
        /// IDからログ文字列を検索して、置換結果の文字列を返す
        /// </summary>
        /// <param name="logID"></param>
        /// <param name="chgstr"></param>
        /// <returns></returns>
        public static string GetLogStr(string logID, params string[] chgstr)
        {
            if (!IsLoaded)
            {
                return logID;
            }

            logData logData = getlogData(LogIdBase + logID);
            string afterchged = logData.logStr;
            if (chgstr != null && chgstr.Length != 0)
            {
                //文字列{n}の置き換え(formatを使用しないのは、隠しでエラーthrow結果を出せる箇所を作るため)
                for (int i = 0; i < chgstr.Length; i++)
                {
                    string oldSet = "{" + i.ToString() + "}";
                    afterchged = afterchged.Replace(oldSet, chgstr[i]);
                }
            }
            return afterchged;
        }
        /// <summary>
        /// デバッグログの出力
        /// </summary>
        /// <param name="logID"></param>
        /// <param name="chgstr"></param>
        public static void OutputDebugLog(string logID, params string[] chgstr)
        {
#if DEBUG
            OutputStaticLog(LogIdBase + logID, CommonConstants.LOGLEVEL_DEBUG, " Called Debuglog");

#endif


            logData logData = getlogData(LogIdBase + logID);
            //IDに対応する文字列が無い場合は出力しない
            if (logData.logStr.Length == 0)
            {
                return;
            }
            string afterchged = logData.logStr;
            if (chgstr != null && chgstr.Length != 0)
            {
                //文字列{n}の置き換え(formatを使用しないのは、隠しでエラーthrow結果を出せる箇所を作るため)
                for (int i = 0; i < chgstr.Length; i++)
                {
                    string oldSet = "{" + i.ToString() + "}";
                    afterchged = afterchged.Replace(oldSet, chgstr[i]);
                }
            }
            OutputStaticLog(LogIdBase + logID, CommonConstants.LOGLEVEL_DEBUG, afterchged);

        }
        /// <summary>
        /// ログをレベルに応じて出力する ログ設定ファイル読み込み前にも固定値を使って出力できる
        /// </summary>
        /// <param name="logID"></param>
        /// <param name="logLevel"></param>
        /// <param name="logStr"></param>
        public static void OutputStaticLog(string logID, int logLevel, string logStr)
        {
            if (!IsLoaded)
            {
                return;
            }

            string outStr = logID + " " + logStr;
            switch (logLevel)
            {
                case CommonConstants.LOGLEVEL_FATAL:
                    logger.Fatal(outStr);
                    break;
                case CommonConstants.LOGLEVEL_ERROR:
                    logger.Error(outStr);
                    break;
                case CommonConstants.LOGLEVEL_WARN:
                    logger.Warn(outStr);
                    break;
                case CommonConstants.LOGLEVEL_INFO:
                    logger.Info(outStr);
                    break;
                case CommonConstants.LOGLEVEL_DEBUG:
                    logger.Debug(outStr);
                    break;
            }
        }
        /// <summary>
        /// Windowsのバージョンを示す文字列を返す
        /// </summary>
        /// <returns></returns>
        public static string GetWindowsVersion()
        {
            System.Management.ManagementClass mc = new System.Management.ManagementClass("Win32_OperatingSystem");
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            String version = "";

            foreach (System.Management.ManagementObject mo in moc)
            {
                version = string.Format("OS=[{0}] [{1}] [{2}]", mo["Caption"], mo["Version"], mo["CSDVersion"]);
                break;
            }
            moc.Dispose();
            mc.Dispose();
            return version;
        }
        /// <summary>
        /// 自身のファイルバージョンを取得する
        /// </summary>
        /// <returns></returns>
        public static string GetSelfFileVersion() {
            System.Diagnostics.FileVersionInfo ver =
            System.Diagnostics.FileVersionInfo.GetVersionInfo(
            System.Reflection.Assembly.GetExecutingAssembly().Location);
            return ver.ProductVersion;
        }



    }
}
