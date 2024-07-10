/**
 *	@(#)PDFUrlTools.java  1.00 2005/06/22
 *
 *	(C) Copyright BrainSellers.com Corporation. 2005, 2005 - All Rights Reserved.
 */

package com.brainsellers.pdf.print;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.UnsupportedEncodingException;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.brainsellers.pdf.PDFRuntimeException;
import com.brainsellers.pdf.internal.PDFMessages;

/**
 *	URL �ϊ��p�̃c�[���E�N���X�ł��B
 *
 *		@version	1.00 2005/06/22
 *		@author		Ogu
 */
public abstract class PDFUrlTools extends Object {
	static final Logger logger = LoggerFactory.getLogger(PDFUrlTools.class);

	/**
	 *	�o�[�W����
	 */
	public static final String VERSION = "1.00 2005/06/22";

	/**
	 *	�ϊ��e�[�u��
	 */
	protected static boolean[] UNCHANGES = new boolean[] {
		false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,		//
		false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,		//
		false, false, false, false, false, false, false, false, false, false, true,  false, false, true , true , false,		//  !"#$%&'()*+,-./
		true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  false, false, false, false, false, false,		// 0123456789:;<=>?
		true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true, 		// @ABCDEFGHIJKLMNO
		true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  false, false, false, false, true,		// PQRSTUVWXYZ[\]^_
		true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true, 		// @abcdefghijklmno
		true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  false, false, false, false, false,		// pqrstuvwxyz{|}~
	};

	/**
	 *	�C���X�^���X�𐶐����܂��B
	 */
	protected PDFUrlTools() {
		super();
	}

	/**
	 *	������� URL ������ɕϊ����܂��B
	 *
	 *		@param		text		������
	 *
	 *		@return		URL ������
	 */
	public static String encode(String text) {
		try {
			StringBuilder buf = new StringBuilder(text.length());
			boolean changed = false;
			ByteArrayOutputStream bbb = new ByteArrayOutputStream(10);
			OutputStreamWriter writer = new OutputStreamWriter(bbb, "UTF-8");

			for (int i = 0; i < text.length(); i++) {
				int ci = (int) text.charAt(i);

				if ((ci <= 0x7f) && UNCHANGES[ci & 0x7f]) {
					buf.append((char) ci);
				}
				else {
					if (ci == ' ') {
						buf.append('+');
						changed = true;
					}
					else {
						bbb.reset();
						writer = new OutputStreamWriter(bbb, "UTF-8");

						writer.write(ci);

						if ((ci >= 0xd800) && (ci <= 0xdbff)) {
							if ((i + 1) < text.length()) {
								ci = (int) text.charAt(i + 1);

								if ((ci >= 0xdc00) && (ci <= 0xdfff)) {
									writer.write(ci);
									i++;
								}
							}
						}

						writer.flush();

						byte[] ba = bbb.toByteArray();

						for (int j = 0; j < ba.length; j++) {
							buf.append('%');

							char ch1 = Character.forDigit((ba[j] >> 4) & 0x0f, 16);
							char ch2 = Character.forDigit( ba[j]       & 0x0f, 16);

							if (Character.isLetter(ch1)) {
								ch1 &= 0xdf;
							}

							if (Character.isLetter(ch2)) {
								ch2 &= 0xdf;
							}

							buf.append(ch1);
							buf.append(ch2);
						}

						changed = true;
					}
				}
			}

			return (changed ? buf.toString() : text);
		}
		catch (IOException e) {
			logger.error(PDFMessages.getMessage(1421));
			PDFRuntimeException.throwException(1421, PDFMessages.getMessage(1421), e);
		}

		return text;
	}

	/**
	 *	URL ������𕶎���ɕϊ����܂��B
	 *
	 *		@param		text		URL ������
	 *
	 *		@return		������
	 */
	public static String decode(String text) {
		StringBuilder buf = new StringBuilder();
		boolean changed = false;
		int length = text.length();
		

		for (int i = 0; i < length; i++) {
			char ch = text.charAt(i);

			switch (ch) {
				case '%': {
					try {
						byte[] bytes = new byte[(length - i) / 3];
						int p = 0;

						while (((i + 2) < length) && (ch == '%')) {
							bytes[p++] = (byte) Integer.parseInt(text.substring(i + 1, i + 3), 16);
							i += 3;

							if (i < length) {
								ch = text.charAt(i);
							}
						}

						if ((i < length) && (ch == '%')) {
							logger.error(PDFMessages.getMessage(1422));
							PDFRuntimeException.throwException(1422, PDFMessages.getMessage(1422), null);
						}

						buf.append(new String(bytes, 0, p, "UTF-8"));
					}
					catch (NumberFormatException e) {
						logger.error(PDFMessages.getMessage(1422));
						PDFRuntimeException.throwException(1422, PDFMessages.getMessage(1422), e);
					}
					catch (UnsupportedEncodingException e) {
						logger.error(PDFMessages.getMessage(1422));
						PDFRuntimeException.throwException(1422, PDFMessages.getMessage(1422), e);
					}

					changed = true;
					--i;
					break;
				}

				case '+': {
					buf.append(' ');
					changed = true;
					break;
				}

				default: {
					buf.append(ch);
					break;
				}
			}
		}

		return (changed ? buf.toString() : text);
	}
}
