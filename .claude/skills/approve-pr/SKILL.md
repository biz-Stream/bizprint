---
name: approve-pr
description: PRを承認・マージし、マージ完了を確認してブランチを戻す。「承認＆マージ」「approve」等のキーワードで使用する。
shell: powershell
---

# approve-pr

## Purpose
PRの承認・マージ・マージ完了確認・ブランチ戻しを一貫して実行する。
レビュースキル（`/create-pr`、`/check-review`）とは分離されている。承認権限を持つユーザーが使用する想定。

## Usage
- `/approve-pr <PR番号>` — 指定PRを承認・マージする
- ユーザーが「承認＆マージして」等と発言した場合にも使用する

## 手順

### 1. 前提条件の確認

PR情報とCI状態を確認する。

```powershell
gh pr view <PR番号>
```

以下を確認する:
- **LGTM であること** — pr-reviewer のレビュー結果に MUST 指摘がないこと。会話履歴またはPRコメントから判断する。
- **CI が通っていること** — PR情報の checks status が pass であること。pass でない場合は中止してユーザーに報告する。

前提条件を満たさない場合はユーザーに報告し、スキルを中止する。

### 2. PR作成者の判定

手順 1 で取得した PR 情報から作成者（author）を確認する。

```powershell
gh pr view <PR番号> --json author --jq ".author.login"
```

現在の GitHub ユーザーを取得する。

```powershell
gh api user --jq ".login"
```

- **作成者と現在のユーザーが異なる場合** → 手順 3A（承認してマージ）へ
- **作成者と現在のユーザーが同じ場合** → 手順 3B（バイパスマージ）へ

### 3A. 承認してマージ（他者が作成したPR）

```powershell
gh pr review <PR番号> --approve
gh pr merge <PR番号> --merge --delete-branch
```

手順 4 へ進む。

### 3B. バイパスマージ（自分が作成したPR）

GitHub では自分が作成した PR を承認できないため、承認をスキップして直接マージする。
ブランチ保護ルールのバイパス権限が必要。

```powershell
gh pr merge <PR番号> --merge --delete-branch --admin
```

手順 4 へ進む。

### 4. マージ完了確認

`gh pr merge` の出力だけでは信用できないため、必ず状態を確認する。

```powershell
gh pr view <PR番号> --json state --jq ".state"
```

- `MERGED` → マージ成功。手順 5 へ進む。
- `OPEN` または `CLOSED` → **マージ失敗**。ユーザーにエラー報告し、スキルを中止する。

### 5. 派生元ブランチに戻す

手順 1 で取得した PR 情報から baseRefName（マージ先ブランチ）を確認する。

現在のブランチと baseRefName を比較し:

- **異なる場合**（自分のPRをマージした場合）:
  ```powershell
  git checkout <baseRefName>
  git pull origin <baseRefName>
  ```

- **同じ場合**（他人のPRを承認した場合。既に baseRefName にいる）:
  ```powershell
  git pull origin <baseRefName>
  ```

### 6. 完了報告

マージ完了と現在のブランチをユーザーに報告する。

## MUST
- 手順 4 のマージ完了確認を必ず実行すること（`gh pr view` で `MERGED` を確認）
- 前提条件（LGTM + CI pass）を満たさない場合は承認・マージしないこと
- 手順 2 でPR作成者を判定し、適切な手順（3A or 3B）を選択すること

## MUST NOT
- MUST 指摘が残っている PR を承認してはいけない
- マージ完了確認をスキップしてはいけない
- `gh pr merge` の出力のみでマージ成功と判断してはいけない
- 自分が作成した PR に対して `gh pr review --approve` を実行してはいけない（エラーになる）

## 完了条件
- PR が `state: MERGED` であること
- baseRefName にチェックアウトし、最新の状態に pull 済みであること
