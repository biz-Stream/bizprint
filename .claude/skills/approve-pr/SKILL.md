---
name: approve-pr
description: PRを承認・マージし、マージ完了を確認してブランチを戻す。「承認＆マージ」「approve」等のキーワードで使用する。
model: opus
effort: low
shell: powershell
---

# approve-pr

## Purpose

PRの承認・マージ・マージ完了確認・ブランチ戻しを一貫して実行する。
レビュースキル（`/create-pr`、`/check-review`）とは分離されている。承認権限については `.claude/rules/pr-approval.md` を参照。

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
```

approve が成功したことを確認してから merge を実行する。

```powershell
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

### 5. ワークツリー判定とブランチ戻し

現在のセッションがワークツリー上かどうかを判定する:

```powershell
git rev-parse --show-toplevel
```

- **ワークツリー上の場合**（パスに `.claude/worktrees/` を含む）:
  → `ExitWorktree` ツールを実行してワークツリーを削除し、本体リポジトリに戻る。
  ブランチ戻し（checkout/pull）は不要。

- **本体リポジトリの場合**:
  PR 情報から baseRefName（マージ先ブランチ）を取得する。

  ```powershell
  gh pr view <PR番号> --json baseRefName --jq ".baseRefName"
  ```

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

### 6. baseRefName マージ後 CI 監視開始

マージ後に baseRefName で新たに起動される CI ワークフローを Monitor ツールで非同期監視する。
`@_shared/ci-monitor.md` のテンプレートを使用する。監視対象 SHA は Monitor 起動時の `HEAD` で自動取得されるため、手順 5 で baseRefName に checkout・pull 済みの状態で呼び出す。

Monitor ツールのパラメータ:

- **timeout_ms**: `600000`（10分。bizprint の Build ワークフローは 1.6〜2.6 分のため十分）
- **persistent**: `false`
- **description**: `<baseRefName> マージ後 CI ワークフロー監視`

**ワークツリー上の場合はこのステップをスキップする**（ExitWorktree 後は本体リポジトリの状態に触らない方針）。
スキップした場合は手順 7 の完了報告でユーザーに手動確認を依頼する旨を含める。

Monitor 起動後はブロックせず手順 7 に進む。CI 結果は後追いで通知される。

### 7. 完了報告

以下をユーザーに報告する:

- マージ完了（PR 番号、`state: MERGED`）
- 現在のブランチ（またはワークツリー削除完了）
- baseRefName の CI 監視を開始した旨（ワークツリー時はスキップした旨と手動確認の依頼）

## MUST
- 手順 4 のマージ完了確認を必ず実行すること（`gh pr view` で `MERGED` を確認）
- 前提条件（LGTM + CI pass）を満たさない場合は承認・マージしないこと
- 手順 2 でPR作成者を判定し、適切な手順（3A or 3B）を選択すること
- 本体リポジトリの場合、手順 6 の baseRefName CI 監視 Monitor を必ず起動すること

## MUST NOT
- MUST 指摘が残っている PR を承認してはいけない
- マージ完了確認をスキップしてはいけない
- `gh pr merge` の出力のみでマージ成功と判断してはいけない
- 自分が作成した PR に対して `gh pr review --approve` を実行してはいけない（エラーになる）

## 完了条件
- PR が `state: MERGED` であること
- ワークツリー上の場合: ExitWorktree でワークツリーが削除され、本体リポジトリに戻っていること
- 本体リポジトリの場合: baseRefName にチェックアウトし、最新の状態に pull 済みであること、かつ baseRefName の CI 監視 Monitor が起動されていること
