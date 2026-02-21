# Changelog

このプロジェクトのすべての注目すべき変更はこのファイルに記録されます。

フォーマットは [Keep a Changelog](https://keepachangelog.com/ja/1.0.0/) に基づいており、
バージョニングは [Semantic Versioning](https://semver.org/lang/ja/) に準拠しています。

## [Unreleased]

### 追加

- CLAUDE.md、.editorconfig、.gitattributes、CheckStyle 設定などのプロジェクト設定ファイルを追加
- CheckStyle エラー38件と EditorConfig 違反を修正し、ビルド時の自動チェックを有効化
- ダイレクト印刷・バッチ印刷・ステータス照会の Java サンプルプログラムを追加

### 変更

- `build/pom.xml` をプロジェクトルートへ移動し、標準的な Maven マルチモジュール構成に変更

## [1.0.0-rc2] - 2025-11-17

### 追加

- GitHub Actions CI 対応（self-hosted Windows ランナーでのビルド自動化）
- プルリクエスト時の自動ビルドを追加
- プルリクエストテンプレートを追加
- CONTRIBUTING.md を追加

### 変更

- JDK 21 でのビルドに対応
- AssemblyInfo.cs のファイルエンコーディングを UTF-8 with BOM に変更
- ファイルエンコーディング・改行コードの統一
- servlet-api を javax から jakarta へ変更
- JDK 互換バージョンを 1.8 に設定
- README.md に CONTRIBUTING.md へのリンクを追加、誤記修正
- AutoApprove ワークフローを削除

## [1.0.0-rc1] - 2025-01-24

### 変更

- README.md を更新

### 修正

- Inno Setup インストーラースクリプトの修正（BatchPrint-OSS.iss、DirectPrint-OSS.iss）
- PDFBatchPrintStream、PDFBatchStatus のバグ修正

## [1.0.0-alpha3] - 2024-12-02

### 変更

- ファイルプロパティの修正
- 依存関係のモジュール（Ionic.Zip.dll、log4net.dll 等）もリリースアーカイブに含めるように変更

## [1.0.0-alpha2] - 2024-11-21

### 変更

- フォルダ名を変更（`bsdpc` -> `bizprint-server-java`、`bsdpc-dotNET` -> `bizprint-server-csharp`、`bizprint_client` -> `bizprint-client`）
- C# 版の名前空間を変更、DLL ファイル名を変更
- Java 版の Jar ファイル名を変更

## [1.0.0-alpha1] - 2024-11-13

### 追加

- biz-Stream 製品版からソースを切り出し、初回リリース
- bizprint-server-java: spp ファイル生成ライブラリ（Java 版）
- bizprint-server-csharp: spp ファイル生成ライブラリ（C# 版）
- bizprint-client: ダイレクト印刷・バッチ印刷 Windows クライアント
- 一括ビルド対応（Maven マルチモジュール構成）
- 暗号化キー自動生成に対応
- Inno Setup インストーラーを追加

[Unreleased]: https://github.com/biz-Stream/bizprint/compare/v1.0.0-rc2...HEAD
[1.0.0-rc2]: https://github.com/biz-Stream/bizprint/compare/v1.0.0-rc1...v1.0.0-rc2
[1.0.0-rc1]: https://github.com/biz-Stream/bizprint/compare/v1.0.0-alpha3...v1.0.0-rc1
[1.0.0-alpha3]: https://github.com/biz-Stream/bizprint/compare/v1.0.0-alpha2...v1.0.0-alpha3
[1.0.0-alpha2]: https://github.com/biz-Stream/bizprint/compare/v1.0.0-alpha1...v1.0.0-alpha2
[1.0.0-alpha1]: https://github.com/biz-Stream/bizprint/releases/tag/v1.0.0-alpha1
