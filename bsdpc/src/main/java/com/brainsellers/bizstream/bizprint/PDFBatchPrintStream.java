/**
 *	@(#)PDFBatchPrintStream.java  1.04 2017/03/08
 *
 *	(C) Copyright BrainSellers.com Corporation. 2017 - All Rights Reserved.
 */

package com.brainsellers.bizstream.bizprint;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.UnsupportedEncodingException;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLDecoder;
import java.util.StringTokenizer;
import java.util.logging.Logger;

/**
 *	バッチ・タイプのダイレクト印刷のストリームを生成するためのクラスです。
 *
 *		@version	 1.04 2017/03/08
 *		@author		Ogu
 */
public class PDFBatchPrintStream extends PDFCommonPrintStream {
//	static final Logger logger = LoggerFactory.getLogger(PDFBatchPrintStream.class);
	private static final Logger LOGGER = Logger.getLogger(PDFBatchPrintStream.class.getName());

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
	 *	印刷指示 ID
	 */
	protected String jobId;

	private static String DEFAULT_PORT = "3000";

	/**
	 *	接続先ポート番号
	 */
	protected String portno = DEFAULT_PORT;

	/**
	 *	初期化に使用されたURL
	 */
	protected String inputUrl = "";

	/**
	 *	デフォルトの3000以外をポート番号として指定します。
	 *		@param		 value		1024～65535までの整数値のみが指定できます。それ以外を指定した場合はデフォルトポートのままです。
	 */
	public void setBatchPrintPort(String value) {
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
	 *	インスタンスを生成し、指定されたサーバ URL で初期化します。
	 *
	 *		@param		inputServerUrl	サーバ URL
	 */
	public PDFBatchPrintStream(String inputServerUrl) {
		super();

		inputUrl = inputServerUrl;
		this.serverUrl = inputUrl;

		fromPage = -1;
		toPage   = -1;

		result       = null;
		errorCode    = null;
		errorCause   = null;
		errorDetails = null;
		jobId        = null;
	}

	/**
	 * 接続に使用するURLを作成します。
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

		if (this.serverUrl.indexOf("/doprint") < 0){
			this.serverUrl = this.serverUrl + "/doprint";
		}

	}

	/**
	 *	URL 文字列を文字列に変換します。
	 *
	 *		@param		text		URL 文字列
	 *
	 *		@return		文字列
	 * @throws UnsupportedEncodingException
	 */
	protected String decode(String text) throws UnsupportedEncodingException {
//		try {
			return URLDecoder.decode(text, "UTF-8");
//		}
//		catch (NoSuchMethodError e) {
//			return URLDecoder.decode(text);
//		}
//		catch (UnsupportedEncodingException e) {
//			return URLDecoder.decode(text);
//		}
	}

	/**
	 *	バッファをクローズします。
	 */
	public void close() throws IOException {
		createConnectUrl();
		pdfdoc.close();
		CreateEncryptSpp cesp = new CreateEncryptSpp();

		StringBuilder tmp = new StringBuilder(parameters());
//		String sep = tmp.length() == 0 ? "" : "\n";


		tmp.append("\n");//最終行に改行

		cesp.setPassword(userPassword);
		byte[] sendspp = null;
		try {
			sendspp = cesp.createCompressedSpp(tmp.toString(), jobName, pdfdoc);
		} catch (Exception e) {
		}
		if (sendspp == null) {
//			logger.error(PDFMessages.getMessage(1428, null));
			LOGGER.severe("send spp file is NULL.");
//			PDFRuntimeException.throwException(true, 1428, PDFMessages.getMessage(1428), null);
			throw new IOException("send spp file is NULL.");
		}

		try {
			URL url = new URL(serverUrl);

			HttpURLConnection connection = (HttpURLConnection) url.openConnection();
			connection.setDoOutput(true);
			connection.setDoInput(true);
			connection.setRequestMethod("POST");

			connection.connect();

			OutputStream output = getOutput(connection.getOutputStream());
//			int size = sendspp.length;

			String head ="sppdata=";
			byte[] params = head.getBytes();



			output.write(params);
			output.write(sendspp);
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

				StringTokenizer tokenizer = new StringTokenizer(buf.toString(), "&");

				while (tokenizer.hasMoreTokens()) {
					String token = tokenizer.nextToken();
					int index = token.indexOf('=');

					if (index > 0) {
						String name  = token.substring(0, index);
						String value = token.substring(index + 1);

						if (name.equalsIgnoreCase("RESULT")) {
							result = value;
						}
						else if (name.equalsIgnoreCase("ERROR_CODE")) {
							errorCode = value;
						}
						else if (name.equalsIgnoreCase("ERROR_CAUSE")) {
							errorCause = value;
						}
						else if (name.equalsIgnoreCase("ERROR_DETAILS")) {
							errorDetails = value;
						}
						else if (name.equalsIgnoreCase("jobID")) {
							jobId = value;
						}
					}
				}
			}
			else {
//				logger.error(PDFMessages.getMessage(1400, Integer.toString(response)));
				LOGGER.severe("HTTP response(" + Integer.toString(response) + ") from Direct Print Server is not OK.");
//				PDFRuntimeException.throwException(1400, PDFMessages.getMessage(1400, Integer.toString(response)), null);
				throw new IOException("HTTP response(" + Integer.toString(response) + ") from Direct Print Server is not OK.");
			}
		}
		catch (IOException t) {
//			logger.error(PDFMessages.getMessage(1401, t.getLocalizedMessage()));
			LOGGER.severe(t.getMessage());
//			PDFRuntimeException.throwException(1401, PDFMessages.getMessage(1401, t.getLocalizedMessage()), t);
			throw t;
		}
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
	 *	印刷指示 ID を返します。
	 *
	 *		@return		印刷指示 ID
	 */
	public String getJobId() {
		return jobId;
	}
}
