/**
 *	@(#)PDFCommonPrintStream.java  1.05 2017/06/21
 *
 *	(C) Copyright BrainSellers.com Corporation. 2017 - All Rights Reserved.
 */

package com.brainsellers.pdf.print;

import java.io.ByteArrayOutputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.brainsellers.pdf.internal.PDFMessages;
import com.brainsellers.pdf.internal.PDFProperties;
import com.brainsellers.pdf.internal.PDFTrackedOutputStream;
import com.brainsellers.util.BASE64;
import com.brainsellers.util.DoubleOutputStream;

/**
 *	��ʓI�ȃ_�C���N�g����̃X�g���[���𐶐����邽�߂̒��ۃN���X�ł��B
 *
 *		@version	1.05 2017/06/21
 *		@author		Ogu
 */
public abstract class PDFCommonPrintStream extends OutputStream implements PDFTrackedOutputStream {
	static final Logger logger = LoggerFactory.getLogger(PDFCommonPrintStream.class);

	/**
	 *	�o�[�W����
	 */
	public static final String VERSION = "1.05 2017/06/21";

	/**
	 *	��i�g���C
	 */
	public static final String TRAY_UPPER	= "UPPER";

	/**
	 *	���i�g���C
	 */
	public static final String TRAY_MIDDLE	= "MIDDLE";

	/**
	 *	���i�g���C
	 */
	public static final String TRAY_LOWER	= "LOWER";

	/**
	 *	�荷���g���C
	 */
	public static final String TRAY_MANUAL	= "MANUAL";

	/**
	 *	�����I��
	 */
	public static final String TRAY_AUTO	= "AUTO";

	/**
	 *	�f�o�b�O�E�t���O
	 */
	protected static boolean DEBUG_FLAG		= false;

	/**
	 *	�f�o�b�O�ۑ���
	 */
	protected static String DEBUG_SAVE		= null;

	/**
	 *	�f�o�b�O�ŏI����
	 */
	protected static long DEBUG_LAST		= 0;

	/**
	 *	�N���X�E���[�h���̏���
	 */
	static {
		if (PDFProperties.getProperty("debug.direct.print", "false").equalsIgnoreCase("true")) {
			DEBUG_FLAG = true;
			DEBUG_SAVE = PDFProperties.getProperty("debug.direct.print.save", "./dp");
			DEBUG_LAST = System.currentTimeMillis();
		}
	}

	/**
	 *	PDF �o�b�t�@
	 */
	protected ByteArrayOutputStream pdfdoc;

	/**
	 *	�v�����^��
	 */
	protected String printerName;

	/**
	 *	�������
	 */
	protected Integer numberOfCopy;

	/**
	 *	�o�̓g���C
	 */
	protected String selectedTray;

	/**
	 *	������ʎq
	 */
	protected String jobName;

	/**
	 *	�y�[�W�T�C�Y�ɍ��킹�Ĉ��
	 */
	protected String doFit;
	/**
	 *	�J�n�y�[�W�ԍ�
	 */
	protected int fromPage;

	/**
	 *	�I���y�[�W�ԍ�
	 */
	protected int toPage;

	/**
	 * ���[�U�w��p�X���[�h������
	 */
	protected String userPassword = "";

	/**
	 *	�C���X�^���X�𐶐����A�w�肳�ꂽ HTTP �����I�u�W�F�N�g�ŏ��������܂��B
	 *
	 *		HTTP �����I�u�W�F�N�g
	 */
	public PDFCommonPrintStream() {
		super();

		pdfdoc = new ByteArrayOutputStream();

		printerName  = null;
		numberOfCopy = null;
		selectedTray = null;
		jobName      = null;
		doFit        = "false";
	}

	/**
	 *	�o�̓X�g���[����Ԃ��܂��B
	 *
	 *		@param		output		�o�̓X�g���[��
	 *
	 *		@return		�o�̓X�g���[��
	 */
	protected OutputStream getOutput(OutputStream output) {
		if (DEBUG_FLAG) {
			try {
				if (DEBUG_LAST == System.currentTimeMillis()) {
					DEBUG_LAST++;
				}
				else {
					DEBUG_LAST = System.currentTimeMillis();
				}

				return new DoubleOutputStream(output, new FileOutputStream(DEBUG_SAVE + DEBUG_LAST + ".spp"));
			}
			catch (FileNotFoundException e) {
				logger.error("Cannot debug direct print: " + e.getMessage());
			}
		}

		return output;
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
	 *	�p�����[�^�������Ԃ��܂��B
	 *
	 *		@return		�p�����[�^������
	 */
	protected String parameters() throws IOException {
		StringBuilder buf = new StringBuilder();
		String sep = "\n";

		if (printerName != null) {
			buf.append(sep + "printerName=" + printerName);
		}

		//�㉺���`�F�b�N�ƃf�t�H���g�̒ǉ�
		if (numberOfCopy != null) {
			int val = numberOfCopy.intValue();
			if (val <= 0) {
				val = 1;
			}
			if (val > 999) {
				val = 999;
			}
			buf.append(sep + "numberOfCopy=" + String.valueOf(val));
		} else {
			buf.append(sep + "numberOfCopy=1");
		}

		if (selectedTray != null) {
			buf.append(sep + "selectedTray=" + selectedTray);
		}

		if (jobName != null && jobName.length() > 0) {
			buf.append(sep + "jobName=" + jobName);
		} else {
			jobName = "JobName_Default";
			buf.append(sep + "jobName=" + jobName);
		}

		if (doFit != null) {
			buf.append(sep + "doFit=" + doFit);
		}
		//Batch/Direct�Ƃ��Ƀy�[�W�w��Ƃ���Bv5.0
		if (fromPage > 0) {
			buf.append(sep + "fromPage=" + (Integer.toString(fromPage)));
		}

		if ((toPage > 0) && (toPage >= fromPage)) {
			buf.append(sep + "toPage=" + (Integer.toString(toPage)));
		}

		return buf.toString();
	}

	/**
	 *	�o�b�t�@�t���b�V�����܂��B
	 */
	public void flush() throws IOException {
		pdfdoc.flush();
	}

	/**
	 *	PDF �f�[�^���o�b�t�@�ɏ����o���܂��B
	 *
	 *		@param		pdf			PDF �f�[�^
	 */
	public void write(int pdf) throws IOException {
		pdfdoc.write(pdf);
	}

	/**
	 *	PDF �f�[�^���w��ʒu����w��o�C�g�������o�b�t�@�ɏ����o���܂��B
	 *
	 *		@param		pdf			PDF �f�[�^
	 *		@param		offset		�J�n�ʒu
	 *		@param		length		�o�C�g��
	 */
	public void write(byte[] pdf, int offset, int length) throws IOException {
		pdfdoc.write(pdf, offset, length);
	}

	/**
	 *	PDF �f�[�^���o�b�t�@�ɏ����o���܂��B
	 *
	 *		@param		pdf			PDF �f�[�^
	 */
	public void write(byte[] pdf) throws IOException {
		pdfdoc.write(pdf);
	}

	/**
	 *	�������܂ꂽ�o�C�g����Ԃ��܂��B
	 *
	 *		@return		�������܂ꂽ�o�C�g��
	 */
	public int size() {
		return pdfdoc.size();
	}

	/**
	 *	�v�����^�����Z�b�g���܂��B
	 *
	 *		@param		value		�v�����^��
	 */
	public void setPrinterName(String value) {
		printerName = value;
	}

	/**
	 *	����������Z�b�g���܂��B
	 *
	 *		@param		value		�������
	 */
	public void setNumberOfCopy(int value) {
		if (value < 1) {
			logger.warn(PDFMessages.getMessage(1429, "(" + Integer.toString(value) + " -> 1)"));
			value = 1;
		}
		if (value > 999) {
			logger.warn(PDFMessages.getMessage(1429, "(" + Integer.toString(value) + " -> 999)"));
			value = 999;
		}
		numberOfCopy = new Integer(value);
	}

	/**
	 *	�o�̓g���C���Z�b�g���܂��B
	 *
	 *		@param		value		�o�̓g���C
	 */
	public void setSelectedTray(String value) {
		selectedTray = value;
	}

	/**
	 *	������ʎq���Z�b�g���܂��B
	 *
	 *		@param		value		������ʎq
	 */
	public void setJobName(String value) {
		if (value == null || value.length() == 0) {
			jobName = "JobName_Default";
		} else {
			jobName = value;
		}
	}

	/**
	 *	�y�[�W�T�C�Y�ɍ��킹�Ĉ���t���O���Z�b�g���܂��B
	 *
	 *		@param		value		������ʎq
	 *
	 *		@since		v5.0.0
	 */
	public void setDoFit(boolean value) {
		if (value) {
			doFit = "true";
		} else {
			doFit = "false";
		}
	}

	/**
	 *	�J�n�y�[�W�ԍ����Z�b�g���܂��B
	 *
	 *		@param		value		�J�n�y�[�W�ԍ�
	 */
	public void setFromPage(int value) {
		fromPage = value;
	}

	/**
	 *	�I���y�[�W�ԍ����Z�b�g���܂��B
	 *
	 *		@param		value		�I���y�[�W�ԍ�
	 */
	public void setToPage(int value) {
		toPage = value;
	}

	/**
	 *	���[�U�p�X���[�h(����)���Z�b�g���܂��B
	 *		@param		value		�����p�X���[�h
	 */
	public void setPassword(String value) {
		if (value == null) {

		} else {
			userPassword = value;
		}

	}

	/**
	*	���[�U�p�X���[�h(base64�G���R�[�h�ς�)���Z�b�g���܂��B
	*		@param		base64�G���R�[�h�ς݃p�X���[�h
	*/
	public void setPasswordWithEncoded(String value) {
		if (value == null || value.length() == 0) {
			return;

		}
		BASE64 base64 = new BASE64();
		byte[] dataToDecode = value.getBytes();
		try {
			byte[] decodedString = base64.decode(dataToDecode);
			userPassword = new String(decodedString, 0, decodedString.length);
		} catch (Exception e) {
			logger.error(PDFMessages.getMessage(1427, value + ": " + e.getMessage()));
		}
	}
}
