---
name: create-pr
description: GitHub プルリクエストを作成する。ビルド成功・コミット・プッシュ完了後に使用。
---

# create-pr スキル

GitHub プルリクエストを作成するワークフロー。

## 前提条件

- ローカルビルドが成功していること（`/maven-build` で確認）。
- コミット・プッシュが完了していること。
- `docs/` 配下のドキュメントを新規作成・大幅更新した場合は、ソースコード照合レビューが完了していること（`.claude/rules/reference-docs.md` 参照）。

## 手順

### 1. PR テンプレートの確認

`.github/pull_request_template.md` を読み込み、テンプレート構造に準拠する。

### 2. PR 作成

```bash
gh pr create \
  --title "<イシュー番号に対応するタイトル>" \
  --body "$(cat <<'EOF'
## 概要

<コミットメッセージの要約を記載>

## 関連イシュー

Closes #<イシュー番号>

## チェックリスト

- [ ] 動作確認済み
- [ ] CI通過
- [ ] 関係者にレビュー依頼済み
EOF
)"
```

### 3. レビューの案内とポーリング

PR 作成が完了したら、ユーザーに以下を案内する:

> PR を作成しました。**別のターミナル**で以下を実行してレビューしてください:
>
> ```
> claude
> /review-pr <PR番号>
> ```
>
> このセッションでレビューコメントを監視しています。
> レビューが投稿されたら自動で指摘対応を開始します。

案内後、バックグラウンドでレビューコメントのポーリングを開始する。

#### ポーリング手順

以下のコマンドをバックグラウンドで実行する（`run_in_background` を使用）:

```bash
PR_NUM=<PR番号>
INTERVAL=30
REPO=$(gh repo view --json nameWithOwner -q '.nameWithOwner')

while true; do
  FOUND=$(gh api "repos/${REPO}/issues/${PR_NUM}/comments?per_page=100&sort=created&direction=desc" 2>/dev/null \
    | python3 -c "
import sys, json
comments = json.load(sys.stdin)
for c in comments:
    if 'Reviewed by Claude Code' in c.get('body', ''):
        print(c['body'])
        sys.exit(0)
sys.exit(1)
" 2>/dev/null)

  if [ $? -eq 0 ]; then
    echo "$FOUND"
    exit 0
  fi

  sleep "$INTERVAL"
done
```

#### レビューコメント検知後

ポーリングタスクの完了通知を受けたら、以下の順で結果を取得する:

1. TaskOutput でバックグラウンドタスクの出力を取得する
2. 取得できない場合（タスク消失等）は、GitHub API で直接取得する:

```bash
PR_NUM=<PR番号>
REPO=$(gh repo view --json nameWithOwner -q '.nameWithOwner')
gh api "repos/${REPO}/issues/${PR_NUM}/comments?per_page=100&sort=created&direction=desc" 2>/dev/null \
  | python3 -c "
import sys, json
comments = json.load(sys.stdin)
for c in comments:
    if 'Reviewed by Claude Code' in c.get('body', ''):
        print(c['body'])
        sys.exit(0)
"
```

レビューコメントを取得したら、`/check-review` スキルの手順に従って
指摘事項への対応を自動で開始する。

### 注意事項

- PR 作成前に必ずユーザーに確認する。
- テンプレートの構造を勝手に変更しない。
- PR 本文に `Closes #<イシュー番号>` を記述してイシューと連携する。
