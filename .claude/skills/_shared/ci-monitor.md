# CI 監視 Monitor テンプレート

CI ワークフローの状態を監視する Monitor を作成する。
Monitor ツールを以下のパラメータで呼び出すこと。

- **description**: `CI ワークフロー監視 (<ブランチ名>)`
- **timeout_ms**: `600000`（10分）
- **persistent**: `false`
- **command**:

```bash
START=$(date -u +%Y-%m-%dT%H:%M:%SZ)
START_EPOCH=$(date -u +%s)
BRANCH="<ブランチ名>"

while true; do
  sleep 30
  RESULT=$(gh run list --branch "$BRANCH" --limit 1 \
    --json status,conclusion,createdAt \
    --jq '.[0] // empty | "\(.createdAt) \(.status) \(.conclusion)"' 2>/dev/null) || {
    echo "WARNING: gh API error" >&2
    continue
  }

  # ワークフローが存在しない場合
  if [ -z "$RESULT" ]; then
    echo "CI NOT_FOUND: ワークフローが見つかりません（CI 対象外の変更です）。"
    exit 1
  fi

  CREATED=$(echo "$RESULT" | awk '{print $1}')
  CONCLUSION=$(echo "$RESULT" | awk '{print $3}')

  # Monitor 起動前のランは無視（2分経過で NOT_FOUND）
  if [ "$CREATED" \< "$START" ]; then
    ELAPSED=$(( $(date -u +%s) - START_EPOCH ))
    if [ "$ELAPSED" -ge 120 ]; then
      echo "CI NOT_FOUND: ワークフローが見つかりません（CI 対象外の変更です）。"
      exit 1
    fi
    continue
  fi

  # terminal state の判定
  case "$CONCLUSION" in
    success)   echo "CI SUCCESS: ワークフローが成功しました。"; exit 0 ;;
    failure)   echo "CI FAILED: ワークフローが失敗しました (conclusion: failure)。確認してください。"; exit 1 ;;
    cancelled) echo "CI FAILED: ワークフローが失敗しました (conclusion: cancelled)。確認してください。"; exit 1 ;;
  esac
done
```

`<ブランチ名>` は `git branch --show-current` で取得した値に置き換えること。

## 動作仕様

- Monitor 起動時の時刻を自動記録し、それ以降に作成されたランのみを対象にする（古いランの誤検知を防止）
- 30秒間隔でポーリング（LLMターン不要）
- terminal state（success / failure / cancelled）を検知すると1行出力して自動終了
- ワークフローが見つからない場合も1行出力して終了
- 起動から2分経過しても新しいランが見つからない場合は NOT_FOUND で終了（CI 対象外の変更への早期フィードバック）
- API エラー時はリトライ（出力なし）
- タイムアウト10分

## CI 監視の検知後

Monitor からの通知を受け取ったら:

- **CI SUCCESS** → ユーザーに報告する。
- **CI FAILED** → ユーザーに確認する。修正・再プッシュした場合は CI 監視 Monitor を再作成する。
- **CI NOT_FOUND** → ユーザーに報告する。
