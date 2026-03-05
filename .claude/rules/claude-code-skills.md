# Claude Code スキル設定ルール

このプロジェクトで Claude Code のカスタムスキルを追加・変更する際のルール。

## ディレクトリ構成（必須）

```
.claude/skills/<skill-name>/SKILL.md
```

- **ディレクトリ形式のみ有効**。フラット形式（`.claude/skills/<skill-name>.md`）では Claude Code に認識されない。
- `SKILL.md` はファイル名を大文字にすること。
- `SKILL.md` の先頭に YAML フロントマター（`name`, `description`）が必要。

```yaml
---
name: skill-name
description: スキルの説明
---
```

## .gitignore との衝突に注意

`.gitignore` に登録されているパターンがスキルディレクトリ名にマッチする場合、
Git 管理から除外されてしまう。

**既知の例**: `build/` が `.gitignore` にマッチするため、`build` という名前のスキルは
リポジトリに追加できない。この場合は `maven-build` のように別名にすること。

スキルを追加したら `git status` で意図通り追跡されているか確認すること。

## 現在登録済みのスキル

| スキル名 | 用途 |
|---|---|
| `start-task` | イシュー・ブランチ作成（開発着手前に必須） |
| `maven-build` | Maven ビルド実行 |
| `create-pr` | GitHub PR 作成 |
| `review-pr` | GitHub PR のコードレビュー・コメント投稿 |
| `check-review` | PR レビューコメント取得・指摘対応支援 |
| `create-release` | GitHub リリース作成（タグ push でワークフロー起動） |
