---
globs:
  - ".claude/skills/**"
---

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

## PowerShell 必須

すべてのスキルは PowerShell を前提とする。

- フロントマターに `shell: powershell` を記述する。
- コードブロックは ` ```powershell ` で記述する（` ```bash ` は使わない）。
- Bash 固有の構文を使わない:
  - リダイレクト: `2>/dev/null` → `2>$null`
  - 行継続: `\` → `` ` ``（バッククォート）
  - 変数代入: `VAR=value` → `$VAR = "value"`
- Claude Code は pwsh 7+ で動作するため、pwsh 7+ 固有構文（`&&`, `||`, 三項演算子等）を使用してよい。
