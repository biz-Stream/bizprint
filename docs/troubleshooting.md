# トラブルシューティング

bizprint の利用時に発生する一般的な問題と解決策をまとめています。

## 接続の問題

### クライアントに接続できない

**症状**: サーバーからクライアントの `/doprint` エンドポイントに接続できない。

**考えられる原因と対処法**:

1. **ポート競合**: DirectPrintService / BatchPrintService のデフォルトポートは `3000` です。他のアプリケーションが同じポートを使用していないか確認してください。
   ```
   netstat -ano | findstr :3000
   ```
   ポートを変更する場合は、設定ファイルの `port` パラメータを変更します。
   - ダイレクト印刷: `C:\ProgramData\brainsellers\DirectPrint\DirectPrintService.xml`
   - バッチ印刷: `C:\ProgramData\brainsellers\BatchPrint\BatchPrintService.xml`

2. **ファイアウォール**: Windows ファイアウォールで該当ポートがブロックされている可能性があります。DirectPrintService / BatchPrintService のポートを許可してください。

3. **プロセス未起動**:
   - **DirectPrintService**: SilentPdfPrinter が起動していない、または SilentPdfPrinter が DirectPrintService を起動できていない可能性があります。SPP ファイルのファイル関連付けが正しく設定されているか確認してください。
   - **BatchPrintService**: 常駐型の Windows フォームアプリケーションのため、ユーザーがログインしている必要があります。タスクマネージャーで BatchPrintService プロセスが動作しているか確認してください。

### ヘルスチェッカーが異常を検知する

**症状**: BizPrintHealthChecker がプロセスを異常と判断し、再起動を繰り返す。

**考えられる原因と対処法**:

1. **タイムアウト値が短すぎる**: `BizPrintHealthChecker.xml` の `connectTimeout`（デフォルト: 2000ms）や `connectRetryNum`（デフォルト: 5回）を調整してください。
2. **印刷処理が長時間実行中**: 大量の印刷ジョブ処理中はキュー処理チェックが停滞し、`/isalive` が `500 NG` を返す場合があります。印刷ダイアログ表示中は正常と判定されます。

---

## 暗号化キーの不一致

### SPP ファイルを解凍できない（エラーコード 0102）

**症状**: `sppファイルを解凍できない。` というエラーが発生する。

**原因**: サーバー側とクライアント側の暗号化キー（`RANDOM_STRING_1` / `RANDOM_STRING_2`）が一致していません。

**対処法**:

1. **必ず一括ビルドする**: 暗号化キーはビルドのたびにランダム生成されます。サーバー側とクライアント側で同じキーが必要なため、一部モジュールのみのビルドは不可です。
   ```bash
   mvn clean install
   ```

2. **固定キーを使用する**: `pom.xml` の `<properties>` セクションで `randomString1` / `randomString2` を明示的に指定することで、ビルドごとのキー変更を防止できます。

3. **デプロイの確認**: ビルド後、サーバー側のライブラリとクライアント側のインストーラーが同一ビルドのものであることを確認してください。

---

## 印刷の問題

### 印刷キューが満杯（エラーコード 0114）

**症状**: `印刷キュー上限を超えたため、印刷要求を破棄した。` というエラーが発生する。

**対処法**:

1. 設定ファイルの `queueSize`（デフォルト: 100）を増やす。
2. 大量の印刷ジョブが滞留している場合は、プリンタの状態を確認する。
3. `spoolTimeOut`（デフォルト: 60000ms）を調整して、タイムアウトした印刷ジョブがキューから除去されるようにする。

### 印刷タイムアウト（エラーコード 0410）

**症状**: `印刷受付タイムアウト。` というエラーが発生する。

**対処法**:

1. `spoolTimeOut` の値を延長する（ネットワークプリンタの場合、応答が遅いことがある）。
2. プリンタの接続状態・電源を確認する。
3. 印刷スプーラサービスが正常に動作しているか確認する。
   ```
   sc query Spooler
   ```

### 複数部数の印刷でタイムアウト（エラーコード 0405）

**症状**: `複数部数の印刷がタイムアウトまでにスプールし終わらない。` というエラーが発生する。

**対処法**:

1. `spoolTimeOut` の値を延長する。
2. 印刷部数を減らしてテストする。
3. プリンタの処理能力を確認する。

---

## プリンタ設定の問題

### プリンタ名が見つからない（エラーコード 0106）

**症状**: `プリンタ名の指定が間違っている。` というエラーが発生する。

**対処法**:

1. クライアント PC にインストールされているプリンタ名を確認する。
   ```
   wmic printer get name
   ```
2. API で指定するプリンタ名がクライアント PC のプリンタ名と完全一致しているか確認する（大文字・小文字、スペースに注意）。
3. プリンタ名を省略した場合はデフォルトプリンタが使用されます。デフォルトプリンタが設定されていない場合はエラーコード 0107 が発生します。

### トレイ指定エラー（エラーコード 0109）

**症状**: `トレイの指定が間違っている。` というエラーが発生する。

**対処法**:

有効なトレイ名を指定してください。

| トレイ名 | 説明 |
|---|---|
| `FIRST` / `UPPER` / `ONLYONE` | 上段トレイ |
| `LOWER` | 下段トレイ |
| `MIDDLE` | 中段トレイ |
| `MANUAL` | 手差しトレイ |
| `ENVELOPE` | 封筒トレイ |
| `AUTO` | 自動選択 |

### トレイ指定時の権限不足（エラーコード 0411）

**症状**: `トレイ指定時のプリンタアクセス権限不足。` というエラーが発生する。

**対処法**:

1. DirectPrintService / BatchPrintService を起動しているユーザーにプリンタへのアクセス権限があるか確認する。
2. ログインユーザーにプリンタの管理権限があるか確認する。

### プリンタの状態異常

| エラーコード | 症状 | 対処法 |
|---|---|---|
| 0406 | オフライン | プリンタの電源・接続を確認し、オンラインにする |
| 0407 | 用紙切れ | プリンタに用紙を補充する |
| 0408 | 不正なドライバ | プリンタドライバを再インストールする |
| 0409 | プリンタ不良 | プリンタの状態を確認し、修理・交換する |

---

## ダイレクト印刷固有の問題

### ブラウザが認識されない

**症状**: ブラウザからの印刷リクエストが正常に処理されない。印刷結果がブラウザに返されない。

**対処法**:

1. `DirectPrintService.xml` の `browserset` パラメータを確認する。デフォルト値:
   ```
   iexplore=iexplore,firefox=firefox,chrome=chrome,browser_broker=Edge,RuntimeBroker=Edge,explorer=Edge,sihost=Edge,msedge=msedge
   ```
2. 使用しているブラウザのプロセス名が `browserset` に含まれていない場合は追加する（`プロセス名=ブラウザ名` 形式）。
3. `defaultBrowser` パラメータ（デフォルト: `msedge`）を使用環境に合わせて変更する。

### 印刷ダイアログが閉じない（エラーコード 0401, 0402）

**症状**: 印刷ダイアログの表示待ちでタイムアウトする、または印刷ダイアログを検知できない。

**対処法**:

1. `printDlgFindTimeOut`（デフォルト: 10000ms）を延長する。
2. `printDlgName` に監視対象のダイアログ名が正しく設定されているか確認する（デフォルト: `印刷|進行状況`）。
3. 印刷ダイアログが不要な場合は、API 側で `printDialog` パラメータを `false` に設定する。

---

## バッチ印刷固有の問題

### サービスが頻繁に再起動する

**症状**: BatchPrintService が予期しないタイミングで再起動する。

**原因**: BatchPrintService には長時間稼働時のメモリリーク対策として、プロセスの自動再起動機能があります。

**対処法**:

1. 自動再起動の設定を確認する:
   - `restartflg`: 自動再起動の有効/無効（デフォルト: `true`）
   - `restartprintnum`: 再起動までの印刷回数（デフォルト: `128`）
   - `restartmin`: キュー空き後の再起動待機時間（デフォルト: `20` 分）
2. 自動再起動を無効にする場合は `restartflg` を `false` に設定する。なお、BatchPrintService は常駐型の Windows フォームアプリケーションのため、再起動後もユーザーがログインしている必要があります。
3. 再起動頻度を下げる場合は `restartprintnum` の値を増やす。

### ステータス照会でジョブが見つからない（エラーコード 0506, 0507）

**症状**: `/getstatus` で `JobIDに対応する印刷履歴なし。` または `印刷履歴なし。` が返される。

**対処法**:

1. 正しい jobID を指定しているか確認する（`/doprint` のレスポンスで返された jobID）。
2. 印刷履歴の保持件数 `histMax`（デフォルト: 100）と保持時間 `holdTime`（デフォルト: 86400秒 = 24時間）を確認する。古い履歴は自動削除されます。
3. プロセスが再起動した場合、印刷履歴はクリアされます。

---

## Acrobat Reader 関連の問題

### Acrobat Reader のエラー（エラーコード 0201〜0204）

**症状**: PDF の読み込みや印刷実行で Acrobat Reader 関連のエラーが発生する。

**対処法**:

1. Acrobat Reader がクライアント PC にインストールされているか確認する。
2. Acrobat Reader のバージョンが対応しているか確認する。
3. Acrobat Reader のプロセスが残留していないか確認する。`killProcessNames` パラメータ（デフォルト: `AcroRd32,Acrobat,RdrCEF`）で設定されたプロセスは、`killProcessPrintCount` の回数後に自動 Kill されます。
4. 手動でプロセスを終了する場合:
   ```
   taskkill /IM AcroRd32.exe /F
   taskkill /IM Acrobat.exe /F
   ```

---

## ログの確認方法

問題の調査にはログファイルが有用です。

### ログファイルの場所

| サービス | ログファイルパス |
|---|---|
| DirectPrintService | `C:\ProgramData\brainsellers\DirectPrint\log\DirectPrintService.log` |
| BatchPrintService | `C:\ProgramData\brainsellers\BatchPrint\log\BatchPrintService.log` |
| BizPrintHealthChecker (Direct用) | `C:\ProgramData\brainsellers\DirectPrint\log\BizPrintHealthChecker.log` |
| BizPrintHealthChecker (Batch用) | `C:\ProgramData\brainsellers\BatchPrint\log\BizPrintHealthChecker.log` |
| SilentPdfPrinter | `C:\ProgramData\brainsellers\DirectPrint\log\SilentPdfPrinter.log` |

### ログレベルの変更

詳細な情報が必要な場合は、ログ設定ファイルでログレベルを `Debug` に変更します。

ログ設定ファイルの場所:
- `C:\ProgramData\brainsellers\DirectPrint\DirectPrintService_logConfig.xml`
- `C:\ProgramData\brainsellers\BatchPrint\BatchPrintService_logConfig.xml`

設定ファイル内の `level` 要素を変更します:

```xml
<level value="Debug" />
```

変更後はプロセスの再起動が必要です。

### ログの見方

ログは以下のフォーマットで出力されます:

```
日時 ログレベル - メッセージ
```

例:
```
2024-01-15 10:30:45,123 INFO - DirectPrintService Start.
2024-01-15 10:30:46,456 ERROR - Error Code:0106 プリンタ名の指定が間違っている。
```

エラー発生時は `ERROR` レベルのログを検索して、エラーコードとメッセージを確認してください。エラーコードの詳細は [エラーコード一覧](error-codes.md) を参照してください。

---

## 関連ドキュメント

- [エラーコード一覧](error-codes.md) - 全エラーコードの詳細
- [設定リファレンス](configuration.md) - 全設定パラメータの説明
- [アーキテクチャ](architecture.md) - システム全体像と通信プロトコル
