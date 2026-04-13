# CI 監視 Monitor テンプレート

CI ワークフローの状態を Monitor ツールで監視する。
Monitor ツールを以下のパラメータで呼び出すこと。

- **command**:

> **Note:** Monitor ツールは bash で実行されるため、コマンドは bash で記述する。

```bash
SHA=$(git rev-parse HEAD)
NOT_FOUND=0
while true; do
  sleep 30
  RESULT=$(gh run list --commit "$SHA" --limit 1 \
    --json status,conclusion \
    --jq '.[0] // empty | "\(.status) \(.conclusion)"' 2>/dev/null) || {
    echo "WARNING: gh API error" >&2
    continue
  }

  if [ -z "$RESULT" ]; then
    NOT_FOUND=$((NOT_FOUND+1))
    if [ $NOT_FOUND -ge 5 ]; then
      echo "CI_RESULT: not_found"; exit 1
    fi
    continue
  fi

  CONCLUSION=$(echo "$RESULT" | awk '{print $2}')

  case "$CONCLUSION" in
    success|skipped)   echo "CI_RESULT: $CONCLUSION"; exit 0 ;;
    failure|cancelled) echo "CI_RESULT: $CONCLUSION"; exit 1 ;;
  esac
done
```

- **timeout_ms**: 監視対象に応じて指定する（bizprint の Build ワークフローは 1.6〜2.6 分のため `600000` / 10分で十分）
- **persistent**: `false`
- **description**: `CI ワークフロー監視`

監視対象のコミット SHA は Monitor 起動時の `HEAD` で自動取得する。呼び出し元で事前に目的のブランチを checkout（必要なら pull）しておくこと。

## ワークフローステータスの扱い

| GitHub Actions conclusion | 扱い | 備考 |
|---|---|---|
| `success` | 正常終了（exit 0） | CI 通過 |
| `skipped` | 正常終了（exit 0） | パス条件等で実行対象外（例: `.claude/` のみの変更） |
| `failure` / `cancelled` | エラー終了（exit 1） | 要対応 |
| `not_found` | 5 回連続で検知したら `not_found` としてエラー終了 | 該当 SHA のワークフローが作成されていない |
| `null`（status: `queued` / `in_progress` 等） | 監視継続 | conclusion が確定するまで待機 |

## CI 監視の検知後

Monitor が完了したら出力内容を確認する。

- **`CI_RESULT: success`** / **`CI_RESULT: skipped`** → ユーザーに報告する。
- **`CI_RESULT: failure`** / **`CI_RESULT: cancelled`** → ユーザーに確認する。修正・再プッシュした場合は Monitor を再作成する。
- **`CI_RESULT: not_found`** → 該当 SHA のワークフローが作成されていない旨をユーザーに報告する。
- **タイムアウト** → ユーザーに報告し、手動確認を依頼する。
