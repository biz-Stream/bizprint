---
name: maven-build
description: Maven 一括ビルドを実行する。コミット・プッシュ前の確認に使用。
model: opus
effort: low
shell: powershell
---

# maven-build スキル

## Purpose

コミット・プッシュ前にローカルビルドの成功を確認する。
暗号化キーがビルドのたびにランダム生成されるため、**一部モジュールのみのビルドは不可。必ず一括ビルドすること。**

## Usage

```
/maven-build
```

コード変更後、コミット・プッシュ前に実行する。ドキュメントのみの変更の場合はスキップ可。

## 手順

### 1. ビルド実行

```powershell
mvn clean install
```

### 2. 結果確認

- **BUILD SUCCESS**: ビルド成功。コミット・プッシュに進んでよい。
- **BUILD FAILURE**: エラーログを確認し、修正する。

### 3. よくある失敗パターン

| エラー | 対処 |
|---|---|
| CheckStyle 違反 | `mvn checkstyle:check -pl bizprint-server-java -Dcheckstyle.consoleOutput=true` で詳細確認 |
| EditorConfig 違反 | `mvn editorconfig:check` で詳細確認 |
| テスト失敗 | `mvn test -pl bizprint-server-java` で個別実行して原因特定 |
| コンパイルエラー | エラーメッセージのファイル・行番号を確認して修正 |

## 完了条件

`BUILD SUCCESS` が出力されること。

## MUST

- 一部モジュールのみのビルドは禁止。必ず `mvn clean install` で一括ビルドすること。
- Windows 環境で実行すること（C# ビルド・Inno Setup インストーラー生成を含むため）。

## MUST NOT

- `mvn clean install` 以外のビルドコマンド（`mvn -pl` 等）をビルド確認に使わないこと。
