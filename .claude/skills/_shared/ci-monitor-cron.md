# CI 監視 Cron プロンプトテンプレート

CronCreate ツールに渡すプロンプト。`<ブランチ名>` と `<基準時刻の ISO 8601>` を実際の値に置き換えて使用する。

- `<基準時刻>` は create-pr では PR 作成時刻、check-review ではプッシュ時刻を使用する。

```
以下のコマンドで CI ワークフローの状態を確認してください:

gh run list --branch <ブランチ名> --limit 1 --json status,conclusion,name,createdAt 2>$null

レスポンスを確認し:
- レスポンスが空配列の場合:
  - 現在時刻が <基準時刻の ISO 8601> から 2 分以上経過している → 「CI ワークフローが見つかりません（CI 対象外の変更です）。」と報告してください。
  - 2 分未満 → 何も報告せず、次の実行を待ってください。
- レスポンスの最初の要素の createdAt が <基準時刻の ISO 8601> より前の場合 → 上記の空配列と同じ判定をしてください。
- レスポンスの最初の要素の createdAt が <基準時刻の ISO 8601> 以降の場合:
  - conclusion が "success" → 「CI が成功しました。」と報告してください。
  - conclusion が "failure" または "cancelled" → 「CI が失敗しました（conclusion: <値>）。確認してください。」と報告してください。
  - status が "in_progress" または "queued" → 何も報告せず、次の実行を待ってください。
  - 上記いずれにも該当しない場合 → 「CI の状態が不明です（status: <値>, conclusion: <値>）。確認してください。」と報告してください。
```
