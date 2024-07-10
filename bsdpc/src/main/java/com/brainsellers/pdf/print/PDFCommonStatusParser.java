/**
 *	@(#)PDFCommonStatusParser.java  1.01 2022/08/31
 *
 *	(C) Copyright BrainSellers.com Corporation. 2005, 2022 - All Rights Reserved.
 */

package com.brainsellers.pdf.print;

import java.io.InputStream;
import java.util.ArrayList;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.w3c.dom.Document;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;

import com.brainsellers.pdf.PDFRuntimeException;
import com.brainsellers.pdf.internal.PDFMessages;
import com.brainsellers.pdf.internal.PDFProperties;
import com.brainsellers.util.XMLVendorChecker;

/**
 *	��ʓI�ȃo�b�`����̈���X�e�[�^�X���p�[�X���邽�߂̒��ۃN���X�ł��B
 *
 *		@version	1.01 2022/08/31
 *		@author		Ogu
 */
public abstract class PDFCommonStatusParser extends Object {
	static final Logger logger = LoggerFactory.getLogger(PDFCommonStatusParser.class);

	/**
	 *	�o�[�W����
	 */
	public static final String VERSION = "1.01 2022/08/31";

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
	 *	����X�e�[�^�X
	 */
	protected ArrayList printStatus;


	/**
	 *	����X�e�[�^�X�̃p�[�T�[��Ԃ��܂��B
	 *
	 *		@return		����X�e�[�^�X�̃p�[�T�[
	 */
	public static PDFCommonStatusParser getInstance() {
		String name = PDFProperties.getProperty("print.status.parser");

		if (name == null) {
			switch (XMLVendorChecker.getVendor()) {
				case XMLVendorChecker.VENDOR_XERCES2:	name = "xerces2";	break;
				case XMLVendorChecker.VENDOR_JAXP:		name = "jaxp";		break;
				default:								name = "jaxp";		break;
			}
		}

		if (name != null) {
			if (name.equalsIgnoreCase("xerces2")) {
				logger.info("Print status parser: xerces2");

				return new PDFXerces2StatusParser();
			}
			else {
				logger.info("Print status parser: jaxp");

				return new PDFJaxpStatusParser();
			}
		}

		return new PDFJaxpStatusParser();
	}

	/**
	 *	�C���X�^���X�𐶐����܂��B
	 */
	public PDFCommonStatusParser() {
		super();

		result       = null;
		errorCode    = null;
		errorCause   = null;
		errorDetails = null;

		printStatus = new ArrayList();
	}

	/**
	 *	����X�e�[�^�X���p�[�X���܂��B
	 *
	 *		@param		input		���̓X�g���[��
	 */
	public void parse(InputStream input) {
		try {
			DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
			DocumentBuilder builder = factory.newDocumentBuilder();
			Document document = builder.parse(input);

			NodeList nodes = document.getChildNodes();

			for (int i = 0; i < nodes.getLength(); i++) {
				Node node = nodes.item(i);

				if (node.getNodeName().equalsIgnoreCase("Response")) {
					parseResponse(node);

					break;
				}
			}

		}
		catch (Exception e) {
			logger.error(PDFMessages.getMessage(1411));
			PDFRuntimeException.throwException(1411, PDFMessages.getMessage(1411), e);
		}
	}

	/**
	 *	���X�|���X���p�[�X���܂��B
	 *
	 *		@param		response	���X�|���X
	 */
	protected abstract void parseResponse(Node response);

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
	 *	����X�e�[�^�X��Ԃ��܂��B
	 *
	 *		@return		����X�e�[�^�X
	 */
	public ArrayList getPrintStatus() {
		return printStatus;
	}
}
