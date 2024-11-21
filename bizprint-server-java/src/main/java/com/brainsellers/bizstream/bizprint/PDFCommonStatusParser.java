/*
 * Copyright 2024 BrainSellers.com Corporation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package com.brainsellers.bizstream.bizprint;

import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.logging.Logger;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Document;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

/**
 * バッチ印刷の印刷ステータスをパースするための抽象クラスです。
 */
public abstract class PDFCommonStatusParser extends Object {
    private static final Logger LOGGER = Logger.getLogger(PDFCommonStatusParser.class.getName());

    /**
     *  結果
     */
    protected String result;

    /**
     *  エラーコード
     */
    protected String errorCode;

    /**
     *  エラーの原因
     */
    protected String errorCause;

    /**
     *  エラーの内容
     */
    protected String errorDetails;

    /**
     *  印刷ステータス
     */
    protected ArrayList printStatus;


    /**
     *  印刷ステータスのパーサーを返します。
     *
     *      @return     印刷ステータスのパーサー
     */
    public static PDFCommonStatusParser getInstance() {
        return new PDFJaxpStatusParser();
    }

    /**
     *  インスタンスを生成します。
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
     *  印刷ステータスをパースします。
     *
     *      @param      input       入力ストリーム
     * @throws ParserConfigurationException
     * @throws IOException
     * @throws SAXException
     */
    public void parse(InputStream input) throws ParserConfigurationException, SAXException, IOException {
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

    /**
     *  レスポンスをパースします。
     *
     *      @param      response    レスポンス
     */
    protected abstract void parseResponse(Node response);

    /**
     *  結果を返します。
     *
     *      @return     結果
     */
    public String getResult() {
        return result;
    }

    /**
     *  エラーコードを返します。
     *
     *      @return     エラーコード
     */
    public String getErrorCode() {
        return errorCode;
    }

    /**
     *  エラーの原因を返します。
     *
     *      @return     エラーの原因
     */
    public String getErrorCause() {
        return errorCause;
    }

    /**
     *  エラーの内容を返します。
     *
     *      @return     エラーの内容
     */
    public String getErrorDetails() {
        return errorDetails;
    }

    /**
     *  印刷ステータスを返します。
     *
     *      @return     印刷ステータス
     */
    public ArrayList getPrintStatus() {
        return printStatus;
    }
}
