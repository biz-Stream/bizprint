# 目的
- ユーザー指示に基づき、正確かつ効率的に開発タスクを完了する。

# 製品概要
bizprint は [biz-Stream](https://www.brainsellers.com/product/bizstream/) 製品のダイレクト印刷・バッチ印刷機能を
オープンソース化したプロジェクトである（Apache License v2.0）。

サーバーサイドで印刷データ（spp ファイル）を生成し、クライアント（Windows サービス）が受信して印刷を実行する。

## biz-Stream との関係

bizprint は biz-Stream の印刷系モジュールを独立させたもの。
最終的に印刷系機能は bizprint に一本化し、biz-Stream からは削除する予定。

### モジュール対応表
| bizprint | biz-Stream |
|---|---|
| `bizprint-server-java` | `modules/bsdpc` |
| `bizprint-server-csharp` | `modules/bsdpc-dotNET` |
| `bizprint-client` | `modules/bizprint_client`, `packages/bizprint_clientp` |

### パッケージ版（biz-Stream 同梱版）との差異
- spp ファイルの暗号化キーが異なる（それ以外のソースは基本的に同一）
- インストーラーにデジタル署名なし
- テクニカルサポートなし

### 改修時の注意（必須）
- **印刷系機能を改修する際は biz-Stream プロジェクトの対応モジュールも同時に改修すること。**

## モジュール構成
| モジュール | 言語 | 責務 |
|---|---|---|
| `bizprint-server-java` | Java | spp ファイル生成ライブラリ（サーバー側） |
| `bizprint-server-csharp` | C# | spp ファイル生成ライブラリ（サーバー側） |
| `bizprint-client` | C# | ダイレクト印刷・バッチ印刷 Windows クライアント |
| `pom.xml`（ルート） | Maven | 一括ビルド用親プロジェクト |

### bizprint-client の内部構成
| プロジェクト | 責務 |
|---|---|
| `DirectPrintService` | ダイレクト印刷 Windows サービス |
| `BatchPrintService` | バッチ印刷 Windows サービス |
| `BizPrintCommon` | 共通ライブラリ |
| `SilentPdfPrinter` | PDF サイレント印刷 |
| `BizPrintHealthChecker` | ヘルスチェッカー |
| `SppFileExtractTool` | spp ファイル展開ツール |
| `BizCommonTests` | 共通ライブラリのユニットテスト |
| `DirectPrintInstaller` | ダイレクト印刷 Inno Setup インストーラー |
| `BatchPrintInstaller` | バッチ印刷 Inno Setup インストーラー |

## ビルド

### 必要環境
- Windows 10 / Windows Server 2019 以上（.NET Framework 3.5 有効化）
- JDK 8 以上
- Maven 3.0.3 以上
- Visual Studio 2019 以上
- Inno Setup 6 以上

### ビルド方法
```bash
mvn clean install
```

### 注意
- 暗号化キーはビルドのたびにランダム生成される。サーバー側とクライアント側で同じキーが必要なため、**一部モジュールのみのビルドは不可。必ず一括ビルドすること。**

# コーディング規約

## 基本方針
- 既存の慣習（ライブラリ、フレームワーク、コーディングスタイル、命名規則）を尊重する。
- 指示が曖昧な場合は必ず質問して確認する。
- 機能追加やバグ修正の際は、可能な限りユニットテストを作成・更新する。

## Java（bizprint-server-java）
- JDK 8 互換
- インデント: スペース4
- 命名: クラスは PascalCase、メソッド・フィールドは camelCase、定数は UPPER_CASE

## C#（bizprint-server-csharp, bizprint-client）
- .NET Framework 2.0 以上互換（動作確認は 4.6）
- 改行: CRLF
- 既存コードのスタイルに従う

# 安全・制約
- 破壊的コマンド（例: `rm`）はユーザー許可なく実行しない。
- 機密情報（APIキー、パスワード、個人情報）を出力しない/DB に保存するコードを書かない。
- 指示された範囲を超える無関係な変更はしない。

# 出力
- コード提示時は言語指定付きの Markdown コードブロックを使う。
- コミュニケーションは簡潔かつ明確に行う。
- 丸数字（(1)(2)(3)等）は一切使用禁止。番号付きリスト（1. 2. 3.）を使うこと。これはソースコード、ドキュメント、Claude Code の回答すべてに適用する。

# ワークフロー（必須）

## ブランチ運用
- `main` での直接作業は禁止。必ずイシュー作成・ブランチ作成してから作業する。
- ブランチ名: `<種別>/<イシュー番号>`（例: `feature/123`, `bugfix/456`, `chore/789`）

## プルリクエスト
- すべての変更は PR 経由で行う。
- PR 本文に `Closes #<イシュー番号>` を記述してイシューと連携する。
- CI が通っていない PR はマージしない。

## CI
- GitHub Actions（self-hosted Windows ランナー）で `mvn clean install` を実行。
- PR 作成時と main への push 時に自動実行。
