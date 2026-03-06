---
name: check-review
description: PR のレビューコメントを取得・表示し、指摘事項への対応を支援する。
---

# check-review スキル

GitHub PR に投稿されたレビューコメントを取得・表示し、指摘事項への対応を支援する。

## 想定フロー

```
Session A: 実装 → /create-pr → ポーリング → レビュー検知 → /check-review 自動実行
Session B: /review-pr <PR番号> → レビューコメント投稿
```

`/create-pr` のポーリングで自動的に呼び出されることを想定しているが、
手動で `/check-review <PR番号>` として実行することもできる。

## 引数

```
/check-review <PR番号>
```

PR 番号が省略された場合はユーザーに確認する。

## 手順

### 1. レビューコメントの取得

```bash
PR_NUM=<PR番号>

# PR のコメント一覧を取得
gh pr view $PR_NUM --comments
```

### 2. 指摘事項の抽出・整理

取得したコメントから「Code Review」コメント（`/review-pr` が投稿したもの）を特定し、
指摘事項を重要度別に整理してユーザーに提示する:

- **MUST（修正必須）**: マージ前に対応が必要
- **SHOULD（修正推奨）**: 原則対応、理由があればスキップ可
- **NITS（軽微な改善提案）**: 任意
- **QUESTION（確認事項）**: 回答が必要

### 3. 指摘事項への対応

指摘事項を整理してユーザーに提示した後、以下のルールで対応する:

- **MUST / SHOULD / NITS**: 自動で修正に着手する（ユーザー確認不要）
- **QUESTION**: ユーザーに確認してから対応を決定する

修正時は通常の開発フロー（CheckStyle 確認等）に従う。

指摘なし（LGTM）の場合は、その旨をユーザーに伝えて終了する。

### 4. 対応完了後

修正が完了したら:

1. 修正内容のサマリーをユーザーに提示する
2. **コミット＆プッシュしてよいかユーザーに確認する**（自動実行しない）
3. ユーザーが承認したら、コミット・プッシュする
4. PR に対応完了のコメントを投稿する:

```bash
gh pr comment $PR_NUM --body "レビュー指摘事項に対応しました。再レビューをお願いします。"
```

### 5. 再レビューのポーリング

対応完了コメント投稿後、再レビューコメントをポーリングする。
最初のレビューコメントと区別するため、対応完了コメントの投稿時刻より後に
投稿された「Reviewed by Claude Code」コメントのみを検出する。

以下のコマンドをバックグラウンドで実行する（`run_in_background` を使用）:

```bash
PR_NUM=<PR番号>
AFTER_ISO=<対応完了コメント投稿時の ISO 8601 タイムスタンプ（例: 2026-03-06T12:00:00Z）>
INTERVAL=30
REPO=$(gh repo view --json nameWithOwner -q '.nameWithOwner')

while true; do
  FOUND=$(gh api "repos/${REPO}/issues/${PR_NUM}/comments?per_page=100&sort=created&direction=desc" 2>/dev/null \
    | python3 -c "
import sys, json
from datetime import datetime, timezone
after = datetime.fromisoformat('${AFTER_ISO}'.replace('Z', '+00:00'))
comments = json.load(sys.stdin)
for c in comments:
    created = datetime.fromisoformat(c['created_at'].replace('Z', '+00:00'))
    if created > after and 'Reviewed by Claude Code' in c.get('body', ''):
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

再レビューコメントを検知したら、ステップ1に戻って再度指摘事項を確認する。
LGTM であればその旨をユーザーに伝えて終了する。

**フォールバック**: TaskOutput で結果を取得できない場合は、GitHub API で直接取得する
（`/create-pr` スキルのフォールバック手順と同様）。

## 注意事項

- レビューコメントが存在しない場合は、その旨をユーザーに伝える。
- 対応中に新たな問題を発見した場合は、ユーザーに報告してから修正する。
