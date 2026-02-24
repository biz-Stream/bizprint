# 設定リファレンス

bizprint クライアントの各サービスで使用する設定ファイルのパラメータ一覧です。

## 設定ファイルの配置場所

各設定ファイルは `C:\ProgramData\brainsellers\` 配下に配置されます。

| サービス | 設定ファイルパス |
|---|---|
| DirectPrintService | `C:\ProgramData\brainsellers\DirectPrint\DirectPrintService.xml` |
| BatchPrintService | `C:\ProgramData\brainsellers\BatchPrint\BatchPrintService.xml` |
| BizPrintHealthChecker (Direct用) | `C:\ProgramData\brainsellers\DirectPrint\BizPrintHealthChecker.xml` |
| BizPrintHealthChecker (Batch用) | `C:\ProgramData\brainsellers\BatchPrint\BizPrintHealthChecker.xml` |
| SilentPdfPrinter | `C:\ProgramData\brainsellers\DirectPrint\SilentPdfPrinter.xml` |

設定ファイルは XML 形式で、以下の構造を持ちます:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<properties>
    <section key="Application">
        <entry key="パラメータ名" type="string">値</entry>
    </section>
</properties>
```

---

## DirectPrintService.xml

ダイレクト印刷の設定ファイルです。DirectPrintService はオンデマンド起動型のプロセスで、SilentPdfPrinter により必要時に起動され、ブラウザからの印刷リクエストを受信して指定プリンタへ印刷を実行します。一定のアイドル時間が経過すると自動的にプロセスを停止します。

### 基本設定

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `port` | int | `3000` | HTTP リクエストを受信するポート番号 |
| `queueSize` | int | `100` | 印刷キューの最大保持数 |
| `spoolTimeOut` | int | `60000` | スプーラ監視タイムアウト (ミリ秒)。ロードリトライ時間が自動加算される |
| `sppPass` | string | (空) | spp ファイル解凍パスワード (Base64 エンコーディング済み)。ビルド時に自動設定される |
| `tmpFolderPath` | string | `C:\ProgramData\brainsellers\DirectPrint\tmp` | 一時ファイル作成フォルダパス。空の場合はデフォルトパスが使用される |
| `cleanup_tmpfolder` | bool | `true` | サービス開始・終了時に一時フォルダのクリーンアップを行うか |
| `responseTemplate` | string | `C:\ProgramData\brainsellers\DirectPrint\html\response.html` | ブラウザへ返すレスポンス HTML テンプレートファイルのパス |

### ブラウザ設定

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `browserset` | string | `iexplore=iexplore,firefox=firefox,chrome=chrome,browser_broker=Edge,RuntimeBroker=Edge,explorer=Edge,sihost=Edge,msedge=msedge` | 起動元ブラウザのプロセス名と返信先ブラウザのマッピング (カンマ区切り、`プロセス名=ブラウザ名` 形式) |
| `defaultBrowser` | string | `msedge` | デフォルトブラウザ識別子。`browserset` でマッチしない場合に使用 |

### 終了制御

DirectPrintService はオンデマンド起動型のため、一定のアイドル時間が経過するとプロセスを自動停止します。

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `exitTimerEnabled` | bool | `true` | 時間経過によるキュー待ちスレッドの自動終了を有効にするか |
| `exitTimerLimit` | int | `30` | キュー待ちスレッドの自動終了までの時間 (秒) |

### 印刷履歴

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `histMax` | int | `100` | 印刷履歴の最大保持件数 |
| `holdTime` | int | `86400` | 印刷履歴の最大保持時間 (秒)。デフォルトは 24 時間 |

### 印刷ダイアログ制御

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `printDlgName` | string | `印刷\|進行状況` | 監視対象の印刷ダイアログ名 (パイプ区切りで複数指定可) |
| `printDlgFindTimeOut` | int | `10000` | 印刷ダイアログを発見できない場合のタイムアウト (ミリ秒) |
| `printDlgStayingTimeCheck` | int | `60000` | 印刷ダイアログが表示されたまま一定時間経過した場合のログ出力までの時間 (ミリ秒) |
| `printDlgLeaveTimeOut` | int | `3` | 印刷ダイアログが閉じてからフォームが終了するまでのタイムアウト (秒)。最小値は 1 |

### 印刷フォーム・スレッド制御

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `printformbythread` | string | `auto` | 印刷フォームのスレッド実行モード。`true` / `false` / `auto` のいずれか |
| `printFormTimerInterval` | int | `100` | 印刷フォームのタイマーインターバル (ミリ秒)。最小値は 1 |
| `printProcessThreadWaitMsec` | int | `100` | 印刷スレッドのループ待機時間 (ミリ秒) |

### フォーム作成制御

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `formCreateTimeoutMsec` | int | `10000` | フォーム作成処理のタイムアウト検知時間 (ミリ秒) |
| `formCreateTimeoutCheckMsec` | int | `100` | フォーム作成処理のタイムアウトチェックループ間隔 (ミリ秒) |
| `formCreateRetryNum` | int | `5` | フォーム作成処理の最大リトライ回数 |
| `formCreateRetryWaitMsec` | int | `500` | フォーム作成処理のリトライ待機時間 (ミリ秒) |

### ファイルロード制御

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `loadRetryNum` | int | `5` | ファイルロード失敗時のリトライ回数 |
| `loadRetryWaitMsec` | int | `1000` | ファイルロード失敗時のリトライ待機時間 (ミリ秒) |

### CPU 負荷制御

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `cpucheckpercentage` | int | `70` | 印刷キューからの取り出しを一時停止する CPU 使用率の閾値 (%) |
| `cpucheckloopmax` | int | `0` | CPU 使用率チェックの最大ループ回数。0 の場合はチェックを行わない |

### プロセス Kill 制御

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `killProcessNames` | string | `AcroRd32,Acrobat,RdrCEF` | Kill 対象となるプロセス名 (カンマ区切り) |
| `killProcessPrintCount` | int | `0` | Kill を実行するまでの同一プリンタでの印刷回数。0 の場合は Kill を行わない |

### デフォルトプリンタ取得

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `defaultPrinterTimeout` | int | `10000` | デフォルトプリンタ取得のタイムアウト (ミリ秒)。最小値は 100 |
| `defaultPrinterCheckInterval` | int | `33` | デフォルトプリンタ取得のチェックループインターバル (ミリ秒)。`defaultPrinterTimeout` 以上の値は無効 |

---

## BatchPrintService.xml

バッチ印刷の設定ファイルです。BatchPrintService は常駐型の Windows フォームアプリケーションとして動作し、サーバーからの印刷リクエストを受信してバッチ処理で印刷を実行します。Acrobat Reader がデスクトップセッションを必要とするため、常にユーザーがログインしている必要があります。

### 基本設定

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `port` | int | `3000` | HTTP リクエストを受信するポート番号 |
| `queueSize` | int | `100` | 印刷キューの最大保持数 |
| `spoolTimeOut` | int | `60000` | スプーラ監視タイムアウト (ミリ秒)。ロードリトライ時間が自動加算される |
| `sppPass` | string | (空) | spp ファイル解凍パスワード (Base64 エンコーディング済み)。ビルド時に自動設定される |
| `tmpFolderPath` | string | `C:\ProgramData\brainsellers\BatchPrint\tmp` | 一時ファイル作成フォルダパス。空の場合はデフォルトパスが使用される |
| `cleanup_tmpfolder` | bool | `true` | サービス開始・終了時に一時フォルダのクリーンアップを行うか |

### 自動再起動設定

BatchPrintService 固有の設定です。長時間稼働時のメモリリーク対策として、プロセスの自動再起動機能を提供します。

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `restartflg` | bool | `true` | プロセスの自動再起動を有効にするか |
| `restartprintnum` | int | `128` | 自動再起動を実行するまでの印刷回数 |
| `restartmin` | int | `20` | キューが空になってから自動再起動するまでの待機時間 (分) |

### 印刷履歴

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `histMax` | int | `100` | 印刷履歴の最大保持件数 |
| `holdTime` | int | `86400` | 印刷履歴の最大保持時間 (秒)。デフォルトは 24 時間 |

### 印刷フォーム・スレッド制御

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `printformbythread` | string | `auto` | 印刷フォームのスレッド実行モード。`true` / `false` / `auto` のいずれか |
| `printFormTimerInterval` | int | `100` | 印刷フォームのタイマーインターバル (ミリ秒)。最小値は 1 |
| `printProcessThreadWaitMsec` | int | `100` | 印刷スレッドのループ待機時間 (ミリ秒) |

### フォーム作成制御

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `formCreateTimeoutMsec` | int | `10000` | フォーム作成処理のタイムアウト検知時間 (ミリ秒) |
| `formCreateTimeoutCheckMsec` | int | `100` | フォーム作成処理のタイムアウトチェックループ間隔 (ミリ秒) |
| `formCreateRetryNum` | int | `5` | フォーム作成処理の最大リトライ回数 |
| `formCreateRetryWaitMsec` | int | `500` | フォーム作成処理のリトライ待機時間 (ミリ秒) |

### ファイルロード制御

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `loadRetryNum` | int | `5` | ファイルロード失敗時のリトライ回数 |
| `loadRetryWaitMsec` | int | `1000` | ファイルロード失敗時のリトライ待機時間 (ミリ秒) |

### CPU 負荷制御

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `cpucheckpercentage` | int | `70` | 印刷キューからの取り出しを一時停止する CPU 使用率の閾値 (%) |
| `cpucheckloopmax` | int | `0` | CPU 使用率チェックの最大ループ回数。0 の場合はチェックを行わない |

### プロセス Kill 制御

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `killProcessNames` | string | `AcroRd32,Acrobat,RdrCEF` | Kill 対象となるプロセス名 (カンマ区切り) |
| `killProcessPrintCount` | int | `0` | Kill を実行するまでの同一プリンタでの印刷回数。0 の場合は Kill を行わない |

### デフォルトプリンタ取得

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `defaultPrinterTimeout` | int | `10000` | デフォルトプリンタ取得のタイムアウト (ミリ秒)。最小値は 100 |
| `defaultPrinterCheckInterval` | int | `33` | デフォルトプリンタ取得のチェックループインターバル (ミリ秒)。`defaultPrinterTimeout` 以上の値は無効 |

---

## BizPrintHealthChecker.xml

ヘルスチェッカーの設定ファイルです。DirectPrintService または BatchPrintService のプロセス生存監視を行い、異常時にプロセスの再起動を実行します。

設定ファイルは監視対象サービスと同じフォルダに配置されます:
- ダイレクト印刷監視時: `C:\ProgramData\brainsellers\DirectPrint\BizPrintHealthChecker.xml`
- バッチ印刷監視時: `C:\ProgramData\brainsellers\BatchPrint\BizPrintHealthChecker.xml`

ポート番号は監視対象の DirectPrintService.xml または BatchPrintService.xml から自動取得されます。

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `serverAddress` | string | `127.0.0.1` | 監視対象サービスの接続先アドレス |
| `connectTimeout` | int | `2000` | サービス接続時のタイムアウト (ミリ秒) |
| `connectRetryNum` | int | `5` | サービス接続失敗時のリトライ回数 |
| `connectRetryWaitMsec` | int | `200` | サービス接続失敗時のリトライ待機時間 (ミリ秒) |
| `killedCheckRetryNum` | int | `5` | プロセス強制終了後の終了確認リトライ回数 |

---

## SilentPdfPrinter.xml

SilentPdfPrinter の設定ファイルです。SPP ファイルのダウンロード時にファイル関連付けで起動され、DirectPrintService が未起動であれば起動した上で、SPP ファイルを DirectPrintService に転送します。

| パラメータ | 型 | デフォルト値 | 説明 |
|---|---|---|---|
| `port` | int | `3000` | DirectPrintService のポート番号 |
| `directprinthost` | string | `127.0.0.1` | DirectPrintService の動作するホスト名または IP アドレス |
| `processname` | string | `DirectPrintService` | 接続先サービスのプロセス名 |
| `maxprocessnum` | int | `10` | SilentPdfPrinter の最大並列起動数 |
| `concurrentconnectionsmax` | int | `5` | DirectPrintService への最大同時接続数。最小値は 1 |
| `deletefile` | bool | `true` | 印刷後に引数として指定された PDF ファイルを削除するか |
| `timeout` | int | `20000` | サービス接続時のタイムアウト (ミリ秒) |
| `retry` | int | `5` | サービス接続失敗時のリトライ回数 |
| `retryinterval` | int | `5000` | サービス接続失敗時のリトライ間隔 (ミリ秒) |
| `waitloopmsec` | int | `1000` | 多重起動時のチェック待機時間 (ミリ秒) |

---

## ログ設定

各サービスのログは log4net を使用して出力されます。ログ設定は専用の XML ファイルで管理されます。

### ログ設定ファイルの配置場所

| サービス | ログ設定ファイルパス |
|---|---|
| DirectPrintService | `C:\ProgramData\brainsellers\DirectPrint\DirectPrintService_logConfig.xml` |
| BatchPrintService | `C:\ProgramData\brainsellers\BatchPrint\BatchPrintService_logConfig.xml` |
| BizPrintHealthChecker (Direct用) | `C:\ProgramData\brainsellers\DirectPrint\BizPrintHealthChecker_logConfig.xml` |
| BizPrintHealthChecker (Batch用) | `C:\ProgramData\brainsellers\BatchPrint\BizPrintHealthChecker_logConfig.xml` |
| SilentPdfPrinter | `C:\ProgramData\brainsellers\DirectPrint\SilentPdfPrinter_logConfig.xml` |

### ログファイルの出力先

| サービス | ログファイルパス |
|---|---|
| DirectPrintService | `C:\ProgramData\brainsellers\DirectPrint\log\DirectPrintService.log` |
| BatchPrintService | `C:\ProgramData\brainsellers\BatchPrint\log\BatchPrintService.log` |
| BizPrintHealthChecker (Direct用) | `C:\ProgramData\brainsellers\DirectPrint\log\BizPrintHealthChecker.log` |
| BizPrintHealthChecker (Batch用) | `C:\ProgramData\brainsellers\BatchPrint\log\BizPrintHealthChecker.log` |
| SilentPdfPrinter | `C:\ProgramData\brainsellers\DirectPrint\log\SilentPdfPrinter.log` |

### ログ設定パラメータ

ログ設定ファイルは log4net の `RollingFileAppender` を使用しています。

| パラメータ | デフォルト値 | 説明 |
|---|---|---|
| `File` | (サービスごとに異なる) | ログファイルの出力パス |
| `MaximumFileSize` | `5MB` | ログファイル 1 つあたりの最大サイズ |
| `maxSizeRollBackups` | `7` | ローテーション時のバックアップファイル数 |
| `rollingStyle` | `Size` | ローテーション方式 (ファイルサイズベース) |
| `appendToFile` | `true` | 既存ファイルへの追記モード |
| `staticLogFileName` | `true` | ログファイル名を固定するか |
| `level` | `Info` | ログレベル (`Debug`, `Info`, `Warn`, `Error`, `Fatal`) |

### ログ出力フォーマット

```
%d %p - %m%n
```

- `%d`: 日時
- `%p`: ログレベル
- `%m`: メッセージ
- `%n`: 改行

出力例:
```
2024-01-15 10:30:45,123 INFO - DirectPrintService Start.
```

---

## タイマー・インターバル設定クイックリファレンス

全タイマー・インターバル設定を処理フェーズ順にまとめた横断的テーブルです。処理フローとの対応関係は[アーキテクチャ - 処理フェーズとタイマー設定](architecture.md#処理フェーズとタイマー設定ダイレクト印刷)を参照してください。

### フェーズ 1: 接続（SilentPdfPrinter → DirectPrintService）

ダイレクト印刷のみ。バッチ印刷ではサーバーから直接接続するため、クライアント側の設定はありません。

| 設定名 | 設定ファイル | デフォルト値 | 単位 | 説明 | 関連エラー |
|---|---|---|---|---|---|
| `timeout` | SilentPdfPrinter.xml | 20000 | ms | 接続タイムアウト | - |
| `retry` | SilentPdfPrinter.xml | 5 | 回 | 接続リトライ回数 | - |
| `retryinterval` | SilentPdfPrinter.xml | 5000 | ms | リトライ間隔 | - |
| `waitloopmsec` | SilentPdfPrinter.xml | 1000 | ms | 多重起動時のチェック待機時間 | - |

### フェーズ 2: キュー監視・フォーム作成

| 設定名 | 設定ファイル | デフォルト値 | 単位 | 説明 | 関連エラー |
|---|---|---|---|---|---|
| `printProcessThreadWaitMsec` | DirectPrintService.xml / BatchPrintService.xml | 100 | ms | 印刷スレッドのループ待機時間 | - |
| `defaultPrinterTimeout` | DirectPrintService.xml / BatchPrintService.xml | 10000 | ms | デフォルトプリンタ取得タイムアウト | 0107 |
| `defaultPrinterCheckInterval` | DirectPrintService.xml / BatchPrintService.xml | 33 | ms | デフォルトプリンタ取得のチェック間隔 | - |
| `formCreateTimeoutMsec` | DirectPrintService.xml / BatchPrintService.xml | 10000 | ms | フォーム作成のタイムアウト | 0201 |
| `formCreateTimeoutCheckMsec` | DirectPrintService.xml / BatchPrintService.xml | 100 | ms | フォーム作成のタイムアウトチェック間隔 | - |
| `formCreateRetryNum` | DirectPrintService.xml / BatchPrintService.xml | 5 | 回 | フォーム作成の最大リトライ回数 | 0201 |
| `formCreateRetryWaitMsec` | DirectPrintService.xml / BatchPrintService.xml | 500 | ms | フォーム作成のリトライ待機時間 | - |

### フェーズ 3: PDF ロード・印刷実行・スプーラー監視

| 設定名 | 設定ファイル | デフォルト値 | 単位 | 説明 | 関連エラー |
|---|---|---|---|---|---|
| `loadRetryNum` | DirectPrintService.xml / BatchPrintService.xml | 5 | 回 | PDF ロードのリトライ回数 | 0202 |
| `loadRetryWaitMsec` | DirectPrintService.xml / BatchPrintService.xml | 1000 | ms | PDF ロードのリトライ待機時間 | - |
| `printFormTimerInterval` | DirectPrintService.xml / BatchPrintService.xml | 100 | ms | スプール状態チェックのタイマー間隔 | - |
| `spoolTimeOut` | DirectPrintService.xml / BatchPrintService.xml | 60000 | ms | スプーラー監視タイムアウト（設定値） | 0405, 0410 |

> **`spoolTimeOut` の実効値に関する注記**
>
> `spoolTimeOut` の設定値には `loadRetryNum * loadRetryWaitMsec` が自動的に加算されます。
> さらに、タイムアウト判定ではこの実効値の **2 倍**の時間が使用されます。
>
> ```
> 実効値 = spoolTimeOut + (loadRetryNum * loadRetryWaitMsec)
> タイムアウト判定 = 実効値 * 2
> ```
>
> デフォルト値での計算:
> - 実効値: 60000 + (5 * 1000) = 65000ms
> - タイムアウト判定: 65000 * 2 = **130000ms (130秒)**

### フェーズ 4: 印刷ダイアログ監視（ダイレクト印刷 + printDialog=true のみ）

| 設定名 | 設定ファイル | デフォルト値 | 単位 | 説明 | 関連エラー |
|---|---|---|---|---|---|
| `printDlgName` | DirectPrintService.xml | `印刷\|進行状況` | - | 監視対象ダイアログのウィンドウ名 | 0401, 0402 |
| `printDlgFindTimeOut` | DirectPrintService.xml | 10000 | ms | ダイアログ検知タイムアウト | 0401, 0402 |
| `printDlgStayingTimeCheck` | DirectPrintService.xml | 60000 | ms | ダイアログ長時間表示の警告ログまでの時間 | - |
| `printDlgLeaveTimeOut` | DirectPrintService.xml | 3 | 秒 | ダイアログ消失後のフォーム終了待機 | - |

### フェーズ 5: サービス自動停止 / 自動再起動

**ダイレクト印刷（自動停止）:**

| 設定名 | 設定ファイル | デフォルト値 | 単位 | 説明 | 関連エラー |
|---|---|---|---|---|---|
| `exitTimerEnabled` | DirectPrintService.xml | true | - | 自動停止を有効にするか | - |
| `exitTimerLimit` | DirectPrintService.xml | 30 | 秒 | 自動停止までのアイドル時間 | - |

**バッチ印刷（自動再起動）:**

| 設定名 | 設定ファイル | デフォルト値 | 単位 | 説明 | 関連エラー |
|---|---|---|---|---|---|
| `restartflg` | BatchPrintService.xml | true | - | 自動再起動を有効にするか | - |
| `restartprintnum` | BatchPrintService.xml | 128 | 回 | 再起動までの印刷回数 | - |
| `restartmin` | BatchPrintService.xml | 20 | 分 | キュー空後の再起動待機時間 | - |

### ヘルスチェッカー

| 設定名 | 設定ファイル | デフォルト値 | 単位 | 説明 | 関連エラー |
|---|---|---|---|---|---|
| `connectTimeout` | BizPrintHealthChecker.xml | 2000 | ms | サービス接続タイムアウト | - |
| `connectRetryNum` | BizPrintHealthChecker.xml | 5 | 回 | 接続リトライ回数 | - |
| `connectRetryWaitMsec` | BizPrintHealthChecker.xml | 200 | ms | リトライ待機時間 | - |
