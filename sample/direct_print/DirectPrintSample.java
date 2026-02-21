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

import java.io.FileInputStream;
import java.io.IOException;

import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

import com.brainsellers.bizstream.bizprint.PDFDirectPrintStream;

/**
 * ダイレクト印刷サンプルプログラム
 *
 * 既存のPDFファイルを読み込み、ダイレクト印刷で出力するサーブレットのサンプルです。
 * サーブレットコンテナ（Tomcat等）にデプロイして使用します。
 *
 * 使い方:
 *   1. 下記の定数を環境に合わせて変更してください。
 *   2. サーブレットコンテナにデプロイし、ブラウザからアクセスしてください。
 *   3. クライアントPCにダイレクト印刷クライアントがインストールされている必要があります。
 */
public class DirectPrintSample extends HttpServlet {

    // ========== 環境に合わせて変更してください ==========

    /** 印刷するPDFファイルのパス */
    private static final String PDF_FILE_PATH = "C:/temp/sample.pdf";

    /** プリンタ名 */
    private static final String PRINTER_NAME = "Microsoft Print to PDF";

    // ==================================================

    // HTTP Post リクエストの処理
    public void doPost(HttpServletRequest req, HttpServletResponse res)
            throws IOException, ServletException {
        doGet(req, res);
    }

    // HTTP Get リクエストの処理
    public void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        // ダイレクト印刷ストリームの生成
        PDFDirectPrintStream direct = new PDFDirectPrintStream(response);

        // プリンタ名
        direct.setPrinterName(PRINTER_NAME);

        // 印刷部数
        direct.setNumberOfCopy(1);

        // 出力トレイ（AUTO / UPPER / MIDDLE / LOWER / MANUAL）
        direct.setSelectedTray("AUTO");

        // 印刷ジョブ名
        direct.setJobName("DirectPrintSample");

        // 印刷ダイアログ表示（true: 表示する / false: 表示しない）
        direct.setPrintDialog(false);

        // 用紙に合わせて印刷
        direct.setDoFit(true);

        // 印刷開始ページ（1から開始）
        direct.setFromPage(1);

        // 印刷終了ページ（-1で最終ページまで）
        direct.setToPage(-1);

        // PDFファイルを読み込んでストリームに書き出す
        FileInputStream fis = null;
        try {
            fis = new FileInputStream(PDF_FILE_PATH);
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = fis.read(buffer)) != -1) {
                direct.write(buffer, 0, bytesRead);
            }
        } finally {
            if (fis != null) {
                fis.close();
            }
        }

        // 暗号化sppファイルを生成し、HTTPレスポンスとして送信
        direct.close();
    }
}
