using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;

namespace com.brainsellers.bizstream.print
{
    /**
     * <summary>
     * ダイレクト印刷の共通クラスです。
     * </summary>
     * <history>
     *  [rikiya]    2006/10/24    Created
     *  [swata]     2017/08/01    Changed
     * </history>
     */
    public abstract class PDFCommonPrintStream
    {
        /// <summary>上段トレイ</summary>
        public static readonly String TRAY_UPPER = "UPPER";

        /// <summary>中段トレイ</summary>
        public static readonly String TRAY_MIDDLE = "MIDDLE";

        /// <summary>下段トレイ</summary>
        public static readonly String TRAY_LOWER = "LOWER";

        /// <summary>手差しトレイ</summary>
        public static readonly String TRAY_MANUAL = "MANUAL";

        /// <summary>自動選択</summary>
        public static readonly String TRAY_AUTO = "AUTO";

        /// <summary>デバッグ・フラグ</summary>
        protected static Boolean DEBUG_FLAG = false;

        /// <summary>デバッグ保存先</summary>
        protected static String DEBUG_SAVE = null;

        /**
         *	デバッグ最終時刻
         */
        protected static long DEBUG_LAST = 0;

        /// <summary>PDF バッファ</summary>
        protected MemoryStream pdfdoc;

        /// <summary>プリンタ名</summary>
        protected String printerName;

        /// <summary>印刷部数</summary>
        protected Int32 numberOfCopy;

        /// <summary>出力トレイ</summary>
        protected String selectedTray;

        /// <summary>印刷識別子</summary>
        protected String jobName;

        /// <summary>ページサイズに合わせて印刷</summary>
        protected bool doFit;

        /// <summary>印刷範囲指定開始ページ</summary>
        protected int fromPage;

        /// <summary>印刷範囲指定終了ページ</summary>
        protected int toPage;

        /// <summary>
        /// インスタンスを生成し、指定された HTTP 応答オブジェクトで初期化します。
        /// </summary>
        public PDFCommonPrintStream()
        {
            pdfdoc = new MemoryStream();

            printerName = null;
            numberOfCopy = 1;
            selectedTray = null;
            jobName = null;
            doFit = true;
            fromPage = -1;
            toPage = -1;
        }
        
        /// <summary>
        /// 文字列を URL 文字列に変換します。
        /// </summary>
        /// <param name="text">文字列</param>
        /// <returns>URL 文字列</returns>
        protected String encode(String text)
        {
            return HttpUtility.UrlEncode(text, Encoding.UTF8);
        }

        /// <summary>
        /// パラメータ文字列を返します。
        /// </summary>
        /// <returns>パラメータ文字列</returns>
        protected String parameters()
        {
            StringBuilder buf = new StringBuilder();
            String sep = "\n";

            if (printerName != null)
            {
                buf.Append(sep + "printerName=" + printerName);
                sep = "\n";
            }

            buf.Append(sep + "numberOfCopy=" + numberOfCopy.ToString());
            sep = "\n";

            if (selectedTray != null)
            {
                buf.Append(sep + "selectedTray=" + selectedTray);
                sep = "\n";
            }

            if (jobName != null)
            {
                buf.Append(sep + "jobName=" + jobName);
                sep = "\n";
            }
            else
            {
                jobName = "JobName_Default";
                buf.Append(sep + "jobName=" + jobName);
            }

            if (doFit == false)
            {
                buf.Append(sep + "doFit=false");
            }
            else {
                buf.Append(sep + "doFit=true");
            }

            if (fromPage > 0)
            {
                buf.Append(sep + "fromPage=" + fromPage.ToString());
            }

            if ((toPage > 0) && (toPage >= fromPage))
            {
                buf.Append(sep + "toPage=" + toPage.ToString());
            }


            return buf.ToString();
        }

        /// <summary>
        /// バッファフラッシュします。
        /// </summary>
        public void Flush()
        {
            pdfdoc.Flush();
        }

        /// <summary>
        /// PDF データをバッファに書き出します。 
        /// </summary>
        /// <param name="pdf">PDF データ</param>
        public void WriteByte(byte pdf)
        {
            pdfdoc.WriteByte(pdf);
        }

        /// <summary>
        /// PDF データを指定位置から指定バイト数だけバッファに書き出します。 
        /// </summary>
        /// <param name="pdf">PDF データ</param>
        /// <param name="offset">開始位置</param>
        /// <param name="length">バイト数</param>
        public void Write(byte[] pdf, int offset, int length)
        {
            pdfdoc.Write(pdf, offset, length);
        }


        /// <summary>
        /// PDF データをバッファに書き出します
        /// </summary>
        /// <param name="pdf">PDF データ</param>
        public void Write(byte[] pdf)
        {
            pdfdoc.Write(pdf, 0, pdf.Length);
        }

        /// <summary>書き込まれたバイト数を返します。</summary>
        /// <returns>書き込まれたバイト数</returns>
        public long size()
        {
            return pdfdoc.Length;
        }

        /// <summary>プリンタ名をセットします。</summary>
        /// <param name="value">プリンタ名</param>

        public void setPrinterName(String value)
        {
            printerName = value;
        }

        /// <summary>印刷部数をセットします。</summary>
        /// <param name="value">印刷部数</param>
        public void setNumberOfCopy(int value)
        {
            if (value < 1)
            {
                //throw new ArgumentException();
                value = 1;
            }

            if (value > 999)
            {
                value = 999;
            }

            numberOfCopy = value;
        }

        /// <summary>出力トレイをセットします。</summary>
        /// <param name="value">出力トレイ</param>
        public void setSelectedTray(String value)
        {
            selectedTray = value;
        }

        /// <summary>印刷識別子をセットします。</summary>
        /// <param name="value">印刷識別子</param>

        public void setJobName(String value)
        {
            jobName = value;
        }

        /// <summary>印刷位置調整フラグをセットします。</summary>
        /// <param name="value">印刷位置調整フラグ</param>
        public void setDoFit(bool value)
        {
            doFit = value;
        }

        /// <summary>印刷範囲の開始ページをセットします。</summary>
        /// <param name="value">印刷開始ページ</param>
        public void setFromPage(int value)
        {
            fromPage = value;
        }

        /// <summary>印刷範囲の終了ページをセットします。</summary>
        /// <param name="value">印刷終了ページ</param>
        public void setToPage(int value)
        {
            toPage = value;
        }

    }
}
