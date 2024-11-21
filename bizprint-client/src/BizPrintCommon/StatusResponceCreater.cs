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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using static BizPrintCommon.PrintHistoryManager;

namespace BizPrintCommon
{
    /// <summary>
    /// 印刷履歴要求返信内容作成クラス
    /// </summary>
    public class StatusResponceCreater
    {

        #region XMLタグ 共通部分
        /// <summary>XMLメイン</summary>
        private const string XML_RESPONSE = "Response";

        /// <summary>結果</summary>
        private const string XML_RESULT = "Result";
        /// <summary>エラーコード</summary>
        private const string XML_ERRORCODE = "ErrorCode";
        /// <summary>エラー原因</summary>
        private const string XML_ERRORCAUSE = "ErrorCause";
        /// <summary>エラー内容</summary>
        private const string XML_ERRORDETAILS = "ErrorDetails";


        #endregion
        #region XMLタグ 1JobID毎のステータス詳細部分
        /// <summary>各ステータス </summary>
        private const string XML_PRINTSTATUS = "PrintStatus";

        //履歴構造体の内容に対応
        public const string XML_JOBID = "JobId";
        public const string XML_JOBNAME = "jobName";
        public const string XML_PRINTERNAME = "printerName";
        public const string XML_DATETIME = "DateTime";
        public const string XML_STATE = "Status";
        public const string XML_STATE_CODE = "StatusCode";

        #endregion


        /// <summary>結果</summary>
        public static string StatusResult { set; get; } = String.Empty;
        /// <summary>エラーコード</summary>
        public static int ErrorCode { set; get; } = ErrCodeAndmErrMsg.STATUS_OK;
        /// <summary>エラー理由(成功時は空)</summary>
        public static string ErrorCause { set; get; } = String.Empty;
        /// <summary>エラー詳細</summary>
        public static string ErrorDetails { set; get; } = String.Empty;

        #region 結果固定文字列
        /// <summary>成功</summary>
        public const string RESULT_SUCCESS = "SUCCESS";
        /// <summary>失敗</summary>
        public const string RESULT_FAIL = "FAIL";
        #endregion


        /// <summary>
        /// 印刷履歴構造体配列の取得
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private static PrintHistoryInfo[] GetHistoryInfo(StatusRequest req)
        {
            LogUtility.OutputLog("072", PrintHistoryManager.PrnStatusJobList.Count.ToString());
            //全履歴を取得
            if (req.IsAllRequest)
            {
                if (PrintHistoryManager.PrnStatusJobList.Count == 0)
                {
                    //NoData
                    return null;
                }
                LogUtility.OutputLog("074");
                return PrintHistoryManager.GetAllHistory();
            }
            //ID指定取得
            else
            {
                int numLightId = 0;
                //取得に成功したID
                string reqIds = "";
                //取得に失敗したID
                string errorIds = "";
                //ID指定の場合、存在しないIDが含まれている可能性があるのでチェック
                for (int i = 0; i < req.ReqJobIDList.Count; i++)
                {
                    PrintHistoryInfo wkInfo = PrintHistoryManager.GetHistoryFromId((string)req.ReqJobIDList[i]);
                    if (wkInfo != null)
                    {
                        //成功側を記録
                        numLightId++;
                        if (reqIds.Length > 0) {
                            reqIds += ",";
                        }
                        reqIds += req.ReqJobIDList[i];
                    }
                    else {
                        //失敗側を記録
                        if (errorIds.Length > 0) {
                            errorIds += ",";
                        }
                        errorIds += req.ReqJobIDList[i];
                    }
                }
                //全て取得に失敗
                if (numLightId == 0)
                {
                    //NoDataError
                    LogUtility.OutputLog("075", errorIds);
                    return null;
                }
                LogUtility.OutputLog("073", reqIds);
                PrintHistoryInfo[] rtnInfo = new PrintHistoryInfo[numLightId];

                //有効なものだけを格納
                numLightId = 0;
                for (int i = 0; i < req.ReqJobIDList.Count; i++)
                {
                    PrintHistoryInfo workInfo = PrintHistoryManager.GetHistoryFromId((string)req.ReqJobIDList[i]);
                    if (workInfo != null)
                    {
                        rtnInfo[numLightId] = workInfo;
                        numLightId++;

                    }
                }
                return rtnInfo;
            }

        }

        /// <summary>
        /// 返信内容XML作成
        /// </summary>
        /// <param name="req">要求内容</param>
        /// <returns>XML文字列</returns>
        public static string MakeStatusReqResponceXML(StatusRequest req)
        {
            StringBuilder rtnStrBuilder = new StringBuilder();
            string strValue = "";

            //履歴配列取得
            PrintHistoryInfo[] pInfo = GetHistoryInfo(req);

            if (pInfo == null || pInfo.Length == 0)
            {
                //要求されたJbIDに対応する履歴が存在しない、または、全選択で1個も無いのでFAIL
                StatusResult = RESULT_FAIL;
                if (req.IsAllRequest)
                {
                    LogUtility.OutputDebugLog("E507");
                    ErrorCode = ErrCodeAndmErrMsg.ERR_CODE_0507;
                }
                else
                {
                    LogUtility.OutputDebugLog("E506");
                    ErrorCode = ErrCodeAndmErrMsg.ERR_CODE_0506;
                }

                ErrorCause = ErrCodeAndmErrMsg.ChangeCodeToCause(ErrorCode);
                ErrorDetails = ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrorCode);
            }
            else if (pInfo.Length == 1)
            {
                //1個だけなので、メインの状態はその1個に由来する
                if (pInfo[0].errCode == ErrCodeAndmErrMsg.STATUS_OK)
                {
                    StatusResult = RESULT_SUCCESS;
                }
                else
                {
                    StatusResult = RESULT_FAIL;
                }
                ErrorCode = pInfo[0].errCode;
                ErrorCause = ErrCodeAndmErrMsg.ChangeCodeToCause(ErrorCode);
                ErrorDetails = ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrorCode);
            }
            else
            {
                //複数個あるので、メインの状態は成功とする
                StatusResult = RESULT_SUCCESS;
                ErrorCode = ErrCodeAndmErrMsg.STATUS_OK;
                ErrorCause = ErrCodeAndmErrMsg.ChangeCodeToCause(ErrorCode);
                ErrorDetails = ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrorCode);
            }


            Encoding encjis = Encoding.GetEncoding(932);
            Encoding encutf8 = Encoding.GetEncoding("utf-8");
            MemoryStream ms = new MemoryStream();
            XmlTextWriter xmlWriter = new XmlTextWriter(ms, encjis);

            xmlWriter.WriteStartDocument();
            // ルートの書き出し
            xmlWriter.WriteStartElement(XML_RESPONSE);

            #region 共通部分を出力

            //statusResult
            xmlWriter.WriteStartElement(XML_RESULT);
            strValue = HttpUtility.UrlEncode(StatusResult, encjis);
            xmlWriter.WriteString(strValue);
            xmlWriter.WriteEndElement();


            //errorCode
            xmlWriter.WriteStartElement(XML_ERRORCODE);
            strValue = string.Format("{0:d3}", (ushort)ErrorCode);
            strValue = HttpUtility.UrlEncode(strValue);
            xmlWriter.WriteString(strValue);
            xmlWriter.WriteEndElement();

            //errorCause
            xmlWriter.WriteStartElement(XML_ERRORCAUSE);
            strValue = HttpUtility.UrlEncode(ErrorCause);
            xmlWriter.WriteString(strValue);
            xmlWriter.WriteEndElement();

            //errorDetails
            xmlWriter.WriteStartElement(XML_ERRORDETAILS);
            strValue = HttpUtility.UrlEncode(ErrorDetails);
            xmlWriter.WriteString(strValue);
            xmlWriter.WriteEndElement();
            #endregion

            //印刷履歴の数だけループ
            if (pInfo != null)
            {
                for (int i = 0; i < pInfo.Length; i++)
                {
                    //詰めて渡される処理なので無いはずだが、チェックはする
                    if (null == pInfo[i])
                    {
                        continue;
                    }
                    //PrintStatusの書き出し
                    xmlWriter.WriteStartElement(XML_PRINTSTATUS);
                    //JobID
                    xmlWriter.WriteAttributeString(XML_JOBID, pInfo[i].jobId);

                    //JobName
                    xmlWriter.WriteStartElement(XML_JOBNAME);
                    strValue = HttpUtility.UrlEncode(pInfo[i].printFileName);
                    xmlWriter.WriteString(strValue);

                    xmlWriter.WriteEndElement();

                    //PrinterName
                    xmlWriter.WriteStartElement(XML_PRINTERNAME);
                    strValue = HttpUtility.UrlEncode(pInfo[i].printerName);
                    xmlWriter.WriteString(strValue);
                    xmlWriter.WriteEndElement();

                    //printedTime
                    xmlWriter.WriteStartElement(XML_DATETIME);
                    strValue = HttpUtility.UrlEncode(pInfo[i].lastEventTime.ToString("G"));
                    xmlWriter.WriteString(strValue);
                    xmlWriter.WriteEndElement();

                    //statusString
                    xmlWriter.WriteStartElement(XML_STATE);
                    strValue = HttpUtility.UrlEncode(CommonConstants.ChangeStatusCodeToString(pInfo[i].statusCode));
                    xmlWriter.WriteString(strValue);
                    xmlWriter.WriteEndElement();

                    //StatusCode
                    xmlWriter.WriteStartElement(XML_STATE_CODE);
                    strValue = string.Format("{0:d4}", (ushort)(pInfo[i].statusCode));
                    strValue = HttpUtility.UrlEncode(strValue);
                    xmlWriter.WriteString(strValue);
                    xmlWriter.WriteEndElement();

                    //ErrorCode
                    xmlWriter.WriteStartElement(XML_ERRORCODE);
                    strValue = string.Format("{0:d3}", (ushort)(pInfo[i].errCode));
                    strValue = HttpUtility.UrlEncode(strValue);
                    xmlWriter.WriteString(strValue);
                    xmlWriter.WriteEndElement();

                    //ErrorCause
                    xmlWriter.WriteStartElement(XML_ERRORCAUSE);
                    strValue = HttpUtility.UrlEncode(ErrCodeAndmErrMsg.ChangeCodeToCause(pInfo[i].errCode));
                    xmlWriter.WriteString(strValue);
                    xmlWriter.WriteEndElement();

                    //ErrorDetail
                    xmlWriter.WriteStartElement(XML_ERRORDETAILS);
                    strValue = HttpUtility.UrlEncode(ErrCodeAndmErrMsg.ChangeCodeToDetail(pInfo[i].errCode));
                    xmlWriter.WriteString(strValue);
                    xmlWriter.WriteEndElement();

                    //1つのInfoの終端
                    xmlWriter.WriteEndElement();

                }
            }


            //終端の書き出し
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();

            //Flush
            xmlWriter.Flush();

            //返信文字列化
            int len = (int)ms.Length;
            byte[] data = new byte[len];
            int pos = (int)ms.Position;
            ms.Seek(0, SeekOrigin.Begin);
            ms.Read(data, 0, len);

            LogUtility.OutputLog("076", Encoding.UTF8.GetString(data));
            return encjis.GetString(data);

        }
    }
}
