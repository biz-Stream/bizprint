# CI 監視 Monitor テンプレート

CI ワークフローの状態を監視する Monitor を作成する。
Monitor ツールを以下のパラメータで呼び出すこと。

- **description**: `CI ワークフロー監視 (<ブランチ名>)`
- **timeout_ms**: `600000`（10分）
- **persistent**: `false`
- **command**:

```powershell
$start = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
while ($true) {
  Start-Sleep -Seconds 30
  try {
    $run = gh run list --branch "<ブランチ名>" --limit 1 --json status,conclusion,createdAt 2>$null |
      ConvertFrom-Json | Select-Object -First 1
    if (-not $run) {
      Write-Output "CI NOT_FOUND: ワークフローが見つかりません（CI 対象外の変更です）。"; exit 1
    }
    if ($run.createdAt -lt $start) { continue }
    switch ($run.conclusion) {
      'success'   { Write-Output "CI SUCCESS: ワークフローが成功しました。"; exit 0 }
      'failure'   { Write-Output "CI FAILED: ワークフローが失敗しました (conclusion: failure)。確認してください。"; exit 1 }
      'cancelled' { Write-Output "CI FAILED: ワークフローが失敗しました (conclusion: cancelled)。確認してください。"; exit 1 }
    }
  } catch { Write-Warning $_.Exception.Message }
}
```

`<ブランチ名>` は `git branch --show-current` で取得した値に置き換えること。

## 動作仕様

- Monitor 起動時の時刻を自動記録し、それ以降に作成されたランのみを対象にする（古いランの誤検知を防止）
- 30秒間隔でポーリング（LLMターン不要）
- terminal state（success / failure / cancelled）を検知すると1行出力して自動終了
- ワークフローが見つからない場合も1行出力して終了
- API エラー・古いラン検知時はリトライ（出力なし）
- タイムアウト10分

## CI 監視の検知後

Monitor からの通知を受け取ったら:

- **CI SUCCESS** → ユーザーに報告する。
- **CI FAILED** → ユーザーに確認する。修正・再プッシュした場合は CI 監視 Monitor を再作成する。
- **CI NOT_FOUND** → ユーザーに報告する。
