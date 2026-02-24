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

## タイマー・インターバル設定の調整ガイド

タイマー・インターバル設定に起因する問題が発生した場合に、どのパラメータを調整すべきかをまとめています。処理フローと各タイマー設定の関係は[アーキテクチャ - 処理フェーズとタイマー設定](architecture.md#処理フェーズとタイマー設定ダイレクト印刷)を参照してください。

### 症状→調整パラメータ対応表

| 症状 | エラーコード | 調整パラメータ | 設定ファイル | 処理フェーズ |
|---|---|---|---|---|
| 白紙が印刷される | - | `spoolTimeOut` | DirectPrintService.xml / BatchPrintService.xml | フェーズ 3 |
| Microsoft Print to PDF でタイムアウト | 0401 / 0402 | `printDlgFindTimeOut`, `printDlgStayingTimeCheck` | DirectPrintService.xml | フェーズ 4 |
| 印刷タイムアウト | 0410 | `spoolTimeOut` | DirectPrintService.xml / BatchPrintService.xml | フェーズ 3 |
| 複数部数でタイムアウト | 0405 | `spoolTimeOut` | DirectPrintService.xml / BatchPrintService.xml | フェーズ 3 |
| 印刷ダイアログ検知失敗 | 0401 / 0402 | `printDlgFindTimeOut`, `printDlgName` | DirectPrintService.xml | フェーズ 4 |
| Acrobat Reader エラー（フォーム作成） | 0201 | `formCreateTimeoutMsec`, `formCreateRetryNum` | DirectPrintService.xml / BatchPrintService.xml | フェーズ 2 |
| PDF ロードエラー | 0202 | `loadRetryNum`, `loadRetryWaitMsec` | DirectPrintService.xml / BatchPrintService.xml | フェーズ 3 |
| ヘルスチェッカーが誤検知 | - | `connectTimeout`, `connectRetryNum` | BizPrintHealthChecker.xml | - |
| DirectPrintService がすぐ停止する | - | `exitTimerLimit` | DirectPrintService.xml | フェーズ 5 |

### シナリオ解説

#### 白紙印刷のメカニズムと対策

**現象**: 印刷は実行されるが、白紙が出力される。エラーは記録されない場合がある。

**原因メカニズム**:

1. PrintForm が Acrobat Reader (ActiveX) に印刷命令（`printAll` / `printPages`）を送信する
2. Acrobat Reader が PDF データを Windows スプーラーに送信する（非同期処理）
3. PrintForm は PrintRequestMonitor でスプーラーへのジョブ登録を監視する
4. **スプーラーへの送信が完了する前にタイムアウトすると**、PrintForm がクローズされ Acrobat Reader の ActiveX コントロールが破棄される
5. ActiveX 破棄により PDF データの送信が中断され、不完全なデータがスプーラーに残る
6. スプーラーは不完全なデータをプリンタに送信し、白紙が出力される

**対策**:

`spoolTimeOut` を延長します。ただし、実効値の計算に注意が必要です。

```
実効値 = spoolTimeOut設定値 + (loadRetryNum * loadRetryWaitMsec)
タイムアウト判定 = 実効値 * 2
```

デフォルト値の場合:
- 実効値: 60000 + (5 * 1000) = 65000ms
- タイムアウト判定: 65000 * 2 = 130000ms（130 秒）

大きな PDF やネットワークプリンタなど、スプーラーへの送信に時間がかかる環境では `spoolTimeOut` を 120000（120 秒）以上に設定することを推奨します。

#### Microsoft Print to PDF のダイアログタイムアウトのメカニズムと対策

**現象**: Microsoft Print to PDF をプリンタに指定して `printDialog=true` で印刷した場合、保存ダイアログで操作中にタイムアウトエラー（0401 または 0402）が発生する。

**原因メカニズム**:

1. `printDialog=true` の場合、Acrobat Reader は印刷ダイアログを表示する
2. Microsoft Print to PDF は印刷ダイアログの後に**ファイル保存ダイアログ**を表示する
3. PrintForm はウィンドウ名（`printDlgName` で設定）に一致するダイアログの出現と消失を監視する
4. 印刷ダイアログが閉じた時点で PrintForm は「ダイアログ操作完了」と判断する
5. しかし保存ダイアログが表示されている間、スプーラーへのジョブ送信は行われない
6. `printDlgFindTimeOut` 以内に保存ダイアログの操作を完了しないと、次の印刷ダイアログ検知でタイムアウトする

**対策**:

1. `printDlgFindTimeOut` を延長する（例: 60000ms 以上）。ユーザーが保存先を選択する時間を十分に確保します。
2. `printDlgStayingTimeCheck` を延長する（例: 120000ms 以上）。ダイアログ操作中の警告ログの出力タイミングを遅らせます。
3. Microsoft Print to PDF を使用する場合は `printDialog=false` に設定し、`saveFileName` パラメータで保存先を事前指定することを推奨します。

---

## 関連ドキュメント

- [エラーコード一覧](error-codes.md) - 全エラーコードの詳細
- [設定リファレンス](configuration.md) - 全設定パラメータの説明
- [アーキテクチャ](architecture.md) - システム全体像と通信プロトコル
