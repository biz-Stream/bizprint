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
 *	�I�����C���E�^�C�v�̃_�C���N�g����̃X�g���[���𐶐����邽�߂̃N���X�ł��B
 *
 *		@version	1.07 2017/03/08
 *		@author		Ogu
 */
public class PDFDirectPrintStream extends PDFCommonPrintStream implements PDFEncryptOutputStream {

	/**
	 *	�o�[�W����
	 */
	public static final String VERSION = "1.07 2017/03/08";

	/**
	 *	HTTP �����I�u�W�F�N�g
	 */
	protected HttpServletResponse response;

	/**
	 *	������� URL
	 */
	protected String responseUrl;

	/**
	 *	�I������
	 *	@deprecated v5.0���p�~����܂����B
	 */
	protected String disposal;

	/**
	 *	�t�@�C���ۑ�
	 */
	protected String saveFileName;

	/**
	 *	����֎~�t���O
	 *	@deprecated v5.0���p�~����܂����B
	 */
	protected Boolean printDeny;

	/**
	 *	�u���E�U target ��
	 */
	protected String target;

	/**
	 *	����ʒu�����t���O
	 *	@deprecated v5.0���p�~����܂����B
	 */
	protected Boolean printAdjust;

	/**
	 *	����_�C�A���O�\���t���O
	 */
	protected Boolean printDialog;

	/**
	 *	�Í����t���O
	 *	@deprecated v5.0���p�~����܂����B
	 */
	protected Boolean encryption;

	/**
	 *	�_�E�����[�h�t�@�C�����̈�Ӊ��t���O
	 */
	protected Boolean sppnameUnified = true;


	/**
	 *	�C���X�^���X�𐶐����A�w�肳�ꂽ HTTP �����I�u�W�F�N�g�ŏ��������܂��B
	 *
	 *		@param		response	HTTP �����I�u�W�F�N�g
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
	 *	�o�b�t�@���N���[�Y���܂��B
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

		//���s���ƂɃt�@�C������ς���
		if(sppnameUnified){
			//���ݎ�������t�@�C�����쐬
			Date nowdate = new Date();
			int rand = (int) (Math.random() * 1000);

			String fname_now = new SimpleDateFormat("yyyyMMdd_HHmmss").format(nowdate) + "_" + String.format("%04d", rand) + ".spp";

			//attachment���ƃu���E�U�ɂ���Ă̓_�E�����[�h��Ɏ蓮�t�@�C���ۑ��ɂȂ邽��inline�ɂ���
			response.addHeader("Content-Disposition", "inline; filename=" + fname_now );
		}

		OutputStream output = getOutput(response.getOutputStream());

		output.write(sendspp);
		output.close();
	}

	/**
	 *	������� URL ���Z�b�g���܂��B
	 *
	 *		@param		value		������� URL
	 */
	public void setResponseUrl(String value) {
		responseUrl = value;
	}

	/**
	 *	�I�������� TERMINATE ���[�h�ɃZ�b�g���܂��B
	 *	@deprecated v5.0���p�~����܂����B
	 */
	public void setTerminate() {

	}

	/**
	 *	�I�������� REGIDENT ���[�h�ɃZ�b�g���܂��B
	 *	@deprecated v5.0���p�~����܂����B
	 */
	public void setRegident() {

	}

	/**
	 *	�t�@�C���ۑ����Z�b�g���܂��B
	 *
	 *		@param		value		�t�@�C���ۑ�
	 */
	public void setSaveFileName(String value) {
		saveFileName = value;
	}

	/**
	 *	����֎~�t���O���Z�b�g���܂��B
	 *
	 *		@param		value		����֎~�t���O
	 *		@deprecated v5.0���p�~����܂����B
	 */
	public void setPrintDeny(boolean value) {

	}

	/**
	 *	�u���E�U target �����Z�b�g���܂��B
	 *
	 *		@param		value		�u���E�U target ��
	 */
	public void setTarget(String value) {
		target = value;
	}

	/**
	 *	����ʒu�����t���O���Z�b�g���܂��B
	 *
	 *		@param		value		����ʒu�����t���O
	 *		@deprecated v5.0���p�~����܂����B
	 */
	public void setPrintAdjust(boolean value) {

	}

	/**
	 *	����_�C�A���O�\���t���O���Z�b�g���܂��B<BR>
	 *	<BR>
	 *	���̃��\�b�h��true(����_�C�A���O��\������)���w�肵���ꍇ�A�ȉ��̃��\�b�h�ł̎w��͖�������܂��B<BR>
	 *	<UL>
	 *	<LI>{@link PDFCommonPrintStream#setNumberOfCopy PDFCommonPrintStream#setNumberOfCopy(�������)}
	 *	<LI>{@link PDFCommonPrintStream#setFromPage PDFCommonPrintStream#setFromPage(�J�n�y�[�W�ԍ�)}
	 *	<LI>{@link PDFCommonPrintStream#setToPage PDFCommonPrintStream#setToPage(�I���y�[�W�ԍ�)}
	 *	<LI>{@link PDFCommonPrintStream#setDoFit PDFCommonPrintStream#setDoFit(�y�[�W�T�C�Y�ɍ��킹�Ĉ���t���O)}
	 *	</UL>
	 *
	 *		@param		value		����_�C�A���O�\���t���O
	 */
	public void setPrintDialog(boolean value) {
		printDialog = new Boolean(value);
	}

	/**
	 *	�Í�������悤�ɃZ�b�g���܂��B
	 *		@deprecated v5.0���p�~����܂����B
	 */
	public void encryptPrintFile() {

	}

	/**
	 *	�Í������邩�ǂ�����Ԃ��܂��B
	 *
	 *		@return		true �̏ꍇ�A�Í�������
	 *		@deprecated v5.0���p�~����܂����B
	 */
	public boolean isEncrypt() {
			return false;
	}

	/**
	 *	SPP�t�@�C��������Ӊ����邩���w�肵�܂�(�f�t�H���g: true)
	 *		@param		value		true: ��Ӊ�����, false: ��Ӊ����Ȃ�
	 */
    public void setSppNameUnified(Boolean value) {
    	sppnameUnified = value;
    }
}
