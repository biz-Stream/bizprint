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
using System.Text;
using System.IO;
using System.Web;

[assembly: CLSCompliant(true)]

namespace com.brainsellers.bizstream.bizprint
{
    /**
     * <summary>
     * ダイレクト印刷のストリームを生成するためのクラスです。
     * </summary>
     */
    public class PDFDirectPrintStream : PDFCommonPrintStream
    {
        /// <summary>HTTP 応答オブジェクト</summary>
        protected HttpResponse response;

        /// <summary>印刷応答 URL</summary>
        protected String responseUrl;

        /// <summary>ファイル保存</summary>
        protected String saveFileName;

        /// <summary>ブラウザ target 名</summary>
        protected String target;

        /// <summary>印刷ダイアログ表示フラグ</summary>
        protected Boolean printDialog;

        /// <summary>sppファイル名の一意化</summary>
        protected Boolean sppnameUnified = true;

        /// <summary>印刷応答 URL</summary>
        protected String userPassword;

        /**
         * <summary>
         * <c>PDFDirectPrintStream</c>クラスの新規インスタンスを生成し初期化します。
         * </summary>
         * <param name="response">HttpResponseオブジェクト</param>
         * <remarks>
         * <c>PDFDirectPrintStream</c>クラスの新規インスタンスを生成し、
         * 指定された HTTP 応答オブジェクトで初期化します。
         * </remarks>
         * <example>
         * <code>
         *    Dim direct As PDFDirectPrintStream
         *    direct = New PDFDirectPrintStream(Response)
         * </code>
         * </example>
         */
        public PDFDirectPrintStream(HttpResponse response)
        {
            this.response = response;
            this.response.Clear();

            responseUrl = null;
            saveFileName = null;
            target = null;
            printDialog = false;
            userPassword = "";
        }

        /**
         * <summary>印刷ダイアログ表示フラグをセットします。</summary>
         * <param name="value">印刷ダイアログ表示フラグ</param>
         * <remarks>
         * <para>このメソッドでtrue(印刷ダイアログを表示する)を指定した場合、以下のメソッドでの指定は無視されます。</para>
         * <para>PDFCommonPrintStream#setNumberOfCopy(印刷部数)</para>
         * <para>PDFCommonPrintStream#setFromPage(開始ページ番号)</para>
         * <para>PDFCommonPrintStream#setToPage(終了ページ番号)</para>
         * <para>PDFCommonPrintStream#setDoFit(ページサイズに合わせて印刷フラグ)</para>
         * </remarks>
         */
        public void setPrintDialog(Boolean value)
        {
            printDialog = value;
        }

        /**
         * <summary>印刷応答 URL をセットします。</summary>
         * <param name="value">印刷応答 URL</param>
         */
        public void setResponseUrl(String value)
        {
            responseUrl = value;
        }

        /**
         * <summary>ファイル保存をセットします。</summary>
         * <param name="value">ファイル保存</param>
         */
        public void setSaveFileName(String value)
        {
            saveFileName = value;
        }

        /**
         * <summary>ブラウザ target 名をセットします。</summary>
         * <param name="value">ブラウザ target 名</param>
         */
        public void setTarget(String value)
        {
            target = value;
        }

        /**
         * <summary>ユーザパスワードをセットします。(平文)</summary>
         * <param name="value">平文パスワード</param>
         */
        public void setPassword(String value)
        {
            if (value == null)
            {
                throw new ArgumentException("setPassword");
            }
            userPassword = value;
        }

        /**
         * <summary>ユーザパスワードをセットします。(base64エンコード済み)</summary>
         * <param name="value">base64エンコード済みパスワード</param>
         */
        public void setPasswordWithEncoded(String value)
        {
            if (value == null)
            {
                //例外Throw
                throw new ArgumentException("setPasswordWithEncoded");
            }

            try
            {
                byte[] decbytes = Convert.FromBase64String(value);
                Encoding enc = Encoding.GetEncoding("UTF-8");
                userPassword = enc.GetString(decbytes);

            }
            catch (Exception ex)
            {
                //例外Throw
                throw new ArgumentException("setPasswordWithEncoded Cannot Decode. " + ex.Message);
            }
        }

        /**
         * <summary>sppファイル名の一意化フラグをセットします。</summary>
         * <param name="value">trueの場合、sppファイル名を一意にします</param>
         */
        public void setSppNameUnified(Boolean value)
        {
            sppnameUnified = value;
        }

        /**
         * <summary>バッファをクローズします。</summary>
         */
        public void Close()
        {
            StringBuilder tmp = new StringBuilder(parameters());
            String sep = (tmp.Length == 0) ? "" : "\n";

            if (responseUrl != null)
            {
                tmp.Append(sep + "responseURL=" + responseUrl);
                sep = "\n";
            }

            if (saveFileName != null)
            {
                tmp.Append(sep + "saveFileName=" + saveFileName);
            }

            if (target != null)
            {
                tmp.Append(sep + "target=" + target);
            }
            tmp.Append(sep + "printDialog=" + printDialog.ToString().ToLower() + sep);


            CreateEncryptSpp createEnc = new CreateEncryptSpp();
            createEnc.setPassword(userPassword);
            byte[] sendspp = createEnc.createCompressedSpp(tmp.ToString(), jobName, pdfdoc.ToArray());

            response.ContentType = "application/x-spp";
            response.AddHeader("Content-Length", sendspp.Length.ToString());

            if (sppnameUnified)
            {
                //現在時刻からファイル名作成
                DateTime DT = new DateTime();
                DT = DateTime.Now;
                Random cRandom = new System.Random();
                int rand = cRandom.Next(1000);
                string fname_now = DT.ToString("_yyMMdd_HHmmss_") + rand.ToString("D4");
                response.AddHeader("Content-Disposition", "inline; filename=" + fname_now+".spp");
            }

            Stream output = response.OutputStream;

            output.Write(sendspp, 0, (int)sendspp.Length);
            output.Close();

            pdfdoc.Close();
        }
    }
}
