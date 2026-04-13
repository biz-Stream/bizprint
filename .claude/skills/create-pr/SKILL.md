---
name: create-pr
description: GitHub プルリクエストを作成する。ビルド成功・コミット・プッシュ完了後に使用。
model: opus
effort: low
shell: powershell
---

# create-pr スキル

## Purpose

GitHub プルリクエストを作成し、CI 監視とレビューエージェントを並列起動する。
PR 作成後の CI 確認・レビュー対応まで一貫して管理する。

## Usage

```
/create-pr
```

## 前提条件

- ローカルビルドが成功していること（`/maven-build` で確認）。
- コミット・プッシュが完了していること。
- `docs/` 配下のドキュメントを新規作成・大幅更新した場合は、ソースコード照合レビューが完了していること（`.claude/rules/reference-docs.md` 参照）。

## 手順

### 1. PR 作成

`.github/pull_request_template.md` を読み込み、テンプレートに沿って本文を作成する。

```powershell
gh pr create `
  --title "<イシュー番号に対応するタイトル>" `
  --body "<テンプレートに沿った本文>"
```

### 2. CI 監視とレビューの開始

PR 作成が完了したら、**CI ワークフロー監視**と**レビューエージェント**を並列で開始する。

#### 2a. CI 監視 Monitor の作成

`.claude/skills/_shared/ci-monitor.md` を Read し、テンプレートのコマンド・パラメータに従って Monitor ツールを呼び出す。
監視対象 SHA は Monitor 起動時の `HEAD` で自動取得されるため、push 完了直後の状態（HEAD が push 済みコミット）で呼び出すこと。

#### 2b. pr-reviewer エージェントの起動

@_shared/pr-reviewer-launch.md の手順に従って Agent ツールを起動する。

pr-reviewer エージェントは独立したコンテキストで PR の差分を取得・レビューし、
PR コメント投稿を自ら行ったうえで、Code Review フォーマットの全文を親セッションに返す。

#### 2c. ユーザーへの案内

CI 監視 Monitor と pr-reviewer エージェントを開始したら、ユーザーに以下を案内する:

> PR を作成しました。レビューエージェントを起動しました。
> CI とレビューを並列で実行しています。完了次第、結果を報告します。

#### CI 監視のスキップ

ユーザーが CI 待ちをスキップしたい場合（ドキュメントのみの変更等）は、
CI 監視 Monitor を作成せず pr-reviewer エージェントのみ開始する。

### 3. CI 監視の検知後

Monitor からの通知を受け取ったら、`CI_RESULT: <value>` の値に応じて対応する:

- **`CI_RESULT: success`** / **`CI_RESULT: skipped`**: CI 完了を記録し、レビューエージェントの完了を待つ（既にレビュー対応中ならそのまま続行）。
- **`CI_RESULT: not_found`**: 該当 SHA のワークフローが作成されていない旨をユーザーに報告し、判断を仰ぐ。
- **`CI_RESULT: failure`** / **`CI_RESULT: cancelled`**: 以下の手順で対応する:
  1. `gh run view <run-id> --log-failed` で失敗ログを取得し、原因を特定する
  2. 修正を実施する
  3. コミット・プッシュする
  4. 新しい CI 監視 Monitor を作成して再監視する

### 4. レビューエージェント完了後

pr-reviewer エージェントは自身で PR コメント投稿を行い、Code Review フォーマットの全文を親セッションに返す。

@_shared/agent-result-display.md のルールに従い、返却値の全文をユーザーに表示する。

#### 結果に応じた対応

- **LGTM（指摘なし）** → ユーザーに報告して終了する。
- **指摘あり** → `/check-review <PR番号>` を実行して指摘事項への対応を開始する（対応完了後の CI 再監視・再レビューは `/check-review` 側で実施する）

### 5. 完了条件

以下が両方揃ったらユーザーに完了を報告する:

- CI 成功（または CI 対象外）
- レビュー LGTM

## MUST

- PR 作成前にユーザーに確認すること。
- PR 本文に `Closes #<イシュー番号>` を記述してイシューと連携すること。
- 前提条件に該当しない場合は中断してユーザーに報告すること。

## MUST NOT

- テンプレートの構造を変更しないこと。
