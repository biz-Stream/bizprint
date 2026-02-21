# API リファレンス

bizprint サーバーライブラリ（Java / C#）の API リファレンスです。

パッケージ: `com.brainsellers.bizstream.bizprint`

---

## 目次

- [クラス一覧](#クラス一覧)
- [PDFCommonPrintStream（共通基底クラス）](#pdfcommonprintstream共通基底クラス)
- [PDFDirectPrintStream（ダイレクト印刷）](#pdfdirectprintstreamダイレクト印刷)
- [PDFBatchPrintStream（バッチ印刷）](#pdfbatchprintstreamバッチ印刷)
- [PDFBatchStatus（バッチ印刷ステータス照会）](#pdfbatchstatusバッチ印刷ステータス照会)
- [PDFBatchPrintStatus（個別ジョブステータス）](#pdfbatchprintstatus個別ジョブステータス)
- [BizPrintException（例外）](#bizprintexception例外)
- [トレイ定数](#トレイ定数)
- [ジョブステータス定数](#ジョブステータス定数)
- [C# 版との対応](#c-版との対応)

---

## クラス一覧

| クラス名 | 概要 |
|---|---|
| `PDFCommonPrintStream` | ダイレクト印刷・バッチ印刷共通の抽象基底クラス |
| `PDFDirectPrintStream` | ダイレクト印刷ストリーム生成クラス |
| `PDFBatchPrintStream` | バッチ印刷ストリーム生成クラス |
| `PDFBatchStatus` | バッチ印刷ステータス照会クラス |
| `PDFBatchPrintStatus` | 個別ジョブの印刷ステータスを保持するクラス |
| `BizPrintException` | bizprint 固有の例外クラス |

---

## PDFCommonPrintStream（共通基底クラス）

ダイレクト印刷・バッチ印刷で共通の抽象基底クラスです。`java.io.OutputStream` を継承しています。

`PDFDirectPrintStream` と `PDFBatchPrintStream` の両方で使用できる共通メソッドを提供します。

### コンストラクタ

```java
public PDFCommonPrintStream()
```

インスタンスを生成し、内部バッファを初期化します。直接インスタンス化はできません（抽象クラス）。

### public メソッド

#### setPrinterName

```java
public void setPrinterName(String value)
```

印刷先のプリンタ名をセットします。

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `String` | プリンタ名 |

#### setNumberOfCopy

```java
public void setNumberOfCopy(int value)
```

印刷部数をセットします。指定可能な範囲は 1 ~ 999 です。範囲外の値が指定された場合、自動的に範囲内に丸められます。

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `int` | 印刷部数（1 ~ 999） |

> **注意**: `PDFDirectPrintStream.setPrintDialog(true)` で印刷ダイアログを表示する場合、このメソッドでの指定は無視されます。

#### setSelectedTray

```java
public void setSelectedTray(String value)
```

出力トレイをセットします。[トレイ定数](#トレイ定数)を使用できます。

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `String` | 出力トレイ名 |

#### setJobName

```java
public void setJobName(String value)
```

印刷識別子（ジョブ名）をセットします。`null` または空文字が指定された場合は `"JobName_Default"` が使用されます。

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `String` | 印刷識別子 |

#### setDoFit

```java
public void setDoFit(boolean value)
```

ページサイズに合わせて印刷するかどうかのフラグをセットします。

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `boolean` | `true`: ページサイズに合わせる / `false`: 合わせない |

> **注意**: `PDFDirectPrintStream.setPrintDialog(true)` で印刷ダイアログを表示する場合、このメソッドでの指定は無視されます。

#### setFromPage

```java
public void setFromPage(int value)
```

印刷開始ページ番号をセットします。1 から開始します。

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `int` | 開始ページ番号（1 以上） |

> **注意**: `PDFDirectPrintStream.setPrintDialog(true)` で印刷ダイアログを表示する場合、このメソッドでの指定は無視されます。

#### setToPage

```java
public void setToPage(int value)
```

印刷終了ページ番号をセットします。

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `int` | 終了ページ番号 |

> **注意**: `PDFDirectPrintStream.setPrintDialog(true)` で印刷ダイアログを表示する場合、このメソッドでの指定は無視されます。

#### setPassword

```java
public void setPassword(String value)
```

ユーザパスワード（平文）をセットします。PDF ファイルにパスワードが設定されている場合に使用します。

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `String` | 平文パスワード |

#### setPasswordWithEncoded

```java
public void setPasswordWithEncoded(String value)
```

ユーザパスワード（Base64 エンコード済み）をセットします。内部で Base64 デコードしてからパスワードとして使用します。

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `String` | Base64 エンコード済みパスワード |

#### write

```java
public void write(int pdf) throws IOException
public void write(byte[] pdf) throws IOException
public void write(byte[] pdf, int offset, int length) throws IOException
```

PDF データを内部バッファに書き出します。

| パラメータ | 型 | 説明 |
|---|---|---|
| `pdf` | `int` / `byte[]` | PDF データ |
| `offset` | `int` | 開始位置 |
| `length` | `int` | バイト数 |

#### flush

```java
public void flush() throws IOException
```

内部バッファをフラッシュします。

#### size

```java
public int size()
```

書き込まれたバイト数を返します。

| 戻り値 | 型 | 説明 |
|---|---|---|
| - | `int` | 書き込み済みバイト数 |

---

## PDFDirectPrintStream（ダイレクト印刷）

ダイレクト印刷の spp ファイルを生成し、HTTP レスポンスとしてクライアントに送信するためのクラスです。`PDFCommonPrintStream` を継承しています。

サーブレット内で使用し、ブラウザ経由でクライアント PC のダイレクト印刷サービスに印刷データを配信します。

### コンストラクタ

```java
public PDFDirectPrintStream(HttpServletResponse response)
```

インスタンスを生成し、指定された HTTP 応答オブジェクトで初期化します。

| パラメータ | 型 | 説明 |
|---|---|---|
| `response` | `jakarta.servlet.http.HttpServletResponse` | HTTP 応答オブジェクト |

### public メソッド

[PDFCommonPrintStream の共通メソッド](#public-メソッド)に加えて、以下のメソッドが使用できます。

#### close

```java
public void close() throws IOException
```

バッファをクローズし、暗号化された spp ファイルを生成して HTTP レスポンスとして送信します。PDF データの書き込みが完了した後に呼び出してください。

レスポンスの Content-Type は `application/x-spp` に設定されます。

#### setResponseUrl

```java
public void setResponseUrl(String value)
```

印刷完了後にブラウザがリダイレクトする応答 URL をセットします。

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `String` | 印刷応答 URL |

#### setSaveFileName

```java
public void setSaveFileName(String value)
```

ファイル保存先のファイル名をセットします。

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `String` | ファイル保存名 |

#### setTarget

```java
public void setTarget(String value)
```

ブラウザの target 名をセットします。

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `String` | ブラウザ target 名 |

#### setPrintDialog

```java
public void setPrintDialog(boolean value)
```

印刷ダイアログ表示フラグをセットします。

`true` を指定した場合、以下のメソッドでの設定は無視されます:

- `setNumberOfCopy`（印刷部数）
- `setFromPage`（開始ページ番号）
- `setToPage`（終了ページ番号）
- `setDoFit`（ページサイズに合わせて印刷フラグ）

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `boolean` | `true`: ダイアログを表示 / `false`: 表示しない |

#### setSppNameUnified

```java
public void setSppNameUnified(Boolean value)
```

spp ファイル名を一意化するかどうかを指定します。デフォルトは `true`（一意化する）です。

`true` の場合、ダウンロード時のファイル名に現在時刻とランダム値が付与されます。

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `Boolean` | `true`: 一意化する / `false`: 一意化しない |

### 使用例

```java
import java.io.FileInputStream;
import java.io.IOException;

import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

import com.brainsellers.bizstream.bizprint.PDFDirectPrintStream;

public void doGet(HttpServletRequest request, HttpServletResponse response)
        throws IOException {

    // ダイレクト印刷ストリームの生成
    PDFDirectPrintStream direct = new PDFDirectPrintStream(response);

    // プリンタ名
    direct.setPrinterName("Microsoft Print to PDF");

    // 印刷部数
    direct.setNumberOfCopy(1);

    // 出力トレイ
    direct.setSelectedTray(PDFDirectPrintStream.TRAY_AUTO);

    // 印刷ジョブ名
    direct.setJobName("DirectPrintSample");

    // 印刷ダイアログ表示
    direct.setPrintDialog(false);

    // 用紙に合わせて印刷
    direct.setDoFit(true);

    // 印刷範囲
    direct.setFromPage(1);
    direct.setToPage(-1);

    // PDF ファイルを読み込んでストリームに書き出す
    FileInputStream fis = null;
    try {
        fis = new FileInputStream("C:/temp/sample.pdf");
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

    // 暗号化 spp ファイルを生成し、HTTP レスポンスとして送信
    direct.close();
}
```

---

## PDFBatchPrintStream（バッチ印刷）

バッチ印刷の spp ファイルを生成し、バッチ印刷サーバへ HTTP POST で送信するためのクラスです。`PDFCommonPrintStream` を継承しています。

サーブレットコンテナが不要で、スタンドアロンプログラムからも使用できます。

### コンストラクタ

```java
public PDFBatchPrintStream(String inputServerUrl)
```

インスタンスを生成し、指定されたバッチ印刷サーバの URL で初期化します。

| パラメータ | 型 | 説明 |
|---|---|---|
| `inputServerUrl` | `String` | バッチ印刷サーバの URL（例: `"http://localhost:3000/"`） |

### public メソッド

[PDFCommonPrintStream の共通メソッド](#public-メソッド)に加えて、以下のメソッドが使用できます。

#### close

```java
public void close() throws IOException
```

バッファをクローズし、暗号化された spp ファイルを生成してバッチ印刷サーバへ HTTP POST で送信します。PDF データの書き込みが完了した後に呼び出してください。

送信先 URL は、コンストラクタで指定した URL に `/doprint` パスが自動付与されます。

送信後、サーバからのレスポンスを解析し、以下の情報を取得できます:

- `getResult()` - 実行結果
- `getErrorCode()` - エラーコード
- `getErrorCause()` - エラー原因
- `getErrorDetails()` - エラー詳細
- `getJobId()` - 印刷指示 ID

#### setBatchPrintPort

```java
public void setBatchPrintPort(String value)
```

バッチ印刷サーバへの接続ポート番号を指定します。デフォルトは `3000` です。

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `String` | ポート番号（1024 ~ 65535）。範囲外の場合はデフォルト（3000）が使用されます |

#### createConnectUrl

```java
public void createConnectUrl()
```

接続に使用する URL を組み立てます。通常は `close()` 内部で自動的に呼び出されるため、明示的に呼び出す必要はありません。

URL に `http://` プレフィックスが無い場合は自動付与されます。ポート番号の指定が無い場合は `setBatchPrintPort` で設定された値（デフォルト: 3000）が使用されます。

#### getResult

```java
public String getResult()
```

印刷要求の結果を返します。`close()` 呼び出し後に使用できます。

| 戻り値 | 型 | 説明 |
|---|---|---|
| - | `String` | 結果（`"OK"` / `"FAIL"` など） |

#### getErrorCode

```java
public String getErrorCode()
```

エラーコードを返します。

| 戻り値 | 型 | 説明 |
|---|---|---|
| - | `String` | エラーコード（正常時は `null`） |

#### getErrorCause

```java
public String getErrorCause()
```

エラーの原因を返します。

| 戻り値 | 型 | 説明 |
|---|---|---|
| - | `String` | エラー原因（正常時は `null`） |

#### getErrorDetails

```java
public String getErrorDetails()
```

エラーの内容を返します。

| 戻り値 | 型 | 説明 |
|---|---|---|
| - | `String` | エラー詳細（正常時は `null`） |

#### getJobId

```java
public String getJobId()
```

印刷指示 ID を返します。この ID を使って `PDFBatchStatus` でステータスを照会できます。

| 戻り値 | 型 | 説明 |
|---|---|---|
| - | `String` | 印刷指示 ID |

### 使用例

```java
import java.io.FileInputStream;
import java.io.IOException;

import com.brainsellers.bizstream.bizprint.PDFBatchPrintStream;

public void batchPrint() throws IOException {
    // バッチ印刷ストリームの生成
    PDFBatchPrintStream batch = new PDFBatchPrintStream("http://localhost:3000/");

    // プリンタ名
    batch.setPrinterName("Microsoft Print to PDF");

    // 印刷部数
    batch.setNumberOfCopy(1);

    // 出力トレイ
    batch.setSelectedTray(PDFBatchPrintStream.TRAY_AUTO);

    // 印刷ジョブ名
    batch.setJobName("BatchPrintSample");

    // 用紙に合わせて印刷
    batch.setDoFit(true);

    // 印刷範囲
    batch.setFromPage(1);
    batch.setToPage(-1);

    // PDF ファイルを読み込んでストリームに書き出す
    FileInputStream fis = null;
    try {
        fis = new FileInputStream("C:/temp/sample.pdf");
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

    // 暗号化 spp ファイルを生成し、バッチ印刷サーバへ送信
    batch.close();

    // 送信結果の確認
    String jobId = batch.getJobId();
    String result = batch.getResult();
    System.out.println("JobID: " + jobId + ", Result: " + result);
}
```

---

## PDFBatchStatus（バッチ印刷ステータス照会）

バッチ印刷サーバに対して印刷ステータスを照会するためのクラスです。

バッチ印刷サーバの `/getstatus` エンドポイントに HTTP POST で問い合わせを行い、結果を XML としてパースします。

### コンストラクタ

```java
public PDFBatchStatus(String inputServerUrl)
```

インスタンスを生成し、指定されたバッチ印刷サーバの URL で初期化します。

| パラメータ | 型 | 説明 |
|---|---|---|
| `inputServerUrl` | `String` | バッチ印刷サーバの URL（例: `"http://localhost:3000/"`） |

### public メソッド

#### query

```java
public void query(String jobId)
        throws IOException, BizPrintException, ParserConfigurationException, SAXException
```

指定した印刷指示 ID のステータスを照会します。

| パラメータ | 型 | 説明 |
|---|---|---|
| `jobId` | `String` | 印刷指示 ID（`null` の場合は全ジョブを照会） |

| 例外 | 説明 |
|---|---|
| `IOException` | 通信エラー |
| `BizPrintException` | HTTP レスポンスが 200 以外の場合 |
| `ParserConfigurationException` | XML パーサーの構成エラー |
| `SAXException` | XML パースエラー |

```java
public void query()
        throws IOException, BizPrintException, ParserConfigurationException, SAXException
```

全ジョブのステータスを照会します。`query(null)` と同等です。

#### setBatchStatusPort

```java
public void setBatchStatusPort(String value)
```

ステータス照会先のポート番号を指定します。デフォルトは `3000` です。

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `String` | ポート番号（1024 ~ 65535）。範囲外の場合はデフォルト（3000）が使用されます |

#### createConnectUrl

```java
public void createConnectUrl()
```

接続に使用する URL を組み立てます。通常は `query()` 内部で自動的に呼び出されます。

URL にポート番号が無い場合は `setBatchStatusPort` で設定された値が使用され、`/getstatus` パスが自動付与されます。

#### getResult

```java
public String getResult()
```

照会結果を返します。

| 戻り値 | 型 | 説明 |
|---|---|---|
| - | `String` | 結果 |

#### getErrorCode

```java
public String getErrorCode()
```

エラーコードを返します。

| 戻り値 | 型 | 説明 |
|---|---|---|
| - | `String` | エラーコード |

#### getErrorCause

```java
public String getErrorCause()
```

エラーの原因を返します。

| 戻り値 | 型 | 説明 |
|---|---|---|
| - | `String` | エラー原因 |

#### getErrorDetails

```java
public String getErrorDetails()
```

エラーの内容を返します。

| 戻り値 | 型 | 説明 |
|---|---|---|
| - | `String` | エラー詳細 |

#### getPrintStatus (リスト)

```java
public Collection getPrintStatus()
```

全ジョブの印刷ステータスをリストとして返します。リストの各要素は `PDFBatchPrintStatus` オブジェクトです。

| 戻り値 | 型 | 説明 |
|---|---|---|
| - | `Collection` | `PDFBatchPrintStatus` のコレクション |

#### getPrintStatus (個別)

```java
public PDFBatchPrintStatus getPrintStatus(String jobId)
```

指定した印刷指示 ID のステータスを返します。

| パラメータ | 型 | 説明 |
|---|---|---|
| `jobId` | `String` | 印刷指示 ID |

| 戻り値 | 型 | 説明 |
|---|---|---|
| - | `PDFBatchPrintStatus` | 印刷ステータス（該当なしの場合は `null`） |

### 使用例

```java
import java.util.Collection;
import java.util.Iterator;

import com.brainsellers.bizstream.bizprint.PDFBatchPrintStatus;
import com.brainsellers.bizstream.bizprint.PDFBatchStatus;

public void checkStatus(String jobId) throws Exception {
    PDFBatchStatus batchStatus = new PDFBatchStatus("http://localhost:3000/");

    // 特定ジョブの照会
    batchStatus.query(jobId);

    // 照会結果の確認
    String result = batchStatus.getResult();
    System.out.println("Result: " + result);

    // 各ジョブの印刷状態を確認
    Collection printStatusList = batchStatus.getPrintStatus();
    for (Iterator it = printStatusList.iterator(); it.hasNext();) {
        PDFBatchPrintStatus ps = (PDFBatchPrintStatus) it.next();
        System.out.println("JobID: " + ps.getJobId());
        System.out.println("  StatusCode: " + ps.getStatusCode());
        System.out.println("  Status: " + ps.getStatus());
    }
}
```

---

## PDFBatchPrintStatus（個別ジョブステータス）

バッチ印刷の個別ジョブの印刷ステータスを保持するクラスです。

`PDFBatchStatus.query()` の結果として取得され、ジョブごとの詳細情報を提供します。

### public メソッド

#### getJobId

```java
public String getJobId()
```

印刷指示 ID を返します。

#### getJobName

```java
public String getJobName()
```

印刷識別子（ジョブ名）を返します。

#### getPrinterName

```java
public String getPrinterName()
```

プリンター名を返します。

#### getDateTime

```java
public String getDateTime()
```

タイムスタンプを返します。

#### getStatusCode

```java
public String getStatusCode()
```

印刷状態コードを返します。主なステータスコードは以下の通りです:

| コード | 意味 |
|---|---|
| `0002` | 印刷指示受付（キュー待ち） |
| `0004` | 印刷中 |
| `0006` | 印刷要求送信完了（正常終了） |
| `0008` | 印刷異常終了 |

#### getStatus

```java
public String getStatus()
```

印刷状態（テキスト）を返します。

#### getErrorCode

```java
public String getErrorCode()
```

エラーコードを返します。

#### getErrorCause

```java
public String getErrorCause()
```

エラーの原因を返します。

#### getErrorDetails

```java
public String getErrorDetails()
```

エラーの内容を返します。

---

## BizPrintException（例外）

bizprint 固有のカスタム例外クラスです。`java.lang.Exception` を継承しています。

主に `PDFBatchStatus.query()` で HTTP レスポンスが正常でない場合にスローされます。

### コンストラクタ

```java
public BizPrintException(String message)
```

メッセージを指定して例外を生成します。

```java
public BizPrintException(String message, Throwable cause)
```

メッセージと原因を指定して例外を生成します。

---

## トレイ定数

`PDFCommonPrintStream` クラスで定義されているトレイ定数です。`setSelectedTray()` の引数に使用します。

| 定数名 | 値 | 説明 |
|---|---|---|
| `TRAY_UPPER` | `"UPPER"` | 上段トレイ |
| `TRAY_MIDDLE` | `"MIDDLE"` | 中段トレイ |
| `TRAY_LOWER` | `"LOWER"` | 下段トレイ |
| `TRAY_MANUAL` | `"MANUAL"` | 手差しトレイ |
| `TRAY_AUTO` | `"AUTO"` | 自動選択 |

---

## ジョブステータス定数

`PDFBatchPrintStatus` クラスで定義されているジョブステータスのビットフラグ定数です。

| 定数名 | 値 | 説明 |
|---|---|---|
| `JOB_STATUS_PAUSED` | `0x00000001` | 一時停止 |
| `JOB_STATUS_ERROR` | `0x00000002` | エラー |
| `JOB_STATUS_DELETING` | `0x00000004` | 削除中 |
| `JOB_STATUS_SPOOLING` | `0x00000008` | スプール中 |
| `JOB_STATUS_PRINTING` | `0x00000010` | 印刷中 |
| `JOB_STATUS_OFFLINE` | `0x00000020` | オフライン |
| `JOB_STATUS_PAPEROUT` | `0x00000040` | 用紙切れ |
| `JOB_STATUS_PRINTED` | `0x00000080` | 完了 |
| `JOB_STATUS_DELETED` | `0x00000100` | 削除 |
| `JOB_STATUS_BLOCKED_DEVQ` | `0x00000200` | 不正なドライバ |
| `JOB_STATUS_USER_INTERVENTION` | `0x00000400` | プリンタ不良 |
| `JOB_STATUS_RESTART` | `0x00000800` | 再起動中 |
| `JOB_STATUS_COMPLETE` | `0x00001000` | 完了 |

---

## C# 版との対応

bizprint は Java 版と C# 版のサーバーライブラリを提供しています。

### クラス対応表

| Java | C# | 備考 |
|---|---|---|
| `PDFCommonPrintStream` | `PDFCommonPrintStream` | 同名。C# 版は `abstract class`（`OutputStream` 継承なし） |
| `PDFDirectPrintStream` | `PDFDirectPrintStream` | 同名。コンストラクタの引数型が異なる |
| `PDFBatchPrintStream` | - | C# 版には未実装 |
| `PDFBatchStatus` | - | C# 版には未実装 |
| `PDFBatchPrintStatus` | - | C# 版には未実装 |
| `BizPrintException` | - | C# 版には未実装 |

### API の主な差異

#### コンストラクタの引数型

| | Java | C# |
|---|---|---|
| `PDFDirectPrintStream` | `jakarta.servlet.http.HttpServletResponse` | `System.Web.HttpResponse` |

#### メソッド名の違い

| Java | C# | 説明 |
|---|---|---|
| `close()` | `Close()` | バッファクローズ |
| `flush()` | `Flush()` | バッファフラッシュ |
| `write(int)` | `WriteByte(byte)` | 1 バイト書き込み |
| `write(byte[])` | `Write(byte[])` | バイト配列書き込み |
| `write(byte[], int, int)` | `Write(byte[], int, int)` | 範囲指定書き込み |
| `size()` | `size()` | 書き込みバイト数取得（C# の戻り値は `long`） |

#### doFit のデフォルト値

| | Java | C# |
|---|---|---|
| デフォルト値 | `"false"`（文字列） | `true`（bool） |

> **注意**: Java 版と C# 版で `doFit` のデフォルト値が異なります。Java 版ではデフォルトで用紙に合わせた印刷が無効、C# 版では有効です。

#### setPassword の null チェック

| | Java | C# |
|---|---|---|
| `null` 指定時 | 何もしない（無視） | `ArgumentException` をスロー |

#### C# 版の使用例

```csharp
using com.brainsellers.bizstream.bizprint;

PDFDirectPrintStream direct = new PDFDirectPrintStream(Response);

direct.setPrinterName("Microsoft Print to PDF");
direct.setNumberOfCopy(1);
direct.setSelectedTray(PDFDirectPrintStream.TRAY_AUTO);
direct.setJobName("DirectPrintSample");
direct.setPrintDialog(false);
direct.setDoFit(true);
direct.setFromPage(1);
direct.setToPage(-1);

// PDF データの書き込み
byte[] pdfData = System.IO.File.ReadAllBytes(@"C:\temp\sample.pdf");
direct.Write(pdfData);

// spp ファイルを生成し HTTP レスポンスとして送信
direct.Close();
```

### C# 版にない機能

以下の機能は Java 版のみで提供されています:

- バッチ印刷（`PDFBatchPrintStream`）
- バッチ印刷ステータス照会（`PDFBatchStatus` / `PDFBatchPrintStatus`）
- カスタム例外（`BizPrintException`）

C# 版でバッチ印刷が必要な場合は Java 版を使用してください。
