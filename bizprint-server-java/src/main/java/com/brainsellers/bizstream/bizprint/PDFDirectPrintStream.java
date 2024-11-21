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
import java.io.OutputStream;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.logging.Logger;

import javax.servlet.http.HttpServletResponse;

/**
 *  ダイレクト印刷のストリームを生成するためのクラスです。
 */
public class PDFDirectPrintStream extends PDFCommonPrintStream {
    private static final Logger LOGGER = Logger.getLogger(PDFDirectPrintStream.class.getName());

    /**
     *  HTTP 応答オブジェクト
     */
    protected HttpServletResponse response;

    /**
     *  印刷応答 URL
     */
    protected String responseUrl;

    /**
     *  ファイル保存
     */
    protected String saveFileName;

    /**
     *  ブラウザ target 名
     */
    protected String target;

    /**
     *  印刷ダイアログ表示フラグ
     */
    protected Boolean printDialog;

    /**
     *  ダウンロードファイル名の一意化フラグ
     */
    protected Boolean sppnameUnified = true;


    /**
     *  インスタンスを生成し、指定された HTTP 応答オブジェクトで初期化します。
     *
     *      @param      response    HTTP 応答オブジェクト
     */
    public PDFDirectPrintStream(HttpServletResponse response) {
        super();


        this.response = response;

        responseUrl  = null;
        saveFileName = null;
        target       = null;
        printDialog  = null;
    }

    /**
     *  バッファをクローズします。
     */
    public void close() throws IOException {
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
        byte[] sendspp = null;
        try {
            sendspp = cesp.createCompressedSpp(tmp.toString(), jobName, pdfdoc);
        } catch (Exception e) {
        }
        if (sendspp == null) {
            LOGGER.severe("send spp file is NULL.");
            throw new IOException("send spp file is NULL.");
        }

        response.setContentLength(sendspp.length);
        response.setContentType("application/x-spp");

        //実行ごとにファイル名を変える
        if(sppnameUnified){
            //現在時刻からファイル名作成
            Date nowdate = new Date();
            int rand = (int) (Math.random() * 1000);

            String fname_now = new SimpleDateFormat("yyyyMMdd_HHmmss").format(nowdate) + "_" + String.format("%04d", rand) + ".spp";

            //attachmentだとブラウザによってはダウンロード後に手動ファイル保存になるためinlineにする
            response.addHeader("Content-Disposition", "inline; filename=" + fname_now );
        }

        OutputStream output = getOutput(response.getOutputStream());

        output.write(sendspp);
        output.close();
    }

    /**
     *  印刷応答 URL をセットします。
     *
     *      @param      value       印刷応答 URL
     */
    public void setResponseUrl(String value) {
        responseUrl = value;
    }

    /**
     *  ファイル保存をセットします。
     *
     *      @param      value       ファイル保存
     */
    public void setSaveFileName(String value) {
        saveFileName = value;
    }

    /**
     *  ブラウザ target 名をセットします。
     *
     *      @param      value       ブラウザ target 名
     */
    public void setTarget(String value) {
        target = value;
    }

    /**
     *  印刷ダイアログ表示フラグをセットします。<BR>
     *  <BR>
     *  このメソッドでtrue(印刷ダイアログを表示する)を指定した場合、以下のメソッドでの指定は無視されます。<BR>
     *  <UL>
     *  <LI>{@link PDFCommonPrintStream#setNumberOfCopy PDFCommonPrintStream#setNumberOfCopy(印刷部数)}
     *  <LI>{@link PDFCommonPrintStream#setFromPage PDFCommonPrintStream#setFromPage(開始ページ番号)}
     *  <LI>{@link PDFCommonPrintStream#setToPage PDFCommonPrintStream#setToPage(終了ページ番号)}
     *  <LI>{@link PDFCommonPrintStream#setDoFit PDFCommonPrintStream#setDoFit(ページサイズに合わせて印刷フラグ)}
     *  </UL>
     *
     *      @param      value       印刷ダイアログ表示フラグ
     */
    public void setPrintDialog(boolean value) {
        printDialog = Boolean.valueOf(value);
    }

    /**
     *  SPPファイル名を一意化するかを指定します(デフォルト: true)
     *      @param      value       true: 一意化する, false: 一意化しない
     */
    public void setSppNameUnified(Boolean value) {
        sppnameUnified = value;
    }
}
