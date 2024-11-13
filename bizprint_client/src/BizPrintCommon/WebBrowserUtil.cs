using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BizPrintCommon
{
    /// <summary>
    /// ブラウザへの結果リターン関連処理クラス
    /// </summary>
    public static class WebBrowserUtil
    {
        /// <summary>
        /// 設定ファイル管理
        /// </summary>
        public static SettingManeger SettingMng { set; get; }
        /// <summary>
        /// doresponceアクセスURL原型
        /// </summary>
        public const string FORMAT_SEND_DATA = @"http://localhost:{0}/doresponse${1}${2}${3}${4}";

        /// <summary>
        /// IEの起動用名
        /// </summary>
        private const string IEXPLORE_PROCESSNAME = "iexplore";
        /// <summary>
        /// Edgeの起動用ID
        /// </summary>
        private const string EDGE_ID = "Microsoft.MicrosoftEdge_8wekyb3d8bbwe!MicrosoftEdge";

        /// <summary>
        /// google chromeの起動用名
        /// </summary>
        private const string CHROME_NAME = "chrome";
        /// <summary>
        /// FireFoxの起動用名
        /// </summary>
        private const string FIRE_FOX_NAME = "firefox";
        /// <summary>
        /// 置換対象タグ開始・終了文字列
        /// </summary>
        private const string SPTAG_START_END = "$$$";
        /// <summary>
        /// 置換対象タグ
        /// </summary>
        private const string SPTAG_URL = "REPLACE_url";
        private const string SPTAG_PARAM = "REPLACE_param";
        private const string SPTAG_SBL_NS = "REPLACE_sbl_ns";
        private const string SPTAG_TARGET = "REPLACE_target";

        /// <summary>
        /// JOBIDなしのレスポンス
        /// </summary>
        /// <param name="brprocessname"></param>
        /// <param name="respUrl"></param>
        /// <param name="target"></param>
        /// <param name="ErrorCode"></param>
        /// <returns></returns>
        public static int OpenResponceNoID(string brprocessname, string respUrl, string target, int ErrorCode)
        {
            int rtn = ErrCodeAndmErrMsg.STATUS_OK;
            if (SettingMng.ServiceType != CommonConstants.MODE_DIRECT)
            {
                //バッチの場合はブラウザを開く処理はすべて無効
                return ErrCodeAndmErrMsg.STATUS_OK;
            }
            if (respUrl == null || respUrl.Length == 0)
            {
                LogUtility.OutputLog("156");
                return ErrCodeAndmErrMsg.STATUS_OK;
            }
            bool haveTarget = (target != null) && (target.Length != 0);
            string url = CreateOpenURL(brprocessname, respUrl, target, ErrorCode, "");
            if (!url.Equals(""))
            {
                rtn = OpenUrlByBrowser(url, brprocessname, haveTarget);
            }

            return rtn;
        }
        /// <summary>
        /// JOBIDを含めたレスポンス
        /// </summary>
        /// <param name="brprocessname"></param>
        /// <param name="respUrl"></param>
        /// <param name="target"></param>
        /// <param name="ErrorCode"></param>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public static int OpenResponceWithID(string brprocessname, string respUrl, string target, int ErrorCode, string jobId)
        {
            if (SettingMng.ServiceType != CommonConstants.MODE_DIRECT)
            {
                //バッチの場合はブラウザを開く処理はすべて無効
                return ErrCodeAndmErrMsg.STATUS_OK;
            }
            if (respUrl == null || respUrl.Length == 0)
            {
                LogUtility.OutputLog("156");
                return ErrCodeAndmErrMsg.STATUS_OK;
            }

            bool haveTarget = (target != null) && (target.Length != 0);
            int rtn = ErrCodeAndmErrMsg.STATUS_OK;
            string url = CreateOpenURL(brprocessname, respUrl, target, ErrorCode, jobId);
            if (!url.Equals(""))
            {
                rtn = OpenUrlByBrowser(url, brprocessname, haveTarget);
            }

            return rtn;
        }

        /// <summary>
        /// ブラウザ種別識別文字列
        /// </summary>
        public static string CreateOpenURL(string brprocessname, string respURL, string target, int errCode, string jobId)
        {
            string rtn = "";
            string paramString = CreateParamString(errCode);
            if (jobId != null && jobId.Length > 0)
            {
                paramString += "&JOBID=" + HttpUtility.UrlEncode(jobId, Encoding.UTF8);

            }
            //IEかつtargetありとそれ以外で開くURLと形式が変わる
            try
            {
                if (brprocessname.Equals(CommonConstants.browserProcessnames[1]) && target != null && target.Length > 0)
                {
                    rtn = String.Format(FORMAT_SEND_DATA, SettingMng.PortNo.ToString(), respURL, paramString, target, jobId);
                }
                else
                {
                    rtn = respURL + "?" + paramString;
                }
            }
            catch (Exception ex)
            {
                //失敗時は空文字列
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                return "";
            }
            return rtn;
        }
        public static string CreateParamString(int errorCode)
        {
            string rtn = "";
            if (errorCode == ErrCodeAndmErrMsg.STATUS_OK)
            {
                rtn = CommonConstants.RESULT + "=" + CommonConstants.SUCCESS;
            }
            else
            {
                rtn = CommonConstants.RESULT + "=" + CommonConstants.FAIL;
            }
            rtn += "&" + CommonConstants.ERROR_CODE + "=" + string.Format("{0:d3}", (ushort)errorCode)
            + "&" + CommonConstants.ERROR_CAUSE + "=" + HttpUtility.UrlEncode(ErrCodeAndmErrMsg.ChangeCodeToCause(errorCode), Encoding.UTF8)
            + "&" + CommonConstants.ERROR_DETAILS + "=" + HttpUtility.UrlEncode(ErrCodeAndmErrMsg.ChangeCodeToDetail(errorCode), Encoding.UTF8);
            return rtn;

        }
        /// <summary>
        /// htmlファイルの読み込みと、タグの置き換え
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="sbl_ns"></param>
        /// <param name="target"></param>
        /// <returns>置き換え済みのhtml文字列</returns>
        public static string LoadResponseHtmlAndReplaceTag(string url, string param, string sbl_ns, string target)
        {
            string htmlFileString = "";

            LogUtility.OutputDebugLog("E014", url, param, sbl_ns, target);

            //存在確認。存在していなければ、エラーとして空文字でリターン
            if (!System.IO.File.Exists(SettingMng.ResponseTemplateHtmlFile))
            {
                //ファイルが存在していない
                LogUtility.OutputLog("049", "File Not Exists", SettingMng.ResponseTemplateHtmlFile);
                return htmlFileString;
            }
            else
            {
                try
                {
                    using (StreamReader fs = new System.IO.StreamReader(SettingMng.ResponseTemplateHtmlFile))
                    {
                        string readL;
                        while ((readL = fs.ReadLine()) != null)
                        {
                            htmlFileString += readL + "\r\n";
                        }
                    }
                }
                catch (Exception ex)
                {
                    //ファイル読み込みに失敗
                    LogUtility.OutputLog("049", ex.Message);
                    return "";
                }
            }
            //特殊タグ部分を置き換え
            if (url != null && url.Length > 0)
            {
                htmlFileString = htmlFileString.Replace(SPTAG_START_END + SPTAG_URL + SPTAG_START_END, url);
            }
            if (param != null && param.Length > 0)
            {
                htmlFileString = htmlFileString.Replace(SPTAG_START_END + SPTAG_PARAM + SPTAG_START_END, param);
            }
            if (sbl_ns != null && sbl_ns.Length > 0)
            {
                htmlFileString = htmlFileString.Replace(SPTAG_START_END + SPTAG_SBL_NS + SPTAG_START_END, sbl_ns);
            }
            if (target != null && target.Length > 0)
            {
                htmlFileString = htmlFileString.Replace(SPTAG_START_END + SPTAG_TARGET + SPTAG_START_END, target);
            }

            return htmlFileString;
        }
        /// <summary>
        /// 指定されたURLをブラウザ種別により開く
        /// </summary>
        /// <param name="reqURL">対象URL</param>
        /// <param name="browserType">ブラウザ種別</param>
        /// <returns></returns>
        public static int OpenUrlByBrowser(string reqURL, string browserType, bool haveTarget)
        {
            int rtn = ErrCodeAndmErrMsg.STATUS_OK;

            //URLが指定されていない場合は当然開けない
            if (reqURL == null || reqURL.Length == 0)
            {
                LogUtility.OutputLog("120");
                return rtn;
            }
            string bName = "Default";
            //プロセス名/ブラウザ起動対応を取得
            int procNum = -1;
            for (int i = 0; i < SettingMng.ProcNames.Length; i++)
            {
                if (browserType.Equals(SettingMng.ProcNames[i]))
                {
                    procNum = i;
                    bName = SettingMng.BrowserNames[i];
                    break;
                }
            }
            LogUtility.OutputLog("078", bName, reqURL);

            //Edgeは特殊処理
            if (procNum > 0 && (String.Compare(bName, "Edge", true) == 0))
            {
                LogUtility.OutputLog("080", bName, reqURL);
                rtn = OpenByEdge(reqURL);
            }
#if DEBUG
            else if (browserType.Equals("devenv"))
            {
                //デバッグ用。IE固定
                rtn = OpenByBrowser(reqURL, CommonConstants.browserProcessnames[1], haveTarget);
                LogUtility.OutputLog("080", CommonConstants.browserProcessnames[1], reqURL);
            }
#endif
            //それ以外の場合は、対応したので動かす。
            else if (procNum >= 0 && bName.Length > 0)
            {
                rtn = OpenByBrowser(reqURL, bName, haveTarget);
                LogUtility.OutputLog("080", bName, reqURL);
            }
            else
            {
                //設定ファイルのデフォルトを使用
                LogUtility.OutputLog("079", browserType, SettingMng.DefaultBrowserType);
                if (String.Compare(SettingMng.DefaultBrowserType, "Edge", true) == 0)
                {
                    rtn = OpenByEdge(reqURL);
                }
                else
                {
                    rtn = OpenByBrowser(reqURL, SettingMng.DefaultBrowserType, haveTarget);
                }
            }
            return rtn;

        }

        /// <summary>
        /// Edgeの起動。Win10未満では失敗する。が、そもそもここに来ない
        /// </summary>
        /// <param name="url">開くurl</param>
        /// <returns></returns>
        public static int OpenByEdge(string url)
        {
            int rtn = ErrCodeAndmErrMsg.STATUS_OK;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "microsoft-edge:" + url;
            startInfo.UserName = Process.GetCurrentProcess().StartInfo.UserName;
            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                LogUtility.OutputLog("154", ex.Message);
                rtn = ErrCodeAndmErrMsg.ERR_CODE_0509;
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
            }

            return rtn;
        }

        /// <summary>
        /// 指定したブラウザでURLを開く
        /// </summary>
        /// <param name="url">開くURL</param>
        /// <param name="browser">SilentPdfPrinter起動元ブラウザプロセス名</param>
        /// <returns></returns>
        public static int OpenByBrowser(string url, string browser, bool haveTarget)
        {
            int rtn = ErrCodeAndmErrMsg.STATUS_OK;
            Process proc;
            int pId = 0;
            ProcessStartInfo startInfo = new ProcessStartInfo(browser);
            if (haveTarget)
            {
                startInfo.WindowStyle = ProcessWindowStyle.Minimized;
            }
            startInfo.Arguments = url;
            startInfo.UserName = Process.GetCurrentProcess().StartInfo.UserName;
            try
            {
                proc = Process.Start(startInfo);
                pId = proc.Id;
            }
            catch (Exception ex)
            {
                LogUtility.OutputLog("155", browser, ex.Message);
                rtn = ErrCodeAndmErrMsg.ERR_CODE_0508;
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
            }


            return rtn;
        }

    }
}
