/**
 *	@(#)PDFBatchStatus.java  1.04 2017/03/08
 *
 *	(C) Copyright BrainSellers.com Corporation. 2017 - All Rights Reserved.
 */

package com.brainsellers.pdf.print;

import java.io.BufferedReader;
import java.io.ByteArrayInputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.UnsupportedEncodingException;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLDecoder;
import java.net.URLEncoder;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Hashtable;
import java.util.Iterator;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.brainsellers.pdf.PDFRuntimeException;
import com.brainsellers.pdf.internal.PDFMessages;

/**
 *	バッチ印刷の印刷ステータスを取得するためのクラスです。
 *
 *		@version	1.04 2017/03/08
 *		@author		Ogu
 */
public class PDFBatchStatus extends Object {
	static final Logger logger = LoggerFactory.getLogger(PDFBatchStatus.class);

	/**
	 *	バージョン
	 */
	public static final String VERSION = "1.04 2017/03/08";

	/**
	 *	サーバ URL
	 */
	protected String serverUrl;

	/**
	 *	結果
	 */
	protected String result;

	/**
	 *	エラーコード
	 */
	protected String errorCode;

	/**
	 *	エラーの原因
	 */
	protected String errorCause;

	/**
	 *	エラーの内容
	 */
	protected String errorDetails;

	/**
	 *	印刷ステータスのインデックス
	 */
	protected Hashtable statusIndex;

	/**
	 *	印刷ステータスのリスト
	 */
	protected ArrayList statusArray;

	/**
	 * 接続先ポート番号
	 */
	protected String portno;

	private static String DEFAULT_PORT = "3000";

	/**
	 * 初期化に使用されたURL
	 */
	protected String inputUrl = "";

	/**
	 *	インスタンスを生成し、指定されたサーバ URL で初期化します。
	 *
	 *		@param		serverUrl	サーバ URL
	 */
	public PDFBatchStatus(String inputServerUrl) {
		super();
		inputUrl = inputServerUrl;
		this.serverUrl = inputUrl;

		result       = null;
		errorCode    = null;
		errorCause   = null;
		errorDetails = null;

		statusIndex = new Hashtable();
		statusArray = new ArrayList();
	}

	/**
	 *	文字列を URL 文字列に変換します。
	 *
	 *		@param		text		文字列
	 *
	 *		@return		URL 文字列
	 */
	protected String encode(String text) {
		try {
			return URLEncoder.encode(text, "UTF-8");
		}
		catch (NoSuchMethodError e) {
			return PDFUrlTools.encode(text);
		}
		catch (UnsupportedEncodingException e) {
			return PDFUrlTools.encode(text);
		}
	}

	/**
	 *	URL 文字列を文字列に変換します。
	 *
	 *		@param		text		URL 文字列
	 *
	 *		@return		文字列
	 */
	protected String decode(String text) {
		try {
			return URLDecoder.decode(text, "UTF-8");
		}
		catch (NoSuchMethodError e) {
			return PDFUrlTools.decode(text);
		}
		catch (UnsupportedEncodingException e) {
			return PDFUrlTools.decode(text);
		}
	}

	/**
	 *	印刷ステータスを取得します。
	 *
	 *		@param		jobId		印刷指示 ID
	 */
	public void query(String jobId) {
		createConnectUrl();
		try {
			URL url = new URL(serverUrl);

			HttpURLConnection connection = (HttpURLConnection) url.openConnection();
			connection.setDoOutput(true);
			connection.setDoInput(true);
//			connection.setAllowUserInteraction(false);
			connection.setRequestMethod("POST");
//			connection.setInstanceFollowRedirects(false);
//			connection.setRequestProperty("accept-Language", "ja;q=0.7,en;q=0.3");

			connection.connect();

			OutputStream output = connection.getOutputStream();

			if (jobId != null) {
				output.write(("jobID=" + encode(jobId)).getBytes());
			}
			else {
				output.write("jobID=".getBytes());
			}

			output.close();

			int response = connection.getResponseCode();

			if (response == 200) {
				BufferedReader reader = new BufferedReader(new InputStreamReader(connection.getInputStream()));
				StringBuilder buf = new StringBuilder();
				String line;

				while ((line = reader.readLine()) != null) {
					buf.append(decode(line));
				}

				reader.close();

				PDFCommonStatusParser parser = PDFCommonStatusParser.getInstance();

				logger.info(buf.toString());

				//明示的に文字コードを指定 v5.0.0
				parser.parse(new ByteArrayInputStream(buf.toString().getBytes("Windows-31j")));

				result = parser.getResult();
				errorCode = parser.getErrorCode();
				errorCause = parser.getErrorCause();
				errorDetails = parser.getErrorDetails();

				for (Iterator iterator = parser.getPrintStatus().iterator(); iterator.hasNext(); ) {
					PDFBatchPrintStatus status = (PDFBatchPrintStatus) iterator.next();
					statusIndex.put(status.getJobId(), status);
					statusArray.add(status);
				}
			}
			else {
				logger.error(PDFMessages.getMessage(1400, Integer.toString(response)));
				PDFRuntimeException.throwException(1400, PDFMessages.getMessage(1400, Integer.toString(response)), null);
			}
		}
		catch (Throwable t) {
			logger.error(PDFMessages.getMessage(1401, t.getLocalizedMessage()));
			PDFRuntimeException.throwException(1401, PDFMessages.getMessage(1401, t.getLocalizedMessage()), t);
		}
	}

	/**
	 *	印刷ステータスを取得します。
	 */
	public void query() {
		query(null);
	}

	/**
	 *	結果を返します。
	 *
	 *		@return		結果
	 */
	public String getResult() {
		return result;
	}

	/**
	 *	エラーコードを返します。
	 *
	 *		@return		エラーコード
	 */
	public String getErrorCode() {
		return errorCode;
	}

	/**
	 *	エラーの原因を返します。
	 *
	 *		@return		エラーの原因
	 */
	public String getErrorCause() {
		return errorCause;
	}

	/**
	 *	エラーの内容を返します。
	 *
	 *		@return		エラーの内容
	 */
	public String getErrorDetails() {
		return errorDetails;
	}

	/**
	 *	印刷ステータスのリストを返します。
	 *
	 *		@return		印刷ステータスのリスト
	 */
	public Collection getPrintStatus() {
		return (Collection) statusArray.clone();
	}

	/**
	 *	印刷ステータスを返します。
	 *
	 *		@param		jobId
	 *
	 *		@return		印刷ステータス
	 */
	public PDFBatchPrintStatus getPrintStatus(String jobId) {
		return (PDFBatchPrintStatus) statusIndex.get(jobId);
	}

	/**
	 *	デフォルトの3000以外をポート番号として指定します。
	 *		@param		value		1024～65535までの整数値のみが指定できます。それ以外を指定した場合はデフォルトポートのままです。
	 */
	public void setBatchStatusPort(String value) {
		if (value != null && value.length() != 0) {
			try {
				int no = Integer.parseInt(value);
				if (1024 <= no && no <= 65535) {
					portno = value;
				} else {
					portno = DEFAULT_PORT;
				}
			} catch (NumberFormatException e) {
				portno = DEFAULT_PORT;
			}
		}
	}

	/**
	 *	接続に使用するURLを作成します。
	 */
	public void createConnectUrl(){

		if (inputUrl.startsWith("http://")) {
			this.serverUrl = inputUrl;
		}
		else {
			this.serverUrl = "http://" + inputUrl;
		}

		if (this.serverUrl.lastIndexOf("/") == this.serverUrl.length() - 1) {
			this.serverUrl = this.serverUrl.substring(0, this.serverUrl.length() - 1);
		}
		int coronpos = this.serverUrl.lastIndexOf(":");
		//ポート番号の指定が既にあるかチェックし、無い場合は付け加える
		if (coronpos == 4 || coronpos < 0) {
			this.serverUrl = this.serverUrl + ":" + portno;
		} else {
			//":"の後ろがポート番号として正しくない場合のチェックはしない。ユーザ指定通りにする
		}

		if (this.serverUrl.indexOf("/getstatus") < 0){
			this.serverUrl = this.serverUrl + "/getstatus";
		}
	}
}
