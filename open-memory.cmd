@echo off
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "$h = (Get-Location).Path -replace '[^a-zA-Z0-9\-]', '-';" ^
  "$m = \"$env:USERPROFILE\.claude\projects\$h\memory\";" ^
  "if (Test-Path $m) { explorer $m } else { Write-Host ('memory フォルダが見つかりません: ' + $m); pause }"
