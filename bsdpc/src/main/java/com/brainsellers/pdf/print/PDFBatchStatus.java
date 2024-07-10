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
 *	�o�b�`����̈���X�e�[�^�X���擾���邽�߂̃N���X�ł��B
 *
 *		@version	1.04 2017/03/08
 *		@author		Ogu
 */
public class PDFBatchStatus extends Object {
	static final Logger logger = LoggerFactory.getLogger(PDFBatchStatus.class);

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
	 *	����X�e�[�^�X�̃C���f�b�N�X
	 */
	protected Hashtable statusIndex;

	/**
	 *	����X�e�[�^�X�̃��X�g
	 */
	protected ArrayList statusArray;

	/**
	 * �ڑ���|�[�g�ԍ�
	 */
	protected String portno;

	private static String DEFAULT_PORT = "3000";

	/**
	 * �������Ɏg�p���ꂽURL
	 */
	protected String inputUrl = "";

	/**
	 *	�C���X�^���X�𐶐����A�w�肳�ꂽ�T�[�o URL �ŏ��������܂��B
	 *
	 *		@param		serverUrl	�T�[�o URL
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
	 *	������� URL ������ɕϊ����܂��B
	 *
	 *		@param		text		������
	 *
	 *		@return		URL ������
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
			return PDFUrlTools.decode(text);
		}
		catch (UnsupportedEncodingException e) {
			return PDFUrlTools.decode(text);
		}
	}

	/**
	 *	����X�e�[�^�X���擾���܂��B
	 *
	 *		@param		jobId		����w�� ID
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

				//�����I�ɕ����R�[�h���w�� v5.0.0
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
	 *	����X�e�[�^�X���擾���܂��B
	 */
	public void query() {
		query(null);
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
	 *	����X�e�[�^�X�̃��X�g��Ԃ��܂��B
	 *
	 *		@return		����X�e�[�^�X�̃��X�g
	 */
	public Collection getPrintStatus() {
		return (Collection) statusArray.clone();
	}

	/**
	 *	����X�e�[�^�X��Ԃ��܂��B
	 *
	 *		@param		jobId
	 *
	 *		@return		����X�e�[�^�X
	 */
	public PDFBatchPrintStatus getPrintStatus(String jobId) {
		return (PDFBatchPrintStatus) statusIndex.get(jobId);
	}

	/**
	 *	�f�t�H���g��3000�ȊO���|�[�g�ԍ��Ƃ��Ďw�肵�܂��B
	 *		@param		value		1024�`65535�܂ł̐����l�݂̂��w��ł��܂��B����ȊO���w�肵���ꍇ�̓f�t�H���g�|�[�g�̂܂܂ł��B
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
	 *	�ڑ��Ɏg�p����URL���쐬���܂��B
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

		if (this.serverUrl.indexOf("/getstatus") < 0){
			this.serverUrl = this.serverUrl + "/getstatus";
		}
	}
}
