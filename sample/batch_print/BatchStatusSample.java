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

import java.util.Collection;
import java.util.Iterator;

import com.brainsellers.bizstream.bizprint.PDFBatchPrintStatus;
import com.brainsellers.bizstream.bizprint.PDFBatchStatus;

/**
 * バッチ印刷ステータス照会サンプルプログラム
 *
 * バッチ印刷サーバに対して印刷状態を照会するスタンドアロンプログラムのサンプルです。
 * ジョブIDを指定すると特定ジョブの状態を、省略すると全ジョブの状態を表示します。
 *
 * 使い方:
 *   javac -cp bizprint-server-java-X.X.X.jar BatchStatusSample.java
 *
 *   特定ジョブの照会:
 *     java -cp .;bizprint-server-java-X.X.X.jar;zip4j-X.X.X.jar BatchStatusSample [jobId]
 *
 *   全ジョブの照会:
 *     java -cp .;bizprint-server-java-X.X.X.jar;zip4j-X.X.X.jar BatchStatusSample
 */
public class BatchStatusSample {

    // ========== 環境に合わせて変更してください ==========

    /** バッチ印刷サーバのホスト名 */
    private static final String BATCH_SERVER = "localhost";

    /** バッチ印刷サーバのポート番号 */
    private static final String PORT_NO = "3000";

    // ==================================================

    public static void main(String[] args) {
        try {
            PDFBatchStatus batchStatus = new PDFBatchStatus(
                    "http://" + BATCH_SERVER + ":" + PORT_NO + "/");

            if (args.length > 0) {
                // ジョブIDを指定して照会
                String jobId = args[0];
                System.out.println("=== ジョブID: " + jobId + " の状態を照会 ===");
                batchStatus.query(jobId);
            } else {
                // 全ジョブを照会
                System.out.println("=== 全ジョブの状態を照会 ===");
                batchStatus.query();
            }

            // 照会結果の表示
            String result = batchStatus.getResult();
            String errorCode = batchStatus.getErrorCode();
            String errorDetails = batchStatus.getErrorDetails();

            System.out.println("Result: " + result);
            System.out.println("ErrorCode: " + errorCode);
            System.out.println("ErrorDetails: " + errorDetails);

            // 各ジョブの印刷状態を表示
            Collection printStatusList = batchStatus.getPrintStatus();
            if (printStatusList == null || printStatusList.isEmpty()) {
                System.out.println("\n印刷状態の情報はありません。");
                return;
            }

            System.out.println("\n--- 印刷状態一覧 ---");
            for (Iterator it = printStatusList.iterator(); it.hasNext();) {
                PDFBatchPrintStatus ps = (PDFBatchPrintStatus) it.next();
                System.out.println("JobID: " + ps.getJobId());
                System.out.println("  JobName: " + ps.getJobName());
                System.out.println("  PrinterName: " + ps.getPrinterName());
                System.out.println("  DateTime: " + ps.getDateTime());
                System.out.println("  StatusCode: " + ps.getStatusCode());
                System.out.println("  Status: " + ps.getStatus());
                System.out.println("  ErrorCode: " + ps.getErrorCode());
                System.out.println("  ErrorCause: " + ps.getErrorCause());
                System.out.println("  ErrorDetails: " + ps.getErrorDetails());
                System.out.println();
            }

        } catch (Exception e) {
            e.printStackTrace();
            System.exit(1);
        }
    }
}
