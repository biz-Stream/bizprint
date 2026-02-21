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
import java.net.URLDecoder;
import java.util.Collection;
import java.util.Iterator;

import com.brainsellers.bizstream.bizprint.PDFBatchPrintStatus;
import com.brainsellers.bizstream.bizprint.PDFBatchPrintStream;
import com.brainsellers.bizstream.bizprint.PDFBatchStatus;

/**
 * バッチ印刷サンプルプログラム
 *
 * 既存のPDFファイルを読み込み、バッチ印刷で出力するスタンドアロンプログラムのサンプルです。
 * 印刷要求の送信後、ステータスをポーリングして印刷完了を確認します。
 *
 * 使い方:
 *   1. 下記の定数を環境に合わせて変更してください。
 *   2. バッチ印刷クライアントが起動していることを確認してください。
 *   3. 以下のコマンドでコンパイル・実行してください。
 *
 *      javac -cp bizprint-server-java-X.X.X.jar BatchPrintSample.java
 *      java -cp .;bizprint-server-java-X.X.X.jar;zip4j-X.X.X.jar BatchPrintSample
 */
public class BatchPrintSample {

    // ========== 環境に合わせて変更してください ==========

    /** バッチ印刷サーバのホスト名 */
    private static final String BATCH_SERVER = "localhost";

    /** バッチ印刷サーバのポート番号 */
    private static final String PORT_NO = "3000";

    /** プリンタ名 */
    private static final String PRINTER_NAME = "Microsoft Print to PDF";

    /** 印刷するPDFファイルのパス */
    private static final String PDF_FILE_PATH = "C:/temp/sample.pdf";

    /** 印刷状態の取得間隔（ミリ秒）。1000ミリ秒未満はサーバに負荷が掛かるため非推奨 */
    private static final int STATUS_INTERVAL = 3000;

    // ==================================================

    public static void main(String[] args) {
        try {
            // 印刷要求を送信
            PDFBatchPrintStream batch = sendPrintRequest();

            // 印刷要求の結果を確認
            String jobId = batch.getJobId();
            String result = batch.getResult();
            String errorCode = batch.getErrorCode();
            String errorDetails = batch.getErrorDetails();
            if (errorDetails != null) {
                errorDetails = URLDecoder.decode(errorDetails, "UTF-8");
            }

            System.out.println("=== 印刷要求結果 ===");
            System.out.println("JobID: " + jobId);
            System.out.println("Result: " + result);
            System.out.println("ErrorCode: " + errorCode);
            System.out.println("ErrorDetails: " + errorDetails);

            // 印刷要求が失敗した場合
            if (result == null || result.equals("") || result.equalsIgnoreCase("FAIL")) {
                System.err.println("印刷要求が失敗しました。");
                System.exit(1);
            }

            // 印刷状態をポーリング
            System.out.println("\n=== 印刷状態の監視を開始 ===");
            pollPrintStatus(jobId);

        } catch (Exception e) {
            e.printStackTrace();
            System.exit(1);
        }
    }

    /**
     * PDFファイルを読み込み、バッチ印刷サーバへ送信する。
     */
    private static PDFBatchPrintStream sendPrintRequest() throws IOException {
        // バッチ印刷ストリームの生成
        PDFBatchPrintStream batch = new PDFBatchPrintStream(
                "http://" + BATCH_SERVER + ":" + PORT_NO + "/");

        // プリンタ名
        batch.setPrinterName(PRINTER_NAME);

        // 印刷部数
        batch.setNumberOfCopy(1);

        // 出力トレイ（AUTO / UPPER / MIDDLE / LOWER / MANUAL）
        batch.setSelectedTray("AUTO");

        // 印刷ジョブ名
        batch.setJobName("BatchPrintSample");

        // 用紙に合わせて印刷
        batch.setDoFit(true);

        // 印刷開始ページ（1から開始）
        batch.setFromPage(1);

        // 印刷終了ページ（-1で最終ページまで）
        batch.setToPage(-1);

        // PDFファイルを読み込んでストリームに書き出す
        FileInputStream fis = null;
        try {
            fis = new FileInputStream(PDF_FILE_PATH);
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = fis.read(buffer)) != -1) {
                batch.write(buffer, 0, bytesRead);
            }
        } finally {
            if (fis != null) {
                fis.close();
            }
        }

        // 暗号化sppファイルを生成し、バッチ印刷サーバへ送信
        batch.close();

        return batch;
    }

    /**
     * 指定したジョブIDの印刷状態をポーリングし、完了またはエラーまで待機する。
     *
     * ステータスコード:
     *   0002 - 印刷指示受付（キュー待ち）
     *   0004 - 印刷中
     *   0006 - 印刷要求送信完了（正常終了）
     *   0008 - 印刷異常終了
     */
    private static void pollPrintStatus(String jobId) throws Exception {
        PDFBatchStatus batchStatus = new PDFBatchStatus(
                "http://" + BATCH_SERVER + ":" + PORT_NO + "/");

        while (true) {
            Thread.sleep(STATUS_INTERVAL);

            // 印刷状態を取得
            batchStatus.query(jobId);
            Collection printStatusList = batchStatus.getPrintStatus();
            if (printStatusList == null || printStatusList.isEmpty()) {
                System.out.println("印刷状態を取得できませんでした。再試行します...");
                continue;
            }

            PDFBatchPrintStatus printStatus =
                    (PDFBatchPrintStatus) printStatusList.iterator().next();
            String statusCode = printStatus.getStatusCode();
            String status = printStatus.getStatus();

            System.out.println("StatusCode: " + statusCode + " (" + status + ")");

            if (statusCode == null || statusCode.equals("")) {
                // ステータスが取得できない場合は再試行
                System.out.println("ステータスコードが空です。再試行します...");
                continue;
            } else if (statusCode.equals("0002") || statusCode.equals("0004")) {
                // 印刷指示受付 / 印刷中 → 待機して再試行
                continue;
            } else if (statusCode.equals("0006")) {
                // 印刷要求送信完了（正常終了）
                System.out.println("印刷が正常に完了しました。JobID: " + jobId);
                return;
            } else if (statusCode.equals("0008")) {
                // 印刷異常終了
                String errCode = printStatus.getErrorCode();
                String errDetails = printStatus.getErrorDetails();
                System.err.println("印刷異常終了。ErrorCode: " + errCode
                        + ", ErrorDetails: " + errDetails);
                return;
            }
        }
    }
}
