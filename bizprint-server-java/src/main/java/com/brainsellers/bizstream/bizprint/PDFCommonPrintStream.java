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

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.util.Base64;
import java.util.logging.Logger;

/**
 * ダイレクト印刷のストリームを生成するための抽象クラスです。
 */
public abstract class PDFCommonPrintStream extends OutputStream  {
    private static final Logger LOGGER = Logger.getLogger(PDFCommonPrintStream.class.getName());

    /**
     *  上段トレイ
     */
    public static final String TRAY_UPPER   = "UPPER";

    /**
     *  中段トレイ
     */
    public static final String TRAY_MIDDLE  = "MIDDLE";

    /**
     *  下段トレイ
     */
    public static final String TRAY_LOWER   = "LOWER";

    /**
     *  手差しトレイ
     */
    public static final String TRAY_MANUAL  = "MANUAL";

    /**
     *  自動選択
     */
    public static final String TRAY_AUTO    = "AUTO";

    /**
     *  デバッグ・フラグ
     */
    protected static boolean DEBUG_FLAG     = false;

    /**
     *  デバッグ保存先
     */
    protected static String DEBUG_SAVE      = null;

    /**
     *  デバッグ最終時刻
     */
    protected static long DEBUG_LAST        = 0;

    /**
     *  PDF バッファ
     */
    protected ByteArrayOutputStream pdfdoc;

    /**
     *  プリンタ名
     */
    protected String printerName;

    /**
     *  印刷部数
     */
    protected Integer numberOfCopy;

    /**
     *  出力トレイ
     */
    protected String selectedTray;

    /**
     *  印刷識別子
     */
    protected String jobName;

    /**
     *  ページサイズに合わせて印刷
     */
    protected String doFit;
    /**
     *  開始ページ番号
     */
    protected int fromPage;

    /**
     *  終了ページ番号
     */
    protected int toPage;

    /**
     * ユーザ指定パスワード文字列
     */
    protected String userPassword = "";

    /**
     *  インスタンスを生成し、指定された HTTP 応答オブジェクトで初期化します。
     *
     *      HTTP 応答オブジェクト
     */
    public PDFCommonPrintStream() {
        super();

        pdfdoc = new ByteArrayOutputStream();

        printerName  = null;
        numberOfCopy = null;
        selectedTray = null;
        jobName      = null;
        doFit        = "false";
    }

    /**
     *  出力ストリームを返します。
     *
     *      @param      output      出力ストリーム
     *
     *      @return     出力ストリーム
     */
    protected OutputStream getOutput(OutputStream output) {
        return output;
    }

    /**
     *  文字列を URL 文字列に変換します。
     *
     *      @param      text        文字列
     *
     *      @return     URL 文字列
     * @throws IOException
     */
    protected String encode(String text) throws IOException {
        try {
            return URLEncoder.encode(text, "UTF-8");
        }
        catch (NoSuchMethodError e) {
            return PDFUrlTools.encode(text);
        }
        catch (UnsupportedEncodingException e) {
            return PDFUrlTools.encode(text);
        }
    }


    /**
     *  パラメータ文字列を返します。
     *
     *      @return     パラメータ文字列
     */
    protected String parameters() throws IOException {
        StringBuilder buf = new StringBuilder();
        String sep = "\n";

        if (printerName != null) {
            buf.append(sep + "printerName=" + printerName);
        }

        // 上下限チェックとデフォルトの追加
        if (numberOfCopy != null) {
            int val = numberOfCopy.intValue();
            if (val <= 0) {
                val = 1;
            }
            if (val > 999) {
                val = 999;
            }
            buf.append(sep + "numberOfCopy=" + String.valueOf(val));
        } else {
            buf.append(sep + "numberOfCopy=1");
        }

        if (selectedTray != null) {
            buf.append(sep + "selectedTray=" + selectedTray);
        }

        if (jobName != null && jobName.length() > 0) {
            buf.append(sep + "jobName=" + jobName);
        } else {
            jobName = "JobName_Default";
            buf.append(sep + "jobName=" + jobName);
        }

        if (doFit != null) {
            buf.append(sep + "doFit=" + doFit);
        }

        // Batch/Directともにページ指定可とする
        if (fromPage > 0) {
            buf.append(sep + "fromPage=" + (Integer.toString(fromPage)));
        }

        if ((toPage > 0) && (toPage >= fromPage)) {
            buf.append(sep + "toPage=" + (Integer.toString(toPage)));
        }

        return buf.toString();
    }

    /**
     *  バッファフラッシュします。
     */
    public void flush() throws IOException {
        pdfdoc.flush();
    }

    /**
     *  PDF データをバッファに書き出します。
     *
     *      @param      pdf         PDF データ
     */
    public void write(int pdf) throws IOException {
        pdfdoc.write(pdf);
    }

    /**
     *  PDF データを指定位置から指定バイト数だけバッファに書き出します。
     *
     *      @param      pdf         PDF データ
     *      @param      offset      開始位置
     *      @param      length      バイト数
     */
    public void write(byte[] pdf, int offset, int length) throws IOException {
        pdfdoc.write(pdf, offset, length);
    }

    /**
     *  PDF データをバッファに書き出します。
     *
     *      @param      pdf         PDF データ
     */
    public void write(byte[] pdf) throws IOException {
        pdfdoc.write(pdf);
    }

    /**
     *  書き込まれたバイト数を返します。
     *
     *      @return     書き込まれたバイト数
     */
    public int size() {
        return pdfdoc.size();
    }

    /**
     *  プリンタ名をセットします。
     *
     *      @param      value       プリンタ名
     */
    public void setPrinterName(String value) {
        printerName = value;
    }

    /**
     *  印刷部数をセットします。
     *
     *      @param      value       印刷部数
     */
    public void setNumberOfCopy(int value) {
        if (value < 1) {
            value = 1;
        }
        if (value > 999) {
            value = 999;
        }
        numberOfCopy = Integer.valueOf(value);
    }

    /**
     *  出力トレイをセットします。
     *
     *      @param      value       出力トレイ
     */
    public void setSelectedTray(String value) {
        selectedTray = value;
    }

    /**
     *  印刷識別子をセットします。
     *
     *      @param      value       印刷識別子
     */
    public void setJobName(String value) {
        if (value == null || value.length() == 0) {
            jobName = "JobName_Default";
        } else {
            jobName = value;
        }
    }

    /**
     *  ページサイズに合わせて印刷フラグをセットします。
     *
     *      @param      value       印刷識別子
     *
     *      @since      v5.0.0
     */
    public void setDoFit(boolean value) {
        if (value) {
            doFit = "true";
        } else {
            doFit = "false";
        }
    }

    /**
     *  開始ページ番号をセットします。
     *
     *      @param      value       開始ページ番号
     */
    public void setFromPage(int value) {
        fromPage = value;
    }

    /**
     *  終了ページ番号をセットします。
     *
     *      @param      value       終了ページ番号
     */
    public void setToPage(int value) {
        toPage = value;
    }

    /**
     *  ユーザパスワード(平文)をセットします。
     *      @param      value       平文パスワード
     */
    public void setPassword(String value) {
        if (value == null) {

        } else {
            userPassword = value;
        }

    }

    /**
    *   ユーザパスワード(base64エンコード済み)をセットします。
    *       @param      base64エンコード済みパスワード
    */
    public void setPasswordWithEncoded(String value) {
        if (value == null || value.length() == 0) {
            return;
        }

        // デコーダーを取得
        Base64.Decoder decoder = Base64.getDecoder();
        byte[] decodedString = decoder.decode(value.getBytes());

        userPassword = new String(decodedString, 0, decodedString.length);
    }
}
