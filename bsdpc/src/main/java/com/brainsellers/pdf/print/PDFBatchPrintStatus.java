/**
 *	@(#)PDFBatchPrintStatus.java  1.01 2005/07/11
 *
 *	(C) Copyright BrainSellers.com Corporation. 2005, 2005 - All Rights Reserved.
 */

package com.brainsellers.pdf.print;

/**
 *	�o�b�`�E�^�C�v�̃_�C���N�g����̈���X�e�[�^�X�𐶐����邽�߂̃N���X�ł��B
 *
 *		@version	1.01 2005/07/11
 *		@author		Ogu
 */
public class PDFBatchPrintStatus extends Object {

	/**
	 *	�o�[�W����
	 */
	public static final String VERSION = "1.01 2005/07/11";

	/**
	 *	�ꎞ��~
	 */
	public static final int JOB_STATUS_PAUSED				= 0x00000001;

	/**
	 *	�G���[
	 */
	public static final int JOB_STATUS_ERROR				= 0x00000002;

	/**
	 *	�폜��
	 */
	public static final int JOB_STATUS_DELETING				= 0x00000004;

	/**
	 *	�X�v�[����
	 */
	public static final int JOB_STATUS_SPOOLING				= 0x00000008;

	/**
	 *	�����
	 */
	public static final int JOB_STATUS_PRINTING				= 0x00000010;

	/**
	 *	�I�t���C��
	 */
	public static final int JOB_STATUS_OFFLINE				= 0x00000020;

	/**
	 *	�p���؂�
	 */
	public static final int JOB_STATUS_PAPEROUT				= 0x00000040;

	/**
	 *	����
	 */
	public static final int JOB_STATUS_PRINTED				= 0x00000080;

	/**
	 *	�폜
	 */
	public static final int JOB_STATUS_DELETED				= 0x00000100;

	/**
	 *	�s���ȃh���C�o
	 */
	public static final int JOB_STATUS_BLOCKED_DEVQ			= 0x00000200;

	/**
	 *	�v�����^�s��
	 */
	public static final int JOB_STATUS_USER_INTERVENTION	= 0x00000400;

	/**
	 *	�ċN����
	 */
	public static final int JOB_STATUS_RESTART				= 0x00000800;

	/**
	 *	
	 */
	public static final int JOB_STATUS_COMPLETE				= 0x00001000;

	/**
	 *	����w�� ID
	 */
	protected String jobId;

	/**
	 *	������ʎq
	 */
	protected String jobName;

	/**
	 *	�v�����^�[��
	 */
	protected String printerName;

	/**
	 *	�^�C���X�^���v
	 */
	protected String dateTime;

	/**
	 *	�����ԃR�[�h
	 */
	protected String statusCode;

	/**
	 *	������
	 */
	protected String status;

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
	 *	�C���X�^���X�𐶐����A�w�肳�ꂽ����w�� ID �ŏ��������܂��B
	 *
	 *		@param		jobId		����w�� ID
	 */
	protected PDFBatchPrintStatus(String jobId) {
		super();

		this.jobId = jobId;

		jobName      = null;
		printerName  = null;
		dateTime     = null;
		statusCode   = null;
		status       = null;
		errorCode    = null;
		errorCause   = null;
		errorDetails = null;
		jobId        = null;
	}

	/**
	 *	����w�� ID ��Ԃ��܂��B
	 *
	 *		@return		����w�� ID
	 */
	public String getJobId() {
		return jobId;
	}

	/**
	 *	������ʎq���Z�b�g���܂��B
	 *
	 *		@param		value		������ʎq
	 */
	protected void setJobName(String value) {
		jobName = value;
	}

	/**
	 *	������ʎq��Ԃ��܂��B
	 *
	 *		@return		������ʎq
	 */
	public String getJobName() {
		return jobName;
	}

	/**
	 *	�v�����^�[�����Z�b�g���܂��B
	 *
	 *		@param		value		�v�����^�[��
	 */
	protected void setPrinterName(String value) {
		printerName = value;
	}

	/**
	 *	�v�����^�[����Ԃ��܂��B
	 *
	 *		@return		�v�����^�[��
	 */
	public String getPrinterName() {
		return printerName;
	}

	/**
	 *	�^�C���X�^���v���Z�b�g���܂��B
	 *
	 *		@param		value		�^�C���X�^���v
	 */
	protected void setDateTime(String value) {
		dateTime = value;
	}

	/**
	 *	�^�C���X�^���v��Ԃ��܂��B
	 *
	 *		@return		�^�C���X�^���v
	 */
	public String getDateTime() {
		return dateTime;
	}

	/**
	 *	�����ԃR�[�h���Z�b�g���܂��B
	 *
	 *		@param		value		�����ԃR�[�h
	 */
	protected void setStatusCode(String value) {
		statusCode = value;
	}

	/**
	 *	�����ԃR�[�h��Ԃ��܂��B
	 *
	 *		@return		�����ԃR�[�h
	 */
	public String getStatusCode() {
		return statusCode;
	}

	/**
	 *	�����Ԃ��Z�b�g���܂��B
	 *
	 *		@param		value		������
	 */
	protected void setStatus(String value) {
		status = value;
	}

	/**
	 *	�����Ԃ�Ԃ��܂��B
	 *
	 *		@return		������
	 */
	public String getStatus() {
		return status;
	}

	/**
	 *	�G���[�R�[�h���Z�b�g���܂��B
	 *
	 *		@param		value		�G���[�R�[�h
	 */
	protected void setErrorCode(String value) {
		errorCode = value;
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
	 *	�G���[�̌������Z�b�g���܂��B
	 *
	 *		@param		value		�G���[�̌���
	 */
	protected void setErrorCause(String value) {
		errorCause = value;
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
	 *	�G���[�̓��e���Z�b�g���܂��B
	 *
	 *		@param		value		�G���[�̓��e
	 */
	protected void setErrorDetails(String value) {
		errorDetails = value;
	}

	/**
	 *	�G���[�̓��e��Ԃ��܂��B
	 *
	 *		@return		�G���[�̓��e
	 */
	public String getErrorDetails() {
		return errorDetails;
	}
}
