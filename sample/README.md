# bizprint サンプルプログラム

bizprint を使って既存の PDF ファイルを印刷するサンプルプログラムです。

## 前提条件

- JDK 8 以上
- ビルド済みの bizprint JAR ファイル（`bizprint-server-java-X.X.X.jar` および `zip4j-X.X.X.jar`）
- 印刷先の PC に bizprint クライアント（ダイレクト印刷 or バッチ印刷）がインストール済みであること

JAR ファイルはプロジェクトルートで `mvn clean install` を実行すると、
`bizprint-server-java/target/` 配下に生成されます。

## サンプル一覧

| ファイル | 種別 | 説明 |
|---|---|---|
| [direct_print/DirectPrintSample.java](direct_print/DirectPrintSample.java) | ダイレクト印刷 | PDF ファイルをダイレクト印刷するサーブレット |
| [batch_print/BatchPrintSample.java](batch_print/BatchPrintSample.java) | バッチ印刷 | PDF ファイルをバッチ印刷するスタンドアロンプログラム（ステータス監視付き） |
| [batch_print/BatchStatusSample.java](batch_print/BatchStatusSample.java) | バッチ印刷 | バッチ印刷のステータスを照会するスタンドアロンプログラム |

## ダイレクト印刷サンプル

### 概要

`DirectPrintSample.java` は、サーブレットとして動作するダイレクト印刷のサンプルです。
Web ブラウザからアクセスすると、サーバ上の PDF ファイルを読み込んで暗号化し、
クライアント PC のダイレクト印刷クライアントが受信して印刷を実行します。

### コンパイル

```
javac -cp bizprint-server-java-X.X.X.jar;servlet-api.jar DirectPrintSample.java
```

※サーブレットコンテナの `servlet-api.jar`（Jakarta Servlet API）をクラスパスに追加してください。

### 使い方

1. `PDF_FILE_PATH` と `PRINTER_NAME` を環境に合わせて変更します。
2. コンパイル後、WAR ファイルに含めてサーブレットコンテナ（Tomcat 等）にデプロイします。
3. クライアント PC でダイレクト印刷クライアントを起動した状態で、ブラウザからサーブレットにアクセスします。

## バッチ印刷サンプル

### 概要

`BatchPrintSample.java` は、スタンドアロンで動作するバッチ印刷のサンプルです。
PDF ファイルを読み込んでバッチ印刷サーバへ送信し、印刷完了までステータスをポーリングします。

### コンパイル

```
javac -cp bizprint-server-java-X.X.X.jar BatchPrintSample.java
```

### 実行

```
java -cp .;bizprint-server-java-X.X.X.jar;zip4j-X.X.X.jar BatchPrintSample
```

※Linux/Mac の場合はクラスパスの区切り文字を `;` から `:` に変更してください。

### 使い方

1. `BATCH_SERVER`、`PORT_NO`、`PRINTER_NAME`、`PDF_FILE_PATH` を環境に合わせて変更します。
2. バッチ印刷クライアントが起動していることを確認します。
3. 上記コマンドで実行すると、印刷要求を送信し、ステータスを監視して結果を表示します。

## バッチ印刷ステータス照会サンプル

### 概要

`BatchStatusSample.java` は、バッチ印刷サーバに対して印刷状態を照会するサンプルです。

### コンパイル

```
javac -cp bizprint-server-java-X.X.X.jar BatchStatusSample.java
```

### 実行

特定ジョブの照会:
```
java -cp .;bizprint-server-java-X.X.X.jar;zip4j-X.X.X.jar BatchStatusSample [jobId]
```

全ジョブの照会:
```
java -cp .;bizprint-server-java-X.X.X.jar;zip4j-X.X.X.jar BatchStatusSample
```

## 設定のカスタマイズ

各サンプルの先頭にある定数を環境に合わせて変更してください。

| 定数名 | 説明 | 設定例 |
|---|---|---|
| `PDF_FILE_PATH` | 印刷する PDF ファイルのパス | `C:/temp/sample.pdf` |
| `PRINTER_NAME` | 出力先プリンタ名 | `Microsoft Print to PDF` |
| `BATCH_SERVER` | バッチ印刷サーバのホスト名 | `localhost` |
| `PORT_NO` | バッチ印刷サーバのポート番号 | `3000` |
| `STATUS_INTERVAL` | ステータスポーリング間隔（ミリ秒） | `3000` |

## ステータスコード一覧

バッチ印刷で返されるステータスコードの意味は以下の通りです。

| コード | 状態 | 説明 |
|---|---|---|
| 0002 | 印刷指示受付 | キュー内で印刷待ちの状態 |
| 0004 | 印刷中 | PDF のロードからプリントスプーラへの送信中 |
| 0006 | 印刷要求送信完了 | プリントスプーラへの送信が正常に完了 |
| 0008 | 印刷異常終了 | 印刷処理中にエラーが発生 |
