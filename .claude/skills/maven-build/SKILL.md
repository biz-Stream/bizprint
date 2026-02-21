---
name: maven-build
description: Maven 一括ビルドを実行する。コミット・プッシュ前の確認に使用。
---

# maven-build スキル

Maven 一括ビルドを実行する。

## ビルドコマンド

### クリーンビルド（フルビルド）

```bash
mvn clean install
```

### パッケージビルド

```bash
mvn clean package
```

## 注意事項

- 暗号化キーはビルドのたびにランダム生成される。サーバー側とクライアント側で同じキーが必要なため、**一部モジュールのみのビルドは不可。必ず一括ビルドすること。**
- コミット・プッシュ前に必ずビルド成功を確認する。
- C# ビルド（Visual Studio）と Inno Setup インストーラー生成も含まれるため、Windows 環境が必須。
