# pr-reviewer エージェント起動テンプレート

Agent ツールで `.claude/agents/pr-reviewer/AGENT.md` のカスタムエージェントをフォアグラウンドで起動する。

```
Agent ツールのパラメータ:
  description: "PR レビュー"
  name: "pr-reviewer"
  prompt: "<プロジェクトディレクトリの絶対パス>/.claude/agents/pr-reviewer/AGENT.md を最初に読み、その手順に厳密に従って PR <PR番号> をレビューしてください。特に手順 6 を必ず実行すること: 6a. gh pr comment で PR コメントを投稿する。6b. Code Review フォーマットの全文をそのまま親セッションに返す（要約しない）。プロジェクトルートは <プロジェクトディレクトリの絶対パス> です。"
```

## MUST NOT

- **`subagent_type` は指定しないこと。** 指定するとビルトインの reviewer エージェントが起動され、カスタムの pr-reviewer（自己投稿・全文返却）が使われない。
