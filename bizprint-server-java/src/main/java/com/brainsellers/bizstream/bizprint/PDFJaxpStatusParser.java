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
import java.util.logging.Logger;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Attr;
import org.w3c.dom.Document;
import org.w3c.dom.NamedNodeMap;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.w3c.dom.Text;
import org.xml.sax.SAXException;

/**
 *  バッチ印刷の印刷ステータスをパースするための JAXP 用クラスです。
 */
public class PDFJaxpStatusParser extends PDFCommonStatusParser {
    private static final Logger LOGGER = Logger.getLogger(PDFJaxpStatusParser.class.getName());

    /**
     *  インスタンスを生成します。
     */
    protected PDFJaxpStatusParser() {
        super();
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
    protected void parseResponse(Node response) {
        NodeList nodes = response.getChildNodes();

        for (int i = 0; i < nodes.getLength(); i++) {
            Node node = nodes.item(i);
            String name = node.getNodeName();

            if (name.equalsIgnoreCase("Result")) {
                result = getText(node);
            }
            else if (name.equalsIgnoreCase("ErrorCode")) {
                errorCode = getText(node);
            }
            else if (name.equalsIgnoreCase("ErrorCause")) {
                errorCause = getText(node);
            }
            else if (name.equalsIgnoreCase("ErrorDetails")) {
                errorDetails = getText(node);
            }
            else if (name.equalsIgnoreCase("PrintStatus")) {
                parsePrintStatus(node);
            }
        }
    }

    /**
     *  印刷ステータスをパースします。
     *
     *      @param      print       印刷ステータス
     */
    protected void parsePrintStatus(Node print) {
        NamedNodeMap attributes = print.getAttributes();
        Attr jobId = (Attr) attributes.getNamedItem("JobId");

        if (jobId != null) {
            PDFBatchPrintStatus status = new PDFBatchPrintStatus(jobId.getValue());
            printStatus.add(status);

            NodeList nodes = print.getChildNodes();

            for (int i = 0; i < nodes.getLength(); i++) {
                Node node = nodes.item(i);
                String name = node.getNodeName();

                if (name.equalsIgnoreCase("JobName")) {
                    status.setJobName(getText(node));
                }
                else if (name.equalsIgnoreCase("PrinterName")) {
                    status.setPrinterName(getText(node));
                }
                else if (name.equalsIgnoreCase("DateTime")) {
                    status.setDateTime(getText(node));
                }
                else if (name.equalsIgnoreCase("StatusCode")) {
                    status.setStatusCode(getText(node));
                }
                else if (name.equalsIgnoreCase("Status")) {
                    status.setStatus(getText(node));
                }
                else if (name.equalsIgnoreCase("ErrorCode")) {
                    status.setErrorCode(getText(node));
                }
                else if (name.equalsIgnoreCase("ErrorCause")) {
                    status.setErrorCause(getText(node));
                }
                else if (name.equalsIgnoreCase("ErrorDetails")) {
                    status.setErrorDetails(getText(node));
                }
            }
        }
    }

    /**
     *  文字列を返します。
     *
     *      @param      node        ノード
     *
     *      @return     文字列
     */
    protected String getText(Node node) {
        NodeList nodes = node.getChildNodes();

        for (int i = 0; i < nodes.getLength(); i++) {
            Node text = nodes.item(i);

            if (text.getNodeType() == Node.TEXT_NODE) {
                return ((Text) text).getData();
            }
        }

        return null;
    }
}
