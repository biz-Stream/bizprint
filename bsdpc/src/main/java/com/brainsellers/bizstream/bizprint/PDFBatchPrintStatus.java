/**
 *	@(#)PDFBatchPrintStatus.java  1.01 2005/07/11
 *
 *	(C) Copyright BrainSellers.com Corporation. 2005, 2005 - All Rights Reserved.
 */

package com.brainsellers.bizstream.bizprint;

/**
 *	バッチ・タイプのダイレクト印刷の印刷ステータスを生成するためのクラスです。
 *
 *		@version	1.01 2005/07/11
 *		@author		Ogu
 */
public class PDFBatchPrintStatus {

	/**
	 *	一時停止
	 */
	public static final int JOB_STATUS_PAUSED				= 0x00000001;

	/**
	 *	エラー
	 */
	public static final int JOB_STATUS_ERROR				= 0x00000002;

	/**
	 *	削除中
	 */
	public static final int JOB_STATUS_DELETING				= 0x00000004;

	/**
	 *	スプール中
	 */
	public static final int JOB_STATUS_SPOOLING				= 0x00000008;

	/**
	 *	印刷中
	 */
	public static final int JOB_STATUS_PRINTING				= 0x00000010;

	/**
	 *	オフライン
	 */
	public static final int JOB_STATUS_OFFLINE				= 0x00000020;

	/**
	 *	用紙切れ
	 */
	public static final int JOB_STATUS_PAPEROUT				= 0x00000040;

	/**
	 *	完了
	 */
	public static final int JOB_STATUS_PRINTED				= 0x00000080;

	/**
	 *	削除
	 */
	public static final int JOB_STATUS_DELETED				= 0x00000100;

	/**
	 *	不正なドライバ
	 */
	public static final int JOB_STATUS_BLOCKED_DEVQ			= 0x00000200;

	/**
	 *	プリンタ不良
	 */
	public static final int JOB_STATUS_USER_INTERVENTION	= 0x00000400;

	/**
	 *	再起動中
	 */
	public static final int JOB_STATUS_RESTART				= 0x00000800;

	/**
	 *	
	 */
	public static final int JOB_STATUS_COMPLETE				= 0x00001000;

	/**
	 *	印刷指示 ID
	 */
	protected String jobId;

	/**
	 *	印刷識別子
	 */
	protected String jobName;

	/**
	 *	プリンター名
	 */
	protected String printerName;

	/**
	 *	タイムスタンプ
	 */
	protected String dateTime;

	/**
	 *	印刷状態コード
	 */
	protected String statusCode;

	/**
	 *	印刷状態
	 */
	protected String status;

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
	 *	インスタンスを生成し、指定された印刷指示 ID で初期化します。
	 *
	 *		@param		jobId		印刷指示 ID
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
	 *	印刷指示 ID を返します。
	 *
	 *		@return		印刷指示 ID
	 */
	public String getJobId() {
		return jobId;
	}

	/**
	 *	印刷識別子をセットします。
	 *
	 *		@param		value		印刷識別子
	 */
	protected void setJobName(String value) {
		jobName = value;
	}

	/**
	 *	印刷識別子を返します。
	 *
	 *		@return		印刷識別子
	 */
	public String getJobName() {
		return jobName;
	}

	/**
	 *	プリンター名をセットします。
	 *
	 *		@param		value		プリンター名
	 */
	protected void setPrinterName(String value) {
		printerName = value;
	}

	/**
	 *	プリンター名を返します。
	 *
	 *		@return		プリンター名
	 */
	public String getPrinterName() {
		return printerName;
	}

	/**
	 *	タイムスタンプをセットします。
	 *
	 *		@param		value		タイムスタンプ
	 */
	protected void setDateTime(String value) {
		dateTime = value;
	}

	/**
	 *	タイムスタンプを返します。
	 *
	 *		@return		タイムスタンプ
	 */
	public String getDateTime() {
		return dateTime;
	}

	/**
	 *	印刷状態コードをセットします。
	 *
	 *		@param		value		印刷状態コード
	 */
	protected void setStatusCode(String value) {
		statusCode = value;
	}

	/**
	 *	印刷状態コードを返します。
	 *
	 *		@return		印刷状態コード
	 */
	public String getStatusCode() {
		return statusCode;
	}

	/**
	 *	印刷状態をセットします。
	 *
	 *		@param		value		印刷状態
	 */
	protected void setStatus(String value) {
		status = value;
	}

	/**
	 *	印刷状態を返します。
	 *
	 *		@return		印刷状態
	 */
	public String getStatus() {
		return status;
	}

	/**
	 *	エラーコードをセットします。
	 *
	 *		@param		value		エラーコード
	 */
	protected void setErrorCode(String value) {
		errorCode = value;
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
	 *	エラーの原因をセットします。
	 *
	 *		@param		value		エラーの原因
	 */
	protected void setErrorCause(String value) {
		errorCause = value;
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
	 *	エラーの内容をセットします。
	 *
	 *		@param		value		エラーの内容
	 */
	protected void setErrorDetails(String value) {
		errorDetails = value;
	}

	/**
	 *	エラーの内容を返します。
	 *
	 *		@return		エラーの内容
	 */
	public String getErrorDetails() {
		return errorDetails;
	}
}
