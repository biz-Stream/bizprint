using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizPrintCommon
{
    /// <summary>
    /// sppファイル解凍クラス
    /// </summary>
    public class SppExtracter
    {

        /// <summary>
        /// パラメータファイルバイト配列
        /// </summary>
        public byte[] ParamFileByte { private set; get; } = null;
        /// <summary>
        /// PDFファイルバイト配列
        /// </summary>
        public byte[] PdfFileByte { private set; get; } = null;
        /// <summary>
        /// PDFファイル名
        /// </summary>
        public string PdfFileName { private set; get; } = "";

        /// <summary>
        /// SPPファイル解凍パスワード文字列
        /// </summary>
        private string SppPassword { set; get; } = "";
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SppExtracter()
        {
        }

        /// <summary>
        /// パスワード文字列初期化。デコードして固定文字列と組み合わせ
        /// </summary>
        /// <param name="settedPass"></param>
        /// <returns></returns>
        public int InitPass(string settedPass)
        {
            int rtn = ErrCodeAndmErrMsg.STATUS_OK;
            //空文字の場合、設定なしの場合は固定文字列だけ。
            if (settedPass == null || settedPass.Length == 0)
            {
                SppPassword = CommonConstants.SPPPATH_BEFORE + CommonConstants.SPPPATH_AFTER;
                return ErrCodeAndmErrMsg.STATUS_OK;
            }
            //base64デコード
            try
            {
                byte[] decbytes = Convert.FromBase64String(settedPass);
                Encoding enc = Encoding.GetEncoding("UTF-8");
                string decstr = enc.GetString(decbytes);

                //前後の固定文字列足す
                SppPassword = CommonConstants.SPPPATH_BEFORE + decstr + CommonConstants.SPPPATH_AFTER;
            }
            catch (Exception ex)
            {
                //例外エラー
                LogUtility.OutputLog("055", settedPass, ex.Message);
                rtn = ErrCodeAndmErrMsg.ERR_CODE_0102;
            }
            return rtn;

        }
        /// <summary>
        /// 解凍実行
        /// </summary>
        /// <param name="input">sppファイルのバイト配列</param>
        /// <returns></returns>
        public int DoExtract(byte[] input)
        {

            LogUtility.OutputLog("051");

            MemoryStream sppMemStream = new MemoryStream(input);
            //zipライブラリで解凍

            try
            {
                ZipFile exZipFile = ZipFile.Read(sppMemStream);
                if (exZipFile.Entries.Count != 2)
                {
                    //ファイル数足りないのでError
                    LogUtility.OutputLog("035", "SPP Data Num Error", exZipFile.Entries.Count.ToString());
                    return ErrCodeAndmErrMsg.ERR_CODE_0103;
                }

                foreach (Ionic.Zip.ZipEntry zipEntry in exZipFile)
                {
                    bool passFlg = false;
                    string origName = zipEntry.FileName;
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    try
                    {
                        //パスワード無しor2個目以降は設定されたパスワードで解凍try
                        zipEntry.Extract(ms);
                    }
                    catch (Ionic.Zip.BadPasswordException err)
                    {
                        //パスワードがあるので1個目はここは必ずException
                        string errlog = err.Message;
                        passFlg = true;
                    }
                    if (passFlg)
                    {
                        //パスワード設定
                        exZipFile.Password = SppPassword;


                        try
                        {
                            zipEntry.Extract(ms);//パスワード違うとException
                        }
                        catch (Ionic.Zip.BadPasswordException err)
                        {
                            string errlog = err.Message;
                            //パスワード違うとException
                            //パスワード付けても開けないエラー
                            //log 108
                            LogUtility.OutputLog("035", "Password Error", ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrCodeAndmErrMsg.ERR_CODE_0102));
                            LogUtility.OutputDebugLog("E092", err.Message);
                            return ErrCodeAndmErrMsg.ERR_CODE_0102;

                        }
                    }
                    byte[] fileByte = ms.ToArray();
                    if (origName.Equals("param.txt"))
                    {
                        ParamFileByte = fileByte;
                    }
                    else
                    {
                        PdfFileByte = fileByte;
                        PdfFileName = origName;
                    }

                }
                if (PdfFileByte == null || PdfFileByte.Length == 0)
                {
                    LogUtility.OutputLog("035", "PDF Size Error", ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrCodeAndmErrMsg.ERR_CODE_0104));
                    return ErrCodeAndmErrMsg.ERR_CODE_0104;
                }
                if (ParamFileByte == null)
                {
                    LogUtility.OutputLog("035", "Param Size Error", ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrCodeAndmErrMsg.ERR_CODE_0105));
                    return ErrCodeAndmErrMsg.ERR_CODE_0105;
                }
                //PDFファイル名称異常
                if (PdfFileName.Length == 0 || !PdfFileName.EndsWith(".pdf"))
                {
                    LogUtility.OutputLog("035", "PDF FileName Error", ErrCodeAndmErrMsg.ChangeCodeToDetail(ErrCodeAndmErrMsg.ERR_CODE_0104));
                    return ErrCodeAndmErrMsg.ERR_CODE_0104;
                }

                LogUtility.OutputLog("034", PdfFileName, PdfFileByte.Length.ToString());
                return ErrCodeAndmErrMsg.STATUS_OK;
            }
            catch (Exception ex)
            {
                LogUtility.OutputDebugLog("E093", ex.Message);
                return ErrCodeAndmErrMsg.ERR_CODE_0102;
            }
        }

    }
}
