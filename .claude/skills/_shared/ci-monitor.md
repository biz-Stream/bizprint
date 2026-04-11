# CI 監視 Monitor テンプレート

CI ワークフローの状態を監視する Monitor を作成する。
Monitor ツールを以下のパラメータで呼び出すこと。

- **description**: `CI ワークフロー監視 (<ブランチ名>)`
- **timeout_ms**: `600000`（10分）
- **persistent**: `false`
- **command**:

```
BRANCH=<ブランチ名>; while true; do sleep 30; STATUS=$(gh run list --branch "$BRANCH" --limit 1 --json status,conclusion --jq ".[0] | if .conclusion == \"success\" then \"success\" elif .conclusion == \"failure\" then \"failure\" elif .conclusion == \"cancelled\" then \"cancelled\" elif .status == \"in_progress\" or .status == \"queued\" then \"in_progress\" else \"unknown\" end" 2>/dev/null || echo "api_error"); case $STATUS in success) echo "CI SUCCESS: ワークフローが成功しました。"; exit 0;; failure|cancelled) echo "CI FAILED: ワークフローが失敗しました (status: $STATUS)。確認してください。"; exit 1;; "") echo "CI NOT_FOUND: ワークフローが見つかりません（CI 対象外の変更です）。"; exit 1;; api_error) ;; esac; done
```

`<ブランチ名>` は `git branch --show-current` で取得した値に置き換えること。

## 動作仕様

- 30秒間隔でポーリング（LLMターン不要）
- terminal state（success / failure / cancelled）を検知すると1行出力して自動終了
- ワークフローが見つからない場合も1行出力して終了
- API エラー時はリトライ（出力なし）
- タイムアウト10分

## CI 監視の検知後

Monitor からの通知を受け取ったら:

- **CI SUCCESS** → ユーザーに報告する。
- **CI FAILED** → ユーザーに確認する。修正・再プッシュした場合は CI 監視 Monitor を再作成する。
- **CI NOT_FOUND** → ユーザーに報告する。
