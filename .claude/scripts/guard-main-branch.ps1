# PreToolUse hook: main ブランチでのプロジェクト内ファイル直接編集をブロックする。
# プロジェクト外ファイル（auto memory 等）はガード対象外。

$jsonInput = $Input | Out-String
try {
    $parsed = $jsonInput | ConvertFrom-Json
    $filePath = [System.IO.Path]::GetFullPath($parsed.tool_input.file_path)
    $projectDir = [System.IO.Path]::GetFullPath($env:CLAUDE_PROJECT_DIR)

    # プロジェクト外ファイルはスルー
    if (-not $filePath.StartsWith($projectDir + [System.IO.Path]::DirectorySeparatorChar) -and $filePath -ne $projectDir) {
        exit 0
    }
} catch {
    # パース失敗時はプロジェクト内とみなして安全側に倒す
}

# プロジェクト内の場合、ブランチをチェック
$branch = git -C $env:CLAUDE_PROJECT_DIR rev-parse --abbrev-ref HEAD 2>$null
if ($branch -eq 'main') {
    [Console]::Error.WriteLine('main ブランチでの直接編集は禁止です。/start-task でブランチを作成してください。')
    exit 2
}
