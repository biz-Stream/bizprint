---
globs:
  - "bizprint-server-java/**/*.java"
  - "config/checkstyle/**"
---

# CheckStyle コーディング規約

## 自動チェック

Java ファイル編集後、PostToolUse hook により CheckStyle が自動実行される。
違反が報告された場合はその場で修正すること。

手動で確認する場合:

```powershell
mvn checkstyle:check -pl <module> -Dcheckstyle.consoleOutput=true
```

## ボーイスカウトルール

ファイルを修正したら、**そのファイル全体**を CheckStyle 準拠にする。よくある違反と対処:

| 違反 | 対処 |
|---|---|
| `UnusedImports` | 不要な import を削除 |
| `ImportOrder` | java → javax → org → com.brainsellers の順に整理 |
| `AvoidStarImport` | `import java.util.*` → 個別 import に展開（static import は可） |
| `FileTabCharacter` | タブ → スペース4に変換 |
| `LeftCurly` | 中括弧を行末に移動（K&R スタイル） |
| `WhitespaceAround` | 演算子の前後にスペースを追加 |
| `MemberName` / `LocalVariableName` | camelCase にリネーム |
| `ConstantName` | 定数は UPPER_CASE にリネーム |

## EditorConfig（非 Java ファイル）

非 Java ファイルの変更時も EditorConfig に従う:

```powershell
mvn editorconfig:check
```

bizprint の `.editorconfig` に準拠:
- 文字コード: UTF-8
- 改行: CRLF
- 末尾空白の除去、最終行に改行
