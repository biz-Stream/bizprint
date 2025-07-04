# biz-Stream ダイレクト印刷・バッチ印刷 (オープンソース版)

このリポジトリでは[ブレインセラーズ・ドットコム株式会社](https://www.brainsellers.com/)が提供している[biz-Stream](https://www.brainsellers.com/product/bizstream/)製品のうち、ダイレクト印刷とバッチ印刷に関するソースを公開しています

※"biz-Stream"はブレインセラーズ・ドットコム株式会社の登録商標です

## パッケージ版との違い
"パッケージ版"とは biz-Stream製品メディアに同梱されているダイレクト印刷・バッチ印刷のことを指し、有償製品です
* ソースコードの違い
  * sppファイルを暗号化するキーが異なりますが、それ以外は基本的には同一です
* インストーラの違い
  * パッケージ版・オープンソース版共にInnoSetupによるインストーラが用意されます
  * パッケージ版にはインストーラにデジタル署名がされますが、オープンソース版はデジタル署名はされない予定です
* サポートの違い
  * パッケージ版はbiz-Stream テクニカルサポート契約を締結していればサポートを得られますが、オープンソース版では一切のサポートはありません

## モジュール一覧
* bizprint-client
  * ダイレクト印刷・バッチ印刷 クライアントモジュール
* bizprint-server-csharp
  * ダイレクト印刷・バッチ印刷へ送信する印刷データ(sppファイル)を作成するライブラリ (C#版)
  * 任意のアプリケーションに組み込むことで、そのアプリケーションからダイレクト印刷・バッチ印刷を行えるようになります
* bizprint-server-java
  * ダイレクト印刷・バッチ印刷へ送信する印刷データ(sppファイル)を作成するライブラリ (Java版)
  * 任意のアプリケーションに組み込むことで、そのアプリケーションからダイレクト印刷・バッチ印刷を行えるようになります
* build
  * ソース一式をビルドするためのMavenプロジェクト

## ビルドに必要なもの
* Windows 10 / Windows Server 2019 以上
  * Windowsの機能として、.NET Framework 3.5を有効化する必要があります
* JDK 8 以上
* Maven 3.0.3 以上
* Visual Studio 2019 以上
* InnoSetup 6 以上

## ビルド方法
```
cd build
mvn clean install
```
各モジュールのtargetディレクトリにビルド結果が出力されます

### 注意
* 暗号化キーはビルドのたびにランダムで生成されます
* 暗号化キーはサーバ側とクライアント側で同じでなければ印刷が行えません。そのため一部のモジュールのみビルドすることはできません。必ず一括ビルドしなければなりません

## インストール方法
* bizprint-client
  * ダイレクト印刷は DirectPrint-OSS-Setup-X.X.X.exe を実行してインストールします (要管理者権限)
  * バッチ印刷は BatchPrint-OSS-Setup-X.X.X.exe を実行してインストールします (要管理者権限)
* bizprint-server-charp
  * bizprint-server-csharp.dll および Ionic.Zip.dll を参照に追加します (Visual Studioの場合)
* bizprint-server-java
  * bizprint-server-java-X.X.X.jar および zip4j-X.X.X.jar をクラスパスに追加します

## 動作確認環境
* bizprint-client
  * ダイレクト印刷は Windows 11 以上で動作確認をしています
  * バッチ印刷は Windows Server 2022 以上で動作確認をしています
* bizprint-server-charp
  * .NET Framework 2.0 以上で動作しますが、 4.6 で動作確認をしています
* bizprint-server-java
  * JDK 8 以上で動作しますが、 JDK 21 で動作確認をしています

## 実行方法
* TBD
* サンプルを参照

## 開発への参加
* 不具合の修正や機能のブラッシュアップを目的として継続的に開発を行っております  
* 2025年6月時点ではブレインセラーズ以外の第三者がリポジトリに変更を加えることはできず、ソースを閲覧することのみ可能です

## ライセンス
* 本ソフトウェアは [Apache Licence v2.0](https://www.apache.org/licenses/LICENSE-2.0) の元提供されています  
※パッケージ版はライセンスが異なるのでご注意ください

## 注意事項
* 全ての記載事項は2025年6月時点の情報であり、今後変更される可能性があります