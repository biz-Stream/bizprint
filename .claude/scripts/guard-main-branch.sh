#!/usr/bin/env bash
# PreToolUse hook: main ブランチでのプロジェクト内ファイル直接編集をブロックする。
# プロジェクト外ファイル（auto memory 等）はガード対象外。

# stdin から file_path を取得し、プロジェクト内か判定
IN_PROJECT=$(python3 -c "
import sys, json, os
fp = os.path.normcase(os.path.abspath(
    json.load(sys.stdin).get('tool_input', {}).get('file_path', '')))
pd = os.path.normcase(os.path.abspath(
    os.environ.get('CLAUDE_PROJECT_DIR', '')))
print('1' if fp.startswith(pd + os.sep) or fp == pd else '0')
" 2>/dev/null)

# プロジェクト外ファイルはスルー
if [ "$IN_PROJECT" = '0' ]; then
  exit 0
fi

# プロジェクト内の場合、ブランチをチェック
BRANCH=$(git -C "$CLAUDE_PROJECT_DIR" rev-parse --abbrev-ref HEAD 2>/dev/null)
if [ "$BRANCH" = 'main' ]; then
  echo 'main ブランチでの直接編集は禁止です。/start-task でブランチを作成してください。' >&2
  exit 2
fi
