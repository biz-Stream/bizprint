/**
 *	@(#)PDFDirectEncryption.java  1.00 2006/01/12
 *
 *	(C) Copyright BrainSellers.com Corporation. 2006, 2006 - All Rights Reserved.
 */

package com.brainsellers.pdf.print;

import java.io.UnsupportedEncodingException;

/**
 *	印刷ファイル暗号化用パスワードを生成するためのクラスです。
 *
 *		@version	1.00 2006/01/12
 *		@author		Ogu
 */
public class PDFDirectEncryption extends Object {

	/**
	 *	バージョン
	 */
	public static final String VERSION = "1.00 2006/01/12";

	/**
	 *	バイト配列
	 */
	private static final byte[] BYTES = new byte[] { (byte) 0x0d, (byte) 0x0a };

	/**
	 *	印刷ファイル暗号化用パスワード
	 */
	private static String PASSWORD;

	/**
	 *	クラスロード時の処理
	 */
	static {
		try {
			PASSWORD = new String(BYTES, "8859_1");
		} catch (UnsupportedEncodingException e1) {
			PASSWORD = new String(BYTES);
		}
	}

	/**
	 *	印刷ファイル暗号化用パスワードを返します。
	 *
	 *		@return		印刷ファイル暗号化用パスワード
	 */
	public static String getPassword() {
		return PASSWORD;
	}

	/**
	 *	インスタンスをします。
	 */
	private PDFDirectEncryption() {
		super();
	}
}
