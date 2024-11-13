# biz-Stream ダイレクト印刷・バッチ印刷 (オープンソース版)

このリポジトリでは[ブレインセラーズ・ドットコム株式会社](https://www.brainsellers.com/)が提供している[biz-Stream](https://www.brainsellers.com/product/bizstream/)製品のうち、ダイレクト印刷とバッチ印刷に関するソースを公開しています

※"biz-Stream"はブレインセラーズ・ドットコム株式会社の登録商標です

## パッケージ版との違い
"パッケージ版"とは biz-Stream製品メディアに同梱されているダイレクト印刷・バッチ印刷のことを指し、有償製品です
* ソースコードの違い
  * sppファイルを暗号化するキーが異なりますが、それ以外は基本的には同一です
* インストーラの違い
  * パッケージ版はブレインセラーズのデジタル署名がされたInstall Shieldによるインストーラが用意されますが、オープンソース版はデジタル署名は無くインストーラは2024年7月時点では存在しません
* サポートの違い
  * パッケージ版はbiz-Stream テクニカルサポート契約を締結していればサポートを得られますが、オープンソース版では一切のサポートはありません

## モジュール一覧
* bizprint_client
  * ダイレクト印刷・バッチ印刷 クライアントモジュール
* bsdpc-dotNET
  * ダイレクト印刷・バッチ印刷へ送信する印刷データ(sppファイル)を作成するライブラリ(C#版)
  * 任意のアプリケーションに組み込むことで、そのアプリケーションからダイレクト印刷・バッチ印刷を行えるようになります
* bsdpc
  * ダイレクト印刷・バッチ印刷へ送信する印刷データ(sppファイル)を作成するライブラリ(Java版)
  * 任意のアプリケーションに組み込むことで、そのアプリケーションからダイレクト印刷・バッチ印刷を行えるようになります

## ビルド・インストール方法
TBD  
2024年7月時点ではbiz-Stream製品からソースを一部切り出した状態で公開しており、現状では正常にビルドが行えません

## 動作確認環境
TBD

## ドキュメント
TBD

## 開発への参加
不具合の修正や機能のブラッシュアップを目的として継続的に開発を行っております  
2024年7月時点ではブレインセラーズ以外の第三者がリポジトリに変更を加えることはできず、ソースを閲覧することのみ可能です

## ライセンス
本ソフトウェアは [Apache Licence v2.0](https://www.apache.org/licenses/LICENSE-2.0) の元提供されています  
※パッケージ版はライセンスが異なるのでご注意ください

## 注意事項
* sppファイルはAES256で暗号化されたzip圧縮ファイル形式ですが、その暗号化キーの一部がハードコーディングされています。その暗号化キーを公開することは既存のbiz-Streamの顧客のセキュリティを下げることになるため本リポジトリにおけるソースでは伏字となっています
* 全ての記載事項は2024年7月時点の情報であり、今後変更される可能性があります
