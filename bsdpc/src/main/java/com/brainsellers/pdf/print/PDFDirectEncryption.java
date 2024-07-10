/**
 *	@(#)PDFDirectEncryption.java  1.00 2006/01/12
 *
 *	(C) Copyright BrainSellers.com Corporation. 2006, 2006 - All Rights Reserved.
 */

package com.brainsellers.pdf.print;

import java.io.UnsupportedEncodingException;

/**
 *	����t�@�C���Í����p�p�X���[�h�𐶐����邽�߂̃N���X�ł��B
 *
 *		@version	1.00 2006/01/12
 *		@author		Ogu
 */
public class PDFDirectEncryption extends Object {

	/**
	 *	�o�[�W����
	 */
	public static final String VERSION = "1.00 2006/01/12";

	/**
	 *	�o�C�g�z��
	 */
	private static final byte[] BYTES = new byte[] { (byte) 0x0d, (byte) 0x0a };

	/**
	 *	����t�@�C���Í����p�p�X���[�h
	 */
	private static String PASSWORD;

	/**
	 *	�N���X���[�h���̏���
	 */
	static {
		try {
			PASSWORD = new String(BYTES, "8859_1");
		} catch (UnsupportedEncodingException e1) {
			PASSWORD = new String(BYTES);
		}
	}

	/**
	 *	����t�@�C���Í����p�p�X���[�h��Ԃ��܂��B
	 *
	 *		@return		����t�@�C���Í����p�p�X���[�h
	 */
	public static String getPassword() {
		return PASSWORD;
	}

	/**
	 *	�C���X�^���X�����܂��B
	 */
	private PDFDirectEncryption() {
		super();
	}
}
