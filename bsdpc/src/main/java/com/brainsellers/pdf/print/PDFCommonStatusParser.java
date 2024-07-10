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
 *	一般的なバッチ印刷の印刷ステータスをパースするための抽象クラスです。
 *
 *		@version	1.01 2022/08/31
 *		@author		Ogu
 */
public abstract class PDFCommonStatusParser extends Object {
	static final Logger logger = LoggerFactory.getLogger(PDFCommonStatusParser.class);

	/**
	 *	バージョン
	 */
	public static final String VERSION = "1.01 2022/08/31";

	/**
	 *	結果
	 */
	protected String result;

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
	 *	印刷ステータス
	 */
	protected ArrayList printStatus;


	/**
	 *	印刷ステータスのパーサーを返します。
	 *
	 *		@return		印刷ステータスのパーサー
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
	 *	インスタンスを生成します。
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
	 *	印刷ステータスをパースします。
	 *
	 *		@param		input		入力ストリーム
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
	 *	レスポンスをパースします。
	 *
	 *		@param		response	レスポンス
	 */
	protected abstract void parseResponse(Node response);

	/**
	 *	結果を返します。
	 *
	 *		@return		結果
	 */
	public String getResult() {
		return result;
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
	 *	エラーの原因を返します。
	 *
	 *		@return		エラーの原因
	 */
	public String getErrorCause() {
		return errorCause;
	}

	/**
	 *	エラーの内容を返します。
	 *
	 *		@return		エラーの内容
	 */
	public String getErrorDetails() {
		return errorDetails;
	}

	/**
	 *	印刷ステータスを返します。
	 *
	 *		@return		印刷ステータス
	 */
	public ArrayList getPrintStatus() {
		return printStatus;
	}
}
