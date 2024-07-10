/**
 *	@(#)PDFDirectPrintStream.java  1.07 2017/03/08
 *
 *	(C) Copyright BrainSellers.com Corporation. 2017 - All Rights Reserved.
 */

package com.brainsellers.pdf.print;

import java.io.IOException;
import java.io.OutputStream;
import java.text.SimpleDateFormat;
import java.util.Date;

import javax.servlet.http.HttpServletResponse;

import com.brainsellers.pdf.PDFRuntimeException;
import com.brainsellers.pdf.internal.PDFEncryptOutputStream;
import com.brainsellers.pdf.internal.PDFMessages;

/**
 *	オンライン・タイプのダイレクト印刷のストリームを生成するためのクラスです。
 *
 *		@version	1.07 2017/03/08
 *		@author		Ogu
 */
public class PDFDirectPrintStream extends PDFCommonPrintStream implements PDFEncryptOutputStream {

	/**
	 *	バージョン
	 */
	public static final String VERSION = "1.07 2017/03/08";

	/**
	 *	HTTP 応答オブジェクト
	 */
	protected HttpServletResponse response;

	/**
	 *	印刷応答 URL
	 */
	protected String responseUrl;

	/**
	 *	終了処理
	 *	@deprecated v5.0より廃止されました。
	 */
	protected String disposal;

	/**
	 *	ファイル保存
	 */
	protected String saveFileName;

	/**
	 *	印刷禁止フラグ
	 *	@deprecated v5.0より廃止されました。
	 */
	protected Boolean printDeny;

	/**
	 *	ブラウザ target 名
	 */
	protected String target;

	/**
	 *	印刷位置調整フラグ
	 *	@deprecated v5.0より廃止されました。
	 */
	protected Boolean printAdjust;

	/**
	 *	印刷ダイアログ表示フラグ
	 */
	protected Boolean printDialog;

	/**
	 *	暗号化フラグ
	 *	@deprecated v5.0より廃止されました。
	 */
	protected Boolean encryption;

	/**
	 *	ダウンロードファイル名の一意化フラグ
	 */
	protected Boolean sppnameUnified = true;


	/**
	 *	インスタンスを生成し、指定された HTTP 応答オブジェクトで初期化します。
	 *
	 *		@param		response	HTTP 応答オブジェクト
	 */
	public PDFDirectPrintStream(HttpServletResponse response) {
		super();


		this.response = response;

		responseUrl  = null;
		disposal     = null;
		saveFileName = null;
		printDeny    = null;
		target       = null;
		printAdjust  = null;
		printDialog  = null;
		encryption   = null;
	}

	/**
	 *	バッファをクローズします。
	 */
	public void close() throws IOException ,PDFRuntimeException{
		pdfdoc.close();

		CreateEncryptSpp cesp = new CreateEncryptSpp();

		StringBuilder tmp = new StringBuilder(parameters());
		String sep = tmp.length() == 0 ? "" : "\n";

		if (responseUrl != null) {
			tmp.append(sep + "responseURL=" + responseUrl);
			sep = "\n";
		}
		if (saveFileName != null) {
			tmp.append(sep + "saveFileName=" + saveFileName);
			sep = "\n";
		}
		if (target != null) {
			tmp.append(sep + "target=" + target);
			sep = "\n";
		}
		if (printDialog != null) {
			tmp.append(sep + "printDialog=" + (String.valueOf(printDialog.booleanValue())));
			sep = "\n";
		}

		tmp.append(sep);

		cesp.setPassword(userPassword);
		byte[]  sendspp = cesp.createCompressedSpp(tmp.toString(), jobName, pdfdoc);
		if(sendspp==null){
			logger.error(PDFMessages.getMessage(1428, null));
			PDFRuntimeException.throwException(1428, PDFMessages.getMessage(1428), null);
		}

		response.setContentLength(sendspp.length);
		response.setContentType("application/x-spp");

		//実行ごとにファイル名を変える
		if(sppnameUnified){
			//現在時刻からファイル名作成
			Date nowdate = new Date();
			int rand = (int) (Math.random() * 1000);

			String fname_now = new SimpleDateFormat("yyyyMMdd_HHmmss").format(nowdate) + "_" + String.format("%04d", rand) + ".spp";

			//attachmentだとブラウザによってはダウンロード後に手動ファイル保存になるためinlineにする
			response.addHeader("Content-Disposition", "inline; filename=" + fname_now );
		}

		OutputStream output = getOutput(response.getOutputStream());

		output.write(sendspp);
		output.close();
	}

	/**
	 *	印刷応答 URL をセットします。
	 *
	 *		@param		value		印刷応答 URL
	 */
	public void setResponseUrl(String value) {
		responseUrl = value;
	}

	/**
	 *	終了処理を TERMINATE モードにセットします。
	 *	@deprecated v5.0より廃止されました。
	 */
	public void setTerminate() {

	}

	/**
	 *	終了処理を REGIDENT モードにセットします。
	 *	@deprecated v5.0より廃止されました。
	 */
	public void setRegident() {

	}

	/**
	 *	ファイル保存をセットします。
	 *
	 *		@param		value		ファイル保存
	 */
	public void setSaveFileName(String value) {
		saveFileName = value;
	}

	/**
	 *	印刷禁止フラグをセットします。
	 *
	 *		@param		value		印刷禁止フラグ
	 *		@deprecated v5.0より廃止されました。
	 */
	public void setPrintDeny(boolean value) {

	}

	/**
	 *	ブラウザ target 名をセットします。
	 *
	 *		@param		value		ブラウザ target 名
	 */
	public void setTarget(String value) {
		target = value;
	}

	/**
	 *	印刷位置調整フラグをセットします。
	 *
	 *		@param		value		印刷位置調整フラグ
	 *		@deprecated v5.0より廃止されました。
	 */
	public void setPrintAdjust(boolean value) {

	}

	/**
	 *	印刷ダイアログ表示フラグをセットします。<BR>
	 *	<BR>
	 *	このメソッドでtrue(印刷ダイアログを表示する)を指定した場合、以下のメソッドでの指定は無視されます。<BR>
	 *	<UL>
	 *	<LI>{@link PDFCommonPrintStream#setNumberOfCopy PDFCommonPrintStream#setNumberOfCopy(印刷部数)}
	 *	<LI>{@link PDFCommonPrintStream#setFromPage PDFCommonPrintStream#setFromPage(開始ページ番号)}
	 *	<LI>{@link PDFCommonPrintStream#setToPage PDFCommonPrintStream#setToPage(終了ページ番号)}
	 *	<LI>{@link PDFCommonPrintStream#setDoFit PDFCommonPrintStream#setDoFit(ページサイズに合わせて印刷フラグ)}
	 *	</UL>
	 *
	 *		@param		value		印刷ダイアログ表示フラグ
	 */
	public void setPrintDialog(boolean value) {
		printDialog = new Boolean(value);
	}

	/**
	 *	暗号化するようにセットします。
	 *		@deprecated v5.0より廃止されました。
	 */
	public void encryptPrintFile() {

	}

	/**
	 *	暗号化するかどうかを返します。
	 *
	 *		@return		true の場合、暗号化する
	 *		@deprecated v5.0より廃止されました。
	 */
	public boolean isEncrypt() {
			return false;
	}

	/**
	 *	SPPファイル名を一意化するかを指定します(デフォルト: true)
	 *		@param		value		true: 一意化する, false: 一意化しない
	 */
    public void setSppNameUnified(Boolean value) {
    	sppnameUnified = value;
    }
}
