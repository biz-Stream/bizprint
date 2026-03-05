# CheckStyle コーディング規約

## コミット前の確認手順

Java ソースを変更した場合、コミット前に以下を実行する:

```bash
mvn checkstyle:check -pl <module> -Dcheckstyle.consoleOutput=true
```

違反が報告された場合は修正してからコミットする。

## ボーイスカウトルール

ファイルを修正したら、**そのファイル全体**を CheckStyle 準拠にする:

1. 未使用 import の削除
2. import 順序の整理（java → javax → org → com.brainsellers）
3. スター import の排除（static import は可）
4. インデントの統一（スペース4）
5. 中括弧スタイルの統一（K&R: 開き括弧は行末）
6. フィールド・変数名は camelCase、定数は UPPER_CASE
7. 演算子の前後にスペースを追加
8. タブ文字は使用せずスペース4に統一

## よくある違反パターンと対処法

| 違反 | 対処 |
|---|---|
| `MemberName` / `LocalVariableName` | camelCase にリネーム |
| `ImportOrder` | IDE の Optimize Imports を使用するか手動で並べ替え |
| `UnusedImports` | 不要な import を削除 |
| `AvoidStarImport` | `import java.util.*` → 個別 import に展開 |
| `FileTabCharacter` | タブ → スペース4に変換 |
| `LeftCurly` | 中括弧を行末に移動（K&R スタイル） |
| `WhitespaceAround` | 演算子の前後にスペースを追加 |
| `ConstantName` | 定数は UPPER_CASE にリネーム |

## EditorConfig（非 Java ファイル）

非 Java ファイルの変更時も EditorConfig に従う:

```bash
mvn editorconfig:check
```

bizprint の `.editorconfig` に準拠:
- 文字コード: UTF-8
- 改行: CRLF
- 末尾空白の除去、最終行に改行
