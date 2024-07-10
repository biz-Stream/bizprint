/**
 *	@(#)PDFBatchPrintStream.java  1.04 2017/03/08
 *
 *	(C) Copyright BrainSellers.com Corporation. 2017 - All Rights Reserved.
 */

package com.brainsellers.pdf.print;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.UnsupportedEncodingException;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLDecoder;
import java.util.StringTokenizer;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.brainsellers.pdf.PDFRuntimeException;
import com.brainsellers.pdf.internal.PDFMessages;

/**
 *	�o�b�`�E�^�C�v�̃_�C���N�g����̃X�g���[���𐶐����邽�߂̃N���X�ł��B
 *
 *		@version	 1.04 2017/03/08
 *		@author		Ogu
 */
public class PDFBatchPrintStream extends PDFCommonPrintStream {
	static final Logger logger = LoggerFactory.getLogger(PDFBatchPrintStream.class);

	/**
	 *	�o�[�W����
	 */
	public static final String VERSION = "1.04 2017/03/08";

	/**
	 *	�T�[�o URL
	 */
	protected String serverUrl;

	/**
	 *	����
	 */
	protected String result;

	/**
	 *	�G���[�R�[�h
	 */
	protected String errorCode;

	/**
	 *	�G���[�̌���
	 */
	protected String errorCause;

	/**
	 *	�G���[�̓��e
	 */
	protected String errorDetails;

	/**
	 *	����w�� ID
	 */
	protected String jobId;

	private static String DEFAULT_PORT = "3000";

	/**
	 *	�ڑ���|�[�g�ԍ�
	 */
	protected String portno = DEFAULT_PORT;

	/**
	 *	�������Ɏg�p���ꂽURL
	 */
	protected String inputUrl = "";

	/**
	 *	�f�t�H���g��3000�ȊO���|�[�g�ԍ��Ƃ��Ďw�肵�܂��B
	 *		@param		 value		1024�`65535�܂ł̐����l�݂̂��w��ł��܂��B����ȊO���w�肵���ꍇ�̓f�t�H���g�|�[�g�̂܂܂ł��B
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
	 *	�C���X�^���X�𐶐����A�w�肳�ꂽ�T�[�o URL �ŏ��������܂��B
	 *
	 *		@param		inputServerUrl	�T�[�o URL
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
	 * �ڑ��Ɏg�p����URL���쐬���܂��B
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
		//�|�[�g�ԍ��̎w�肪���ɂ��邩�`�F�b�N���A�����ꍇ�͕t��������
		if (coronpos == 4 || coronpos < 0) {
			this.serverUrl = this.serverUrl + ":" + portno;
		} else {
			//":"�̌�낪�|�[�g�ԍ��Ƃ��Đ������Ȃ��ꍇ�̃`�F�b�N�͂��Ȃ��B���[�U�w��ʂ�ɂ���
		}

		if (this.serverUrl.indexOf("/doprint") < 0){
			this.serverUrl = this.serverUrl + "/doprint";
		}

	}

	/**
	 *	URL ������𕶎���ɕϊ����܂��B
	 *
	 *		@param		text		URL ������
	 *
	 *		@return		������
	 */
	protected String decode(String text) {
		try {
			return URLDecoder.decode(text, "UTF-8");
		}
		catch (NoSuchMethodError e) {
			return URLDecoder.decode(text);
		}
		catch (UnsupportedEncodingException e) {
			return URLDecoder.decode(text);
		}
	}

	/**
	 *	�o�b�t�@���N���[�Y���܂��B
	 */
	public void close() throws IOException {
		createConnectUrl();
		pdfdoc.close();
		CreateEncryptSpp cesp = new CreateEncryptSpp();

		StringBuilder tmp = new StringBuilder(parameters());
		String sep = tmp.length() == 0 ? "" : "\n";


		tmp.append("\n");//�ŏI�s�ɉ��s

		cesp.setPassword(userPassword);
		byte[] sendspp = cesp.createCompressedSpp(tmp.toString(), jobName, pdfdoc);
		if (sendspp == null) {
			logger.error(PDFMessages.getMessage(1428, null));
			PDFRuntimeException.throwException(true, 1428, PDFMessages.getMessage(1428), null);
		}

		try {
			URL url = new URL(serverUrl);

			HttpURLConnection connection = (HttpURLConnection) url.openConnection();
			connection.setDoOutput(true);
			connection.setDoInput(true);
			connection.setRequestMethod("POST");

			connection.connect();

			OutputStream output = getOutput(connection.getOutputStream());
			int size = sendspp.length;

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
	 *	���ʂ�Ԃ��܂��B
	 *
	 *		@return		����
	 */
	public String getResult() {
		return result;
	}

	/**
	 *	�G���[�R�[�h��Ԃ��܂��B
	 *
	 *		@return		�G���[�R�[�h
	 */
	public String getErrorCode() {
		return errorCode;
	}

	/**
	 *	�G���[�̌�����Ԃ��܂��B
	 *
	 *		@return		�G���[�̌���
	 */
	public String getErrorCause() {
		return errorCause;
	}

	/**
	 *	�G���[�̓��e��Ԃ��܂��B
	 *
	 *		@return		�G���[�̓��e
	 */
	public String getErrorDetails() {
		return errorDetails;
	}

	/**
	 *	����w�� ID ��Ԃ��܂��B
	 *
	 *		@return		����w�� ID
	 */
	public String getJobId() {
		return jobId;
	}
}
