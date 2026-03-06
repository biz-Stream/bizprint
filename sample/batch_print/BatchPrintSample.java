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

    /**
     * 印刷状態の取得間隔（ミリ秒）。
     * 1000ミリ秒未満はバッチ印刷サーバに負荷が掛かるため短くしすぎないこと。
     */
    private static final int STATUS_INTERVAL = 3000;

    // ==================================================

    public static void main(String[] args) {
        try {
            new BatchPrintSample().execute();
            System.exit(0);
        } catch (Exception e) {
            e.printStackTrace();
            System.exit(1);
        }
    }

    /**
     * 印刷要求を1回送信し、印刷状態を監視する。
     *
     * 通常は1回で完了するが、リトライ可能なエラーが発生した場合は
     * PDF送信からやり直す。
     */
    private void execute() throws Exception {
        // 印刷のループ
        // 通常は1回で抜けるが、PDF送信からやり直した場合は複数回ループする
        PRINT_LOOP : while (true) {

            PDFBatchPrintStream batch;
            try {
                // PDF読み込みとバッチ印刷サーバへの送信を行う
                batch = sendPrintRequest();
            } catch (Exception ex) {
                // 以下の場合に該当
                // ・PDF読み込みでエラーが発生した場合

                // エラー処理をして異常終了とする
                throw new Exception("【エラー1】PDF読み込みでエラーが発生しました。プログラムを終了します。", ex);
            }

            String jobId = batch.getJobId();
            String result = batch.getResult();
            String errorCode = batch.getErrorCode();
            String errorDetails = batch.getErrorDetails();
            if (errorDetails != null) {
                errorDetails = URLDecoder.decode(errorDetails, "UTF-8");
            }

            System.out.println("==============================");
            System.out.println("JobID=" + jobId);
            System.out.println("Result=" + result);
            System.out.println("ErrorCode=" + errorCode);
            System.out.println("ErrorDetails=" + errorDetails);
            System.out.println("==============================");

            if (result == null || result.equals("")) {
                // 以下の場合に該当
                // ・バッチ印刷が起動していない場合
                // ・接続先が誤っている場合
                // ・バッチ印刷が印刷要求または印刷状態要求を受け付けられない場合
                System.err.println("【エラー2】バッチ印刷の印刷要求結果を取得できませんでした。再度PDF送信からやり直します。");

                // 待ち時間の後、再度印刷要求を行う
                sleep();
                continue PRINT_LOOP;
            }

            if (result.equalsIgnoreCase("FAIL")) {
                if (errorCode != null && (errorCode.equals("114") || errorCode.equals("405") || errorCode.equals("506"))) {
                    // 以下の場合に該当
                    // 114=バッチ印刷のキュー上限に達した場合
                    // 405=スプールタイムアウトが発生した場合
                    // 506=指定したJobIDに対応する印刷状態が存在しない場合(バッチ印刷が再起動された場合など)
                    System.err.println("【エラー3】バッチ印刷でエラーが発生しました。再度PDF送信からやり直します。エラーコード: " + errorCode + ", エラーメッセージ: " + errorDetails);

                    // 待ち時間の後、再度印刷要求を行う
                    sleep();
                    continue PRINT_LOOP;
                } else {
                    // 上記以外の場合に該当
                    // エラー処理をして異常終了とする
                    throw new Exception("【エラー4】バッチ印刷でエラーが発生しました。プログラムを終了します。エラーコード: " + errorCode + ", エラーメッセージ: " + errorDetails);
                }
            } else {
                // 待ち時間の後、印刷状態取得を行う
                sleep();

                // 印刷状態取得ループ
                // ステータスコードが0006になるまでINTERVAL間隔で印刷状態取得を繰り返す
                STATUS_LOOP : while (true) {
                    // 印刷状態を取得する
                    PDFBatchPrintStatus printStatus = getBatchPrintStatus(jobId);

                    if (printStatus == null) {
                        // 以下の場合に該当
                        // ・バッチ印刷が印刷状態要求を受け付けられなかった場合

                        // 【注意】
                        // このパターンは印刷状態取得に失敗するが、印刷成功・失敗は不定となる
                        // PDF送信からやり直した場合、二重に印刷されてしまう可能性があるため注意
                        // 厳密に印刷結果を管理するには異常終了としてプログラムを止めた後に出力結果を確認、未印刷分からやり直すこと

                        // 二重に印刷される可能性を許容する場合はこちらのロジックを使用する
                        System.err.println("【エラー5】バッチ印刷の印刷状態を取得できませんでした。再度PDF送信からやり直します。");
                        // 待ち時間の後、再度印刷要求を行う
                        sleep();
                        continue PRINT_LOOP;

                        // 厳密に出力を管理するにはこちらのロジックを使用する
                        // エラー処理をして異常終了とする
                        // throw new Exception("【エラー5】バッチ印刷の印刷状態を取得できませんでした。プログラムを終了します。");
                    }

                    String dateTime = printStatus.getDateTime();
                    String errorCause = printStatus.getErrorCause();
                    errorCode = printStatus.getErrorCode();
                    errorDetails = printStatus.getErrorDetails();
                    String jobName = printStatus.getJobName();
                    String printerName = printStatus.getPrinterName();
                    String status = printStatus.getStatus();
                    String statusCode = printStatus.getStatusCode();

                    System.out.println("------------------------------");
                    System.out.println("dateTime=" + dateTime);
                    System.out.println("errorCause=" + errorCause);
                    System.out.println("errorCode=" + errorCode);
                    System.out.println("errorDetails=" + errorDetails);
                    System.out.println("jobId=" + jobId);
                    System.out.println("jobName=" + jobName);
                    System.out.println("printerName=" + printerName);
                    System.out.println("status=" + status);
                    System.out.println("statusCode=" + statusCode);
                    System.out.println("------------------------------");

                    if (statusCode == null || statusCode.equals("")) {
                        // 以下の場合に該当
                        // ・バッチ印刷が印刷要求または印刷状態要求を受け付けられない場合
                        // ・印刷状態最大保持数、印刷状態最大保持時間の値が小さく、印刷状態がクリアされてしまった場合

                        // 【注意】
                        // このパターンは印刷状態取得に失敗するが、印刷成功・失敗は不定となる
                        // PDF送信からやり直した場合、二重に印刷されてしまう可能性があるため注意
                        // 厳密に印刷結果を管理するには異常終了としてプログラムを止めた後に出力結果を確認、未印刷分からやり直すこと

                        // 二重に印刷される可能性を許容する場合はこちらのロジックを使用する
                        System.err.println("【エラー6】バッチ印刷の印刷状態を取得できませんでした。再度PDF送信からやり直します。");
                        // 待ち時間の後、再度印刷要求を行う
                        sleep();
                        continue PRINT_LOOP;

                        // 厳密に出力を管理するにはこちらのロジックを使用する
                        // エラー処理をして異常終了とする
                        // throw new Exception("【エラー6】バッチ印刷の印刷状態を取得できませんでした。プログラムを終了します。");
                    } else if (statusCode.equals("0002") || statusCode.equals("0004")) {
                        // 0002 (印刷指示受付)
                        // 0004 (印刷中)

                        // 以下の場合に該当
                        // ・バッチ印刷のキューに存在するが、キュー取り出し待ち状態の場合
                        // ・キューから取り出されてAdobeモジュールによるPDFロードが行われ、プリントスプーラへ入るまでの間の場合

                        // 待ち時間の後、再度印刷状態取得を行う
                        sleep();
                        continue STATUS_LOOP;
                    } else if (statusCode.equals("0006")) {
                        // 0006 (印刷要求送信完了)

                        // 以下の場合に該当
                        // ・正常にプリントスプーラへ送信完了した場合
                        System.out.println("【正常】プリントスプーラへの送信が完了しました。JobID: " + jobId);

                        break PRINT_LOOP;
                    } else if (statusCode.equals("0008")) {
                        // 0008 (印刷異常終了)

                        if (errorCode != null && errorCode.equals("000")) {
                            // 以下の場合に該当
                            // ・印刷状態は0008で確定しているがエラーコードが確定するまでの間に印刷状態取得が行われた場合

                            // 待ち時間の後、再度印刷状態取得を行う
                            sleep();
                            continue STATUS_LOOP;
                        } else if (errorCode != null && errorCode.equals("405")) {
                            // 以下の場合に該当
                            // 405=スプールタイムアウトが発生した場合
                            System.err.println("【エラー7】バッチ印刷でエラーが発生しました。再度PDF送信からやり直します。エラーコード: " + errorCode + ", エラーメッセージ: " + errorDetails);

                            // 待ち時間の後、再度印刷要求を行う
                            sleep();
                            continue PRINT_LOOP;
                        } else {
                            // 以下の場合に該当
                            // ・AdobeモジュールによるPDFロード、またはプリントスプーラへの投入時にエラーが発生した場合

                            // エラー処理をして異常終了とする
                            throw new Exception("【エラー8】印刷異常終了となりました。プログラムを終了します。エラーコード: " + errorCode + ", エラーメッセージ: " + errorDetails);
                        }
                    }
                }
            }
        }
    }

    /**
     * PDFファイルを読み込み、バッチ印刷サーバへ送信する。
     */
    private PDFBatchPrintStream sendPrintRequest() throws IOException {
        // バッチ印刷ストリームの生成
        PDFBatchPrintStream batch = new PDFBatchPrintStream(
                "http://" + BATCH_SERVER + ":" + PORT_NO + "/");

        // プリンタ名
        batch.setPrinterName(PRINTER_NAME);

        // 印刷部数
        batch.setNumberOfCopy(1);

        // 出力トレイ（AUTO / UPPER / MIDDLE / LOWER / MANUAL）
        batch.setSelectedTray("AUTO");

        // 印刷ジョブ名（2バイト文字を使用する場合はUTF-8）
        batch.setJobName("BatchPrintSample");

        // 用紙サイズに合わせて印刷
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
     * 指定したJobIDをキーとして印刷状態を取得する。
     *
     * @param jobId JobID
     * @return 印刷状態。取得できなかった場合はnull。
     */
    private PDFBatchPrintStatus getBatchPrintStatus(String jobId) throws Exception {
        // バッチ印刷サーバを指定
        PDFBatchStatus batchStatus = new PDFBatchStatus("http://" + BATCH_SERVER + ":" + PORT_NO + "/");
        // JobIDを指定して印刷状態を取得
        batchStatus.query(jobId);
        Collection printStatusList = batchStatus.getPrintStatus();
        if (printStatusList == null || printStatusList.isEmpty()) {
            return null;
        }
        return (PDFBatchPrintStatus) printStatusList.iterator().next();
    }

    /**
     * 指定した時間待つ。
     */
    private void sleep() {
        try {
            Thread.sleep(STATUS_INTERVAL);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
    }
}
