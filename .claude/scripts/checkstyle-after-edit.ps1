# PostToolUse hook: Java ファイル編集後に CheckStyle を自動チェックする。
# bizprint-server-java 配下の .java ファイルが対象。

$jsonInput = $Input | Out-String
try {
    $parsed = $jsonInput | ConvertFrom-Json
    $filePath = $parsed.tool_input.file_path
} catch {
    exit 0
}

# bizprint-server-java 配下の Java ファイルでなければスキップ
if ($filePath -notmatch 'bizprint-server-java[/\\].*\.java$') {
    exit 0
}

Set-Location $env:CLAUDE_PROJECT_DIR
mvn checkstyle:check -pl bizprint-server-java -Dcheckstyle.consoleOutput=true 2>&1
# CheckStyle 違反があっても hook はブロックしない（情報提供のみ。違反はその場で修正すること）
exit 0
