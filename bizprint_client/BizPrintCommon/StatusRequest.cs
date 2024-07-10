using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BizPrintCommon
{
    /// <summary>
    /// 印刷履歴要求を分解し、要求内容を保持するクラス
    /// </summary>
    public class StatusRequest
    {
        /// <summary>
        /// 要求されたJobIDのリスト
        /// </summary>
        public ArrayList ReqJobIDList { private set; get; } = new ArrayList();
        /// <summary>
        /// 全履歴送信フラグ
        /// </summary>
        public bool IsAllRequest { private set; get; } = false;

        /// <summary>
        /// JobId毎に分割するためのキー文字列
        /// </summary>
        private const string JOBID = "jobID";
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="log"></param>
        public StatusRequest()
        {

            ReqJobIDList.Clear();
        }
        /// <summary>
        /// リクエスト文字列を分解し、要求されるJobIDを格納する。
        /// </summary>
        /// <param name="req"></param>
        public void ReadParam(string req)
        {
            ReqJobIDList.Clear();
            try
            {
                // 分割
                string[] paramSplitted = req.Split(new char[] { '&' });
                string[] elemntsSplitted = null;
                string Value = "";
                for (int i = 0; i < paramSplitted.Length; i++)
                {
                    elemntsSplitted = null;
                    elemntsSplitted = paramSplitted[i].Split(new char[] { '=' });
                    if (2 != elemntsSplitted.Length)
                    {
                        continue;
                    }
                    // Urlデコードする
                    Value = "";
                    Value = HttpUtility.UrlDecode(elemntsSplitted[1]);
                    switch (elemntsSplitted[0])
                    {
                        case JOBID: // ジョブID
                            ReqJobIDList.Add(Value);
                            break;
                    }
                }


            }
            catch (Exception ex)
            {
                LogUtility.OutputDebugLog("E302", ex.Message);
            }
            //JOBID指定が無い場合は全JOBIDのステータスを送信
            if (ReqJobIDList.Count == 1 && ReqJobIDList[0].Equals(""))
            {
                IsAllRequest = true;
            }
            else if (ReqJobIDList.Count == 0)
            {
                IsAllRequest = true;
            }

        }

    }
}
