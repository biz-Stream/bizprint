using Ionic.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using static BizPrintCommon.PrintHistoryManager;

namespace BizPrintCommon
{
    /// <summary>
    /// HTTP受信内容解析、処理実行クラス
    /// </summary>
    public class HttpAnalyzer
    {
        public SettingManeger m_SetMng { set; get; }
        /// <summary>サーバ種別</summary>
        private int ServiceType;//Direct or Batch
        /// <summary>ソケット</summary>
        private Socket ConnectSocket;
        /// <summary>ワーカスレッド</summary>
        private Thread ThreadOfWorker = null;
        /// <summary>接続フラグ</summary>
        private bool IsConnected = false;

        /// <summary>接続監視タイマー</summary>
        private System.Threading.Timer CheckConnectionTimer = null;


        /// <summary>ネットワークストリーム</summary>
        private NetworkStream LocalNetStream;
        /// <summary>バッファストリーム</summary>
        private BufferedStream LocalBufferStream;
        /// <summary>ストリームリーダー</summary>
        private StreamReader LocalStreamReader;
        /// <summary>ストリームライター</summary>
        private StreamWriter LocalStreamWriter;

        /// <summary>ハッシュテーブル</summary>
        private Hashtable HeaderTable;
        /// <summary>ボディ部ハッシュテーブル</summary>
        private Hashtable BodyTable;

        /// <summary>エラーコード</summary>
        private int LatestErrCode = 0;
        /// <summary>データ保持用ArrayList</summary>
        ArrayList DataArrayList = new ArrayList();

        /// <summary>
        /// メソッド名
        /// </summary>
        private string RequestMethodName { set; get; } = string.Empty;
        /// <summary>
        /// URL
        /// </summary>
        private string RequestURL { set; get; } = string.Empty;
        /// <summary>
        /// プロトコル
        /// </summary>
        private string RequestProtocol { set; get; } = string.Empty;


        /// <summary>ジョブID</summary>
        private static string CreatedJobID { set; get; } = string.Empty;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="type">Direct/Batch</param>
        /// <param name="soc">ソケット</param>
        /// <param name="log">ロガー</param>
        public HttpAnalyzer(int type, Socket soc)
        {
            ServiceType = type;
            ConnectSocket = soc;

            HeaderTable = new Hashtable();
            BodyTable = new Hashtable();
            // エラーコードの初期化
            LatestErrCode = 0;
        }
        /// <summary>
        /// スレッド生存チェック
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return (ThreadOfWorker != null && ThreadOfWorker.IsAlive);
            }
        }
        /// <summary>
        ///  スレッド開始処理
        /// </summary>
        public void StartWorkThread()
        {

            if (!IsRunning)
            {
                ThreadOfWorker = new Thread(new ThreadStart(WorkerThread));
                ThreadOfWorker.IsBackground = true;
                ThreadOfWorker.Start();
            }
            else
            {
                //既に動作中なので、エラー(なにもしない)
            }


        }

        /// <summary>
        /// 実処理スレッド
        /// </summary>
        public void WorkerThread()
        {
            LogUtility.OutputLog("025");
            CreatedJobID = "";
            try
            {
                LocalNetStream = new NetworkStream(ConnectSocket, FileAccess.ReadWrite);

                LocalBufferStream = new BufferedStream(LocalNetStream);
                LocalStreamReader = new StreamReader(LocalBufferStream);
                LocalStreamWriter = new StreamWriter(LocalBufferStream);
                IsConnected = true;
                // 同期
                byte[] byteData = new byte[CommonConstants.RECV_LEN];
                byte[] byteWork = null;
                int len = 0;
                string request = "";
                string work = "";
                int iCnt = 0;
                Encoding encJis = Encoding.GetEncoding("shift-jis");
                StringBuilder strBuilder = new StringBuilder();
                byte[] kData = new byte[CommonConstants.RECV_LEN];
                byte[] dataStartChk = Encoding.ASCII.GetBytes(CommonConstants.SEND_PARANT_NAME);


                //ソケットからの初期読み込み
                len = ConnectSocket.Receive(kData);//このkDataはlenより先が存在する

                //サイズ丁度の配列に入れ直す
                byte[] kWork = new byte[len];
                Array.Copy(kData, 0, kWork, 0, len);
                Array.Clear(kData, 0, kData.Length);
                kData = null;
                //読み込みサイズ==0ならば、受信データなし log027
                if (0 >= len)
                {
                    String msg = "";
                    LogUtility.OutputLog("027", ConnectSocket.ToString(), msg);
                    IsConnected = false;
                    //データエラーで返す。jobIDなし
                    LatestErrCode = ErrCodeAndmErrMsg.ERR_CODE_0502;
                    throw new Exception(ErrCodeAndmErrMsg.ERR_MSG_0502);
                }
                DataArrayList.Clear();

                request = encJis.GetString(kWork, 0, len);
                strBuilder.Append(request);
                int errChk = 0;
                // 受信データを変換
                StringReader stringReaderRcv = new StringReader(strBuilder.ToString());
                // HTTPメソッド取得
                errChk = GetMethod(stringReaderRcv);
                if (errChk != ErrCodeAndmErrMsg.STATUS_OK)
                {
                    LatestErrCode = ErrCodeAndmErrMsg.ERR_CODE_0504;
                    LogUtility.OutputDebugLog("E504", "GetMethod");
                    throw new Exception(ErrCodeAndmErrMsg.ERR_MSG_0504);
                }
                // HTTPヘッダー取得
                errChk = GetHttpHeader(stringReaderRcv);
                if (errChk != 0)
                {
                    LatestErrCode = ErrCodeAndmErrMsg.ERR_CODE_0503;
                    LogUtility.OutputDebugLog("E503");
                    throw new Exception(ErrCodeAndmErrMsg.ERR_MSG_0503);
                }

                //残り全データの取得開始
                // 残りの電文は一旦ひとまとめにしてBODY扱い
                string allDataString = stringReaderRcv.ReadToEnd();
                byte[] tmpByteData = System.Text.Encoding.GetEncoding("shift_jis").GetBytes(allDataString);
                work = Encoding.ASCII.GetString(kWork);
                int sppstartIndex = work.IndexOf(CommonConstants.SEND_DATA_BYTES);
                if (sppstartIndex > 0 && work.Length > 0)
                {
                    DataArrayList.Add((byte[])kWork.Clone());
                }

                string subStrings = strBuilder.ToString().Substring(0, strBuilder.ToString().Length - allDataString.Length);
                strBuilder.Remove(0, strBuilder.Length);
                strBuilder.Append(allDataString);

                // 電文長カウンタ ヘッダにContent-Lengthが無い場合はあるのか？
                string strContentLength = (string)HeaderTable["Content-Length"];
                int contentLength = 0;
                if (null != strContentLength)
                {
                    contentLength = int.Parse(strContentLength);
                }

                contentLength -= tmpByteData.Length;

                //ソケットから全データを受信
                iCnt++;
                while (0 < contentLength)
                {
                    Array.Clear(byteData, 0, byteData.Length);
                    byteWork = null;
                    work = "";
                    request = "";

                    len = ConnectSocket.Receive(byteData);
                    contentLength -= len;

                    byteWork = new byte[len];
                    Array.Copy(byteData, 0, byteWork, 0, len);
                    DataArrayList.Add((byte[])byteWork.Clone());
                    Array.Clear(byteWork, 0, byteWork.Length);
                    byteWork = null;
                    //バッファに全部入れる
                    request = encJis.GetString(byteData, 0, len);
                    strBuilder.Append(request);
                }
                int allLength = System.Text.Encoding.GetEncoding("shift_jis").GetBytes(strBuilder.ToString()).Length;
                StringReader strRd = new StringReader(strBuilder.ToString());
                LogUtility.OutputLog("028", RequestMethodName, RequestProtocol, allLength.ToString());

                int Type = UrlToRequestType(RequestURL);
                //呼び出されたサービスごとに分岐し、処理を行う
                switch (Type)
                {
                    //印刷指示
                    case CommonConstants.URLTYPE_PRINTSTART:
                        LogUtility.OutputLog("029");
                        if (ServiceType == CommonConstants.MODE_BATCH)
                        {
                            //分解、解析、解凍
                            errChk = getBrowserAndParamAndPdf(strRd);
                            //結果込みで200返信
                            WriteBatchPrintRes(errChk);
                        }
                        else
                        {
                            //200返信
                            WriteDirectPrintReq();
                            //分解、解析、解凍
                            errChk = getBrowserAndParamAndPdf(strRd);
                            //レスポンス指定が取れているならdoresponceをしているのでここでの処理は不要
                        }
                        break;
                    //印刷状態取得
                    case CommonConstants.URLTYPE_PRTSTATGET:
                        LogUtility.OutputLog("030");
                        //Batchの場合のみ動作。Directの場合は404
                        if (ServiceType == CommonConstants.MODE_BATCH)
                        {
                            StatusRequest stReq = new StatusRequest();
                            stReq.ReadParam(strRd.ReadToEnd());
                            string stateRtn = StatusResponceCreater.MakeStatusReqResponceXML(stReq);
                            //JOBID指定、または現在の全ステータス列挙XMLを作成
                            //ステータスリクエスト返信200処理
                            WriteBatchStatusReq(stateRtn);
                        }
                        else
                        {
                            Write404NotFound();
                        }
                        break;
                    //レスポンス指示
                    case CommonConstants.URLTYPE_DORESPONSE:
                        LogUtility.OutputLog("031");
                        //Directの場合のみ動作。Batchの場合は404
                        if (ServiceType == CommonConstants.MODE_DIRECT)
                        {
                            LogUtility.OutputLog("048");
                            errChk = WriteDoresponceReq(RequestURL);
                        }
                        else
                        {
                            LogUtility.OutputDebugLog("E031");
                            Write404NotFound();
                        }
                        break;
                    //favicon.ico
                    case CommonConstants.URLTYPE_FAVICON:
                        LogUtility.OutputLog("032");
                        //存在しないので404
                        Write404NotFound();
                        break;
                    //状態確認
                    case CommonConstants.URLTYPE_ISALIVE:
                        LogUtility.OutputLog("450");
                        WriteIsaliveReq(strRd.ReadToEnd());
                        break;
                    //それ以外
                    case CommonConstants.URLTYPE_OTHER:
                    default:
                        LogUtility.OutputLog("033", RequestURL);
                        //対象外なので404
                        Write404NotFound();
                        break;
                }
                //最後にkWorkをクリア
                Array.Clear(kWork, 0, kWork.Length);
                kWork = null;

            }
            catch (SocketException ex)
            {
                LogUtility.OutputLog("026", m_SetMng.PortNo.ToString(), ex.Message);
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
                //ソケットエラーで受信できない場合、この書き込みも失敗するかもしれないが一応
                Write404NotFound();
            }
            catch (Exception ex)
            {
                LogUtility.OutputLog("152", m_SetMng.PortNo.ToString(), ex.Message);
                //通信エラー、プロトコルエラーなど、特殊なエラーなので、書き込む出来るか微妙だが一応
                if (LatestErrCode != 0)
                {
                    WriteErrorPrintRes(LatestErrCode);
                }
                else
                {
                    //基本的にあり得ないはずだが、正常ルートではないので404
                    Write404NotFound();
                }
                //LOG 501
                LogUtility.OutputLog("501", "HttpAnalyzer", ex.Message);

            }
            finally
            {
                DisConnect();
            }

        }
        /// <summary>
        /// URLから処理種別数値への変換
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns>対応する数値</returns>
        private int UrlToRequestType(string url)
        {
            //開始文字列で分岐
            if (url.IndexOf(CommonConstants.URL_PRINTSTART) == 0)
            {
                return CommonConstants.URLTYPE_PRINTSTART;
            }
            else if (url.IndexOf(CommonConstants.URL_PRTSTATGET) == 0)
            {
                return CommonConstants.URLTYPE_PRTSTATGET;
            }
            else if (url.IndexOf(CommonConstants.URL_DORESPONSE) == 0)
            {
                return CommonConstants.URLTYPE_DORESPONSE;
            }
            else if (url.IndexOf(CommonConstants.URL_FAVICON) == 0)
            {
                return CommonConstants.URLTYPE_FAVICON;
            }
            else if (url.EndsWith(CommonConstants.URL_FAVICON))
            {
                //ここのみ終端でも判断。終端にfavicon.icoが付いたURLを送ってくるブラウザがあるため
                return CommonConstants.URLTYPE_FAVICON;
            }
            else if (url.IndexOf(CommonConstants.URL_ISALIVE) == 0)
            {
                return CommonConstants.URLTYPE_ISALIVE;
            }
            else
            {
                return CommonConstants.URLTYPE_OTHER;
            }
        }
        /// <summary>
        /// メソッド・URL・プロトコルの取得
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private int GetMethod(StringReader reader)
        {
            int rtn = ErrCodeAndmErrMsg.STATUS_OK;

            string request = "";
            try
            {

                request = reader.ReadLine();

                string[] tokens = request.Split(new char[] { ' ' });
                if (tokens.Length < 3)
                {
                    //トークン数が足りないので、取得内容エラー
                    LogUtility.OutputDebugLog("E504", "tokens.Length < 3");
                    rtn = ErrCodeAndmErrMsg.ERR_CODE_0504;
                    return rtn;
                }
                RequestMethodName = tokens[0];
                RequestURL = tokens[1];
                RequestProtocol = tokens[2];
                // 「POST」or「GET」の判断(大文字小文字同一視)
                // 「HTTP」で開始されているかの判断(大文字小文字同一視) HTTP/1.1 のはずだが、スペース入ってるブラウザもある
                bool getFlg = (String.Compare(RequestMethodName, CommonConstants.HTTP_GET, true) == 0);
                bool postFlg = (String.Compare(RequestMethodName, CommonConstants.HTTP_POST, true) == 0);
                bool httpFlg = RequestProtocol.StartsWith(CommonConstants.HTTP_HTTP, true, null);
                if ((!getFlg && !postFlg) || !httpFlg)
                {
                    //取得内容エラー。本来ありえない文字列が送信されてきてる
                    rtn = ErrCodeAndmErrMsg.ERR_CODE_0504;
                    LogUtility.OutputDebugLog("E504", request);
                    if (!getFlg && !postFlg)
                    {
                        LogUtility.OutputLog("114", request);
                    }

                    return rtn;
                }
            }
            catch (Exception ex)
            {
                // 通信エラー
                rtn = ErrCodeAndmErrMsg.ERR_CODE_0502;
                LogUtility.OutputDebugLog("E502", request);
                // ログ出力
                string errstr = ex.Message;
                return rtn;

            }
            finally
            {

            }
            return rtn;
        }
        /// <summary>
        /// HTTPHeaderを分解してテーブルに格納
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private int GetHttpHeader(StringReader reader)
        {
            string line = "";
            HeaderTable.Clear();
            try
            {
                while ((line = reader.ReadLine()) != null && line != "")
                {
                    string[] tokens = line.Split(new char[] { ':' });
                    string name = tokens[0];
                    string value = "";
                    for (int i = 1; i < tokens.Length; i++)
                    {
                        value += tokens[i];
                        if (i < tokens.Length - 1) tokens[i] += ":";
                    }
                    HeaderTable[name] = value;

                }
            }
            catch (Exception ex)
            {
                // 通信エラー
                string errstr = ex.Message;
                return ErrCodeAndmErrMsg.ERR_CODE_0502;
            }
            finally
            {

            }
            if (HeaderTable.Count == 0)
            {
                //ヘッダエラー
                LogUtility.OutputLog("115", reader.ToString());
                return ErrCodeAndmErrMsg.ERR_CODE_0503;
            }
            //成功
            return ErrCodeAndmErrMsg.STATUS_OK; ;
        }
        /// <summary>
        /// 印刷指示(ダイレクト) 200 OK 応答 
        /// </summary>
        private void WriteDirectPrintReq()
        {
            LogUtility.OutputLog("041", "Direct");
            try
            {
                //Silentに200 OKを返して終わり
                // HTTPメソッド送信
                LocalStreamWriter.WriteLine("HTTP/1.1 200 OK");

                // HTTPヘッダ送信
                LocalStreamWriter.WriteLine("Content-Length:0");
                LocalStreamWriter.WriteLine("Content-Type: text/html");
                LocalStreamWriter.WriteLine();//ヘッダ終端


                LocalStreamWriter.Flush();

            }
            catch (Exception)
            {
            }
            finally
            {
            }

        }

        /// <summary>
        /// 印刷指示(バッチ) 200 OK 応答
        /// </summary>
        private void WriteBatchPrintRes(int errCode)
        {
            LogUtility.OutputLog("041", "Batch", errCode.ToString());
            try
            {
                // HTTPメソッド送信(要求受信としては200)
                LocalStreamWriter.WriteLine("HTTP/1.1 200 OK");

                string errstr = GetBatchPrintResStr(errCode); //応答内容のhtmlを作成
                errstr = HttpUtility.UrlEncode(errstr);
                // HTTPヘッダ送信
                LocalStreamWriter.WriteLine("Content-Length:" + errstr.Length);
                LocalStreamWriter.WriteLine("Content-Type: text/plain");
                LocalStreamWriter.WriteLine();//ヘッダ終端
                LocalStreamWriter.WriteLine(errstr);
                LocalStreamWriter.Flush();

            }
            catch (Exception ex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
            }
            finally
            {
            }

        }
        /// <summary>
        /// ステータス要求(バッチ) 200 OK 応答
        /// </summary>
        private void WriteBatchStatusReq(string returnXML)
        {
            int length;
            if (returnXML == null || returnXML.Length == 0)
            {
                length = 0;
            }
            else
            {
                length = returnXML.Length;
            }
            try
            {
                LogUtility.OutputLog("042", returnXML);

                // HTTPメソッド送信(要求受信としては200)
                LocalStreamWriter.WriteLine("HTTP/1.1 200 OK");

                // HTTPヘッダ送信
                LocalStreamWriter.WriteLine("Content-Length:" + length);
                LocalStreamWriter.WriteLine("Content-Type: text/xml");
                LocalStreamWriter.WriteLine();//ヘッダ終端
                LocalStreamWriter.WriteLine(returnXML);
                LocalStreamWriter.Flush();

            }
            catch (Exception ex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
            }
            finally
            {

            }

        }
        /// <summary>
        /// 印刷指示
        /// 特殊なエラー時応答
        /// </summary>
        private void WriteErrorPrintRes(int errCode)
        {
            try
            {
                // HTTPメソッド送信(要求受信としては200？)
                LocalStreamWriter.WriteLine("HTTP/1.1 200 OK");

                string errStr = GetBatchPrintResStr(errCode); //応答内容のXMLを作成
                errStr = HttpUtility.UrlEncode(errStr);

                // HTTPヘッダ送信
                LocalStreamWriter.WriteLine("Content-Length:" + errStr.Length);
                LocalStreamWriter.WriteLine("Content-Type: text/plain");
                LocalStreamWriter.WriteLine();//ヘッダ終端
                LocalStreamWriter.WriteLine(errStr);
                LocalStreamWriter.Flush();

            }
            catch (Exception ex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
            }
            finally
            {


            }

        }
        /// <summary>
        /// 異常応答
        /// </summary>
        private void Write404NotFound()
        {
            try
            {
                // HTTPメソッド送信
                LocalStreamWriter.WriteLine("HTTP/1.1 404 Not Found");
                // HTTPヘッダ送信
                LocalStreamWriter.WriteLine("Content-Length:0");
                LocalStreamWriter.WriteLine("Content-Type: text/html");
                LocalStreamWriter.WriteLine();//ヘッダ終端
                LocalStreamWriter.Flush();
            }
            catch (Exception ex)
            {
                //ソケットエラーの場合書けない可能性がある
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
            }
            finally
            {

            }
            LogUtility.OutputLog("050");
        }
        /// <summary>
        /// 受信内容を解析し、sppファイルの解凍を行ってキューに登録する
        /// </summary>
        /// <param name="strReader"></param>
        /// <returns></returns>
        private int getBrowserAndParamAndPdf(StringReader strReader)
        {
            int errCode = 0;
            string directBrowserName = "";
            byte[] sppFileByte = null;
            if (DataArrayList.Count <= 0)
            {
                //データなし
                return -1;
            }
            string rcv = strReader.ReadLine();
            int browsenameIndex = rcv.IndexOf(CommonConstants.SEND_DATA_BYTES);
            int sppstartIndex = rcv.IndexOf(CommonConstants.SEND_DATA_BYTES);
            string work = "";
            for (int i = 0; i < DataArrayList.Count; i++)
            {
                work = Encoding.ASCII.GetString((byte[])DataArrayList[i]);
                sppstartIndex = work.IndexOf(CommonConstants.SEND_DATA_BYTES);
                if (sppstartIndex > -1)
                {
                    break;
                }
            }



            //起動元ブラウザの情報を取得(Batchでは存在しない)
            string[] Elemnt = null;

            if (m_SetMng.ServiceType == CommonConstants.MODE_DIRECT)
            {
                directBrowserName = rcv.Substring(0, browsenameIndex);
                Elemnt = directBrowserName.Split(new char[] { '=' });
                string Value = "";
                if (Elemnt.Length >= 2)
                {
                    Value = Elemnt[1];
                }
                if (Elemnt[0].Equals("parent"))
                {

                    directBrowserName = Value.Replace("&", "");
                    LogUtility.OutputLog("037", directBrowserName);
                }
                if (directBrowserName.Length == 0)
                {
                    LogUtility.OutputLog("038", m_SetMng.DefaultBrowserType);
                }
            }
            sppstartIndex += CommonConstants.SEND_DATA_BYTES.Length;

            //SPPのサイズを計算 全byteArrayの総計から、"sppdata="より前の文字列長を引いたもの
            byte[] Plus;
            int sppSize = 0;
            for (int i = 0; i < DataArrayList.Count; i++)
            {
                Plus = null;
                Plus = (byte[])DataArrayList[i];
                sppSize += Plus.Length;
            }
            sppSize -= sppstartIndex;

            //sppファイルのバイト配列を作成
            sppFileByte = new byte[sppSize];
            int iOffset = 0;
            for (int i = 0; i < DataArrayList.Count; i++)
            {
                Plus = null;
                Plus = (byte[])DataArrayList[i];
                if (i == 0)
                {
                    Array.Copy(Plus, sppstartIndex, sppFileByte, iOffset, Plus.Length - sppstartIndex);
                    iOffset += (Plus.Length - sppstartIndex);
                }
                else
                {
                    Array.Copy(Plus, 0, sppFileByte, iOffset, Plus.Length);
                    iOffset += Plus.Length;
                }
            }
            if (sppSize != iOffset || iOffset == 0)
            {
                //sppファイルのサイズが異常
                errCode = ErrCodeAndmErrMsg.ERR_CODE_0103;
                LogUtility.OutputDebugLog("E096", iOffset.ToString());
                LogUtility.OutputLog("177", iOffset.ToString());
                if (m_SetMng.ServiceType == CommonConstants.MODE_DIRECT)
                {
                    MessageBox.Show(LogUtility.GetLogStr("177", iOffset.ToString()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return errCode;
            }

            //sppファイル解凍
            SppExtracter sppExtract = new SppExtracter();
            //パスワード初期化。失敗は解凍失敗と同じ
            int rtn = sppExtract.InitPass(m_SetMng.SppPassword);
            if (rtn != ErrCodeAndmErrMsg.STATUS_OK)
            {
                //エラーリターン
                //内容が取得できないので、Responseは開けない
                if (m_SetMng.ServiceType == CommonConstants.MODE_DIRECT)
                {
                    MessageBox.Show(LogUtility.GetLogStr("055", m_SetMng.SppPassword), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                errCode = rtn;

            }
            else
            {
                rtn = sppExtract.DoExtract(sppFileByte);
                if (rtn != ErrCodeAndmErrMsg.STATUS_OK)
                {
                    //失敗したので、エラーリターン
                    //内容が取得できないので、Responseは開けない
                    LogUtility.OutputLog("108", ErrCodeAndmErrMsg.ChangeCodeToDetail(rtn));
                    if (m_SetMng.ServiceType == CommonConstants.MODE_DIRECT)
                    {
                        MessageBox.Show(LogUtility.GetLogStr("108", ErrCodeAndmErrMsg.ChangeCodeToDetail(rtn)), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    errCode = rtn;
                }
                else
                {
                    //解凍に成功したので、ID作成してセットする
                    PrintParameter newParam = new PrintParameter();
                    rtn = newParam.ReadParamFile(sppExtract.ParamFileByte);
                    if (rtn != ErrCodeAndmErrMsg.STATUS_OK)
                    {
                        //パラメータに問題があったので、JobIDの発番もキュー追加もせずにエラーリターン
                        errCode = rtn;
                        LogUtility.OutputLog("127", Encoding.UTF8.GetString(sppExtract.ParamFileByte));
                        if (newParam.ResponseURL.Length != 0)
                        {
                            //doResponce先が取れているので、開く
                            int chk = WebBrowserUtil.OpenResponceNoID(directBrowserName, newParam.ResponseURL, newParam.TargetFrame, errCode);
                            if (chk != ErrCodeAndmErrMsg.STATUS_OK)
                            {
                                //レスポンス失敗
                                LogUtility.OutputLog("128", newParam.ResponseURL);
                            }
                        }
                    }
                    else
                    {
                        LogUtility.OutputLog("039", Encoding.UTF8.GetString(sppExtract.ParamFileByte));
                        //キューの上限個数チェック
                        if (PrintReqQueue.IsQueSizeOverMax())
                        {
                            //キュー上限でエラー
                            errCode = ErrCodeAndmErrMsg.ERR_CODE_0114;
                            LogUtility.OutputLog("119", newParam.JobName);

                            if (newParam.ResponseURL.Length != 0)
                            {
                                //doResponce先が取れているので、開く
                                int chk = WebBrowserUtil.OpenResponceNoID(directBrowserName, newParam.ResponseURL, newParam.TargetFrame, errCode);
                                if (chk != ErrCodeAndmErrMsg.STATUS_OK)
                                {
                                    //レスポンス失敗
                                    LogUtility.OutputDebugLog("E119");
                                }
                            }
                        }
                        else
                        {
                            string newID = JobIDManager.CreateJobID(newParam.JobName);
                            newParam.JobID = newID;
                            CreatedJobID = newID;
                            newParam.PdfDocumentByte = sppExtract.PdfFileByte;
                            newParam.PdfFileName = sppExtract.PdfFileName;
                            newParam.BrowserProcessname = directBrowserName;
                            //pIの引数付きコンストラクタ
                            PrintHistoryInfo pInfo = new PrintHistoryInfo(newID, CommonConstants.JOB_STATUS_ACCEPT);
                            pInfo.printFileName = newID + ".pdf";

                            //印刷履歴への追加
                            PrintHistoryManager.AddNewHistory(pInfo);
                            //印刷要求キューへの追加
                            PrintReqQueue.AddReqest(newParam);
                            //log036
                            LogUtility.OutputLog("036", newID);
                        }
                    }
                }
            }
            return errCode;
        }
        /// <summary>
        /// doresponceで書き込むhtmlを取得・作成する
        /// </summary>
        /// <param name="rcvStr"></param>
        /// <returns></returns>
        private string GetWriteHtml(string rcvStr)
        {
            string[] receiptData = null;

            if (rcvStr != null && !rcvStr.Equals(""))
            {
                receiptData = rcvStr.Split('$');
            }
            else
            {
                return "";
            }

            string url = "";   //接続先ＵＲＬ
            string param = ""; //パラメータ文字列
            string target = ""; //ターゲット名
            if (receiptData != null && receiptData.Length > 3)
            {
                url = receiptData[1];
                param = receiptData[2];
                target = receiptData[3];
            }
            else
            {
            }
            string[] urlp1 = null;
            string[] urlp2 = null;
            string[] urlp3 = null;
            urlp1 = url.Split('?');

            string[] param1 = null;
            string[] param2 = null;
            StringBuilder sbl_ns = new StringBuilder();

            StringBuilder sbl = new StringBuilder();
            if (urlp1.GetLength(0) >= 2)
            {
                if (urlp1[1] != null && !urlp1[1].Equals(""))
                {
                    urlp2 = urlp1[1].Split('&');
                    for (int j = 0; j < urlp2.Length; j++)
                    {
                        urlp3 = urlp2[j].Split('=');
                        sbl.Append("<input type=hidden name=\"" + urlp3[0] + "\" value=\"" + urlp3[1] + "\">");
                        sbl_ns.Append(urlp3[0] + "=" + urlp3[1] + "&");
                    }
                }
            }

            url = urlp1[0];
            if (param != null && !param.Equals(""))
            {

                param1 = param.Split('&');

                for (int i = 0; i < param1.Length; i++)
                {

                    param2 = param1[i].Split('=');

                    switch (param2[0])
                    {

                        case "RESULT":
                            sbl.Append("<input type=hidden name=\"RESULT\" value=\"" + param2[1] + "\">");
                            sbl_ns.Append("RESULT=" + param2[1] + "&");
                            break;
                        case "ERROR_CODE":
                            sbl.Append("<input type=hidden name=\"ERROR_CODE\" value=\"" + param2[1] + "\">");
                            sbl_ns.Append("ERROR_CODE=" + param2[1] + "&");
                            break;
                        case "ERROR_CAUSE":
                            sbl.Append("<input type=hidden name=\"ERROR_CAUSE\" value=\"" + param2[1] + "\">");
                            sbl_ns.Append("ERROR_CAUSE=" + param2[1] + "&");
                            break;
                        case "ERROR_DETAILS":
                            sbl.Append("<input type=hidden name=\"ERROR_DETAILS\" value=\"" + param2[1] + "\">");
                            sbl_ns.Append("ERROR_DETAILS=" + System.Web.HttpUtility.UrlEncode(param2[1]));
                            break;
                        default: break;
                    }
                }

            }
            param = sbl.ToString();


            string sendHtml = WebBrowserUtil.LoadResponseHtmlAndReplaceTag(url, param, sbl_ns.ToString(), target);

            return sendHtml;
        }
        /// <summary>
        /// Doresponce要求に対して返信を行う
        /// </summary>
        /// <param name="rcvStr"></param>
        /// <returns></returns>
        private int WriteDoresponceReq(string rcvStr)
        {
            int rtn = ErrCodeAndmErrMsg.STATUS_OK;

            string res = GetWriteHtml(rcvStr);
            string jobId = GetResponseJobId(rcvStr);

            if (rcvStr == null || rcvStr.Length == 0 || res == null || res.Length == 0)
            {
                LocalStreamWriter.WriteLine("HTTP/1.1 204 No Content");
                LocalStreamWriter.WriteLine("Content-Length: 0");
                LocalStreamWriter.WriteLine("Cache-Control: no-store");
                LocalStreamWriter.WriteLine("Connection: close");
                LocalStreamWriter.WriteLine();
                LocalStreamWriter.WriteLine();
                LocalStreamWriter.Flush();
                return -1;
            }
            // HTTPメソッド送信
            LocalStreamWriter.WriteLine("HTTP/1.1 200 OK");
            // HTTPヘッダ送信
            if (false == res.Equals(""))
            {
                LocalStreamWriter.WriteLine("Content-Length:" + res.Length);
            }
            else
            {
                LocalStreamWriter.WriteLine("Content-Length: 0");
            }

            LocalStreamWriter.WriteLine("Content-Type: text/html");

            LocalStreamWriter.WriteLine("Cache-Control: no-store");
            LocalStreamWriter.WriteLine("Connection: close");


            LocalStreamWriter.WriteLine();

            // HTTP BODY 送信
            if (null != res)
            {
                LocalStreamWriter.WriteLine(res);
            }
            // ログ書き出し
            LogUtility.OutputLog("145");

            LocalStreamWriter.Flush();

            return rtn;
        }
        /// <summary>
        /// doresponceで使用するJobIDを取得する
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetResponseJobId(string url)
        {
            string[] buffer = null;
            string jobID = string.Empty;

            if (url != null && url.Length > 0)
            {
                buffer = url.Split('$');

                if (buffer.Length >= 5)
                {
                    jobID = buffer[4];
                }
            }

            return jobID;
        }
        /// <summary>
        /// バッチ印刷の印刷要求に対する応答文字列を作成する
        /// </summary>
        /// <param name="errorCode">STATE_OK：成功、それ以外：失敗</param>
        /// <returns></returns>
        private static string GetBatchPrintResStr(int errorCode)
        {
            string rtn = "";
            //成功パターン
            if (errorCode == ErrCodeAndmErrMsg.STATUS_OK)
            {
                rtn = CommonConstants.RESULT + "=" + CommonConstants.SUCCESS
            + "&" + CommonConstants.ERROR_CODE + "=" + string.Format("{0:d3}", (ushort)errorCode)
            + "&" + CommonConstants.JOBID + "=" + CreatedJobID;

            }
            else
            {
                rtn = CommonConstants.RESULT + "=" + CommonConstants.FAIL;
                rtn += "&" + CommonConstants.ERROR_CODE + "=" + string.Format("{0:d3}", (ushort)errorCode);
                rtn += "&" + CommonConstants.ERROR_CAUSE + "=" + ErrCodeAndmErrMsg.ChangeCodeToCause(errorCode); ;
                rtn += "&" + CommonConstants.ERROR_DETAILS + "=" + HttpUtility.UrlEncode(ErrCodeAndmErrMsg.ChangeCodeToDetail(errorCode), Encoding.UTF8);
                //要求で失敗の場合、IDは生成されないので、必ず空値として返す
                rtn += "&" + CommonConstants.JOBID + "=";

            }
            return rtn;
        }
        /// <summary>
        /// isalive要求に対して返信を行う
        /// </summary>
        /// <param name="rcvStr"></param>
        /// <returns></returns>
        private int WriteIsaliveReq(string rcvStr)
        {
            //条件判定
            String rtnStr = "";

            bool queueCheckOK = true;

            //時間差分取得
            TimeSpan timeDiffer = DateTime.Now - SettingManeger.GetLastQueCheckTime();
            //ダイアログ表示中
            if (SettingManeger.IsPrintingWithDialog) {
                queueCheckOK = true;
            }
            else if (timeDiffer.TotalMilliseconds < SettingManeger.NoQueCheckTimeoutMsec)
            {
                queueCheckOK = true;
            }
            else {
                queueCheckOK = false;
            }

            string strDiffer = timeDiffer.TotalMilliseconds.ToString();
            //小数点以下がある場合はTrim
            if (strDiffer.IndexOf(".") > 0) {
                strDiffer = strDiffer.Substring(0, strDiffer.IndexOf("."));
            }


            //queCheckOKなら200で返す
            try
            {
                if (queueCheckOK)
                {
                    LogUtility.OutputLog("451", strDiffer);
                    // HTTPメソッド送信(200 OK)
                    LocalStreamWriter.WriteLine("HTTP/1.1 200 OK");
                    rtnStr = "isalive ok";
                }
                else {
                    LogUtility.OutputLog("452", strDiffer);
                    // HTTPメソッド送信(500 NG)
                    LocalStreamWriter.WriteLine("HTTP/1.1 500 NG");
                    rtnStr = "isalive ng";
                }

                // HTTPヘッダ送信
                LocalStreamWriter.WriteLine("Content-Length:" + rtnStr.Length);
                LocalStreamWriter.WriteLine("Content-Type: text/xml");
                LocalStreamWriter.WriteLine();//ヘッダ終端
                LocalStreamWriter.WriteLine(rtnStr);
                LocalStreamWriter.Flush();

            }
            catch (Exception ex)
            {
                ErrCodeAndmErrMsg.SetExErrorMsg = ex.Message;
            }

            return 0;
        }
        /// <summary>
        /// 切断処理
        /// </summary>
        private void DisConnect()
        {
            LogUtility.OutputLog("023");
            if (false == IsConnected)
            {
                return;
            }
            IsConnected = false;

            try
            {
                // ソケット切断
                if (null != ConnectSocket && true == ConnectSocket.Connected)
                {

                    ConnectSocket.Shutdown(SocketShutdown.Both);
                }
            }
            catch (Exception)
            {
                //相手側が先にクローズしてても、無視
            }
            finally
            {
                if (null != ConnectSocket)
                {
                    ConnectSocket.Close();
                    ConnectSocket = null;
                }
            }

            // ストリームクローズ
            DisConnectStream();
            //try{
            if (null != CheckConnectionTimer)
            {
                CheckConnectionTimer.Change(Timeout.Infinite, 0);
                CheckConnectionTimer.Dispose();
                CheckConnectionTimer = null;
            }
            //}
            /*catch (Exception ex)
            {
                LogUtility.OutputDebugLog("E024" + ex.Message);
            }*/

        }
        /// <summary>
        /// ストリームクローズ
        /// </summary>
        private void DisConnectStream()
        {
            try
            {
                Object[] strms = { LocalStreamWriter, LocalStreamReader, LocalBufferStream, LocalNetStream };
                String[] strmsNames = { "m_strWrtr", "m_strRedr", "m_bufStm", "m_netStm" };
                bool bflg = false;
                for (int i = 0; i < strms.Length && !bflg; i++)
                {
                    if (null == strms[i]) { continue; }
                    try
                    {
                        strms[i].GetType().InvokeMember("Close", BindingFlags.InvokeMethod, null, strms[i], new object[] { });
                    }
                    catch (Exception ex)
                    {
                        //削除失敗
                        if (ex.InnerException.InnerException is ObjectDisposedException)
                        {
                        }
                        else
                        {
                        }
                    }
                    finally
                    {
                        strms[i] = null;
                    }

                }
            }
            /*            catch (Exception ex) {
                            LogUtility.OutputDebugLog("E023"+ex.Message);
                        }*/
            finally
            {

            }
        }
    }
}
