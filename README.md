# phpuploaderCS

![phpuploader](http://teamwind.serveblog.net/github/phpuploader/phpuploader.jpg)

本プロジェクトは、Visual Basic .netによるサーバーサイドの保守ツールです。  
指定フォルダをzip(password付)し指定サイズで分割後、他サーバーへアップロード(php)しバックアップしています。  
  
処理内容は、  
1.指定フォルダをzip。  
2.負荷のかからないサイズの複数ファイルにカット。  
3.順次phpサーバーへアップロードしています。  
4.phpサーバー側で結合し保管。phpも添付しています。(receive.php)  
5.以上を日次または即時実行。  
以上です。  
  
※※補足※※  
同じ処理を別の言語で作成した下記プロジェクトも公開中です。よろしければこちらのリポリトジもご覧ください。  
phpuploaderCS c#版  
phpuploaderNJS Node.js版  
  
# Requirement
  
Windows11上で書いています。  
nvmにてnodejsをインストール。現時点では下記バージョンです。  
Visual Studio 2022を使用しています。Version 17.14.13 (August 2025)  
.net framework4.8を指定しているので適宜変更してください。  
zipは、NuGetでSharpZipLibライブラリを使用しています。  
アップロードは、Microsoft.VisualBasic.Devices.Network()を使用しています。[参照にMicrosoft.VisualBasic.dllを追加しています。]  
  
本プロジェクトを実行するためのphpサーバーを別途用意してください。  
  
# Usage
  
1.起動後、上図サンプル画面を参考に入力欄を設定してください。startボタンで開始します。  
2.分割サイズは、php.iniの「post_max_size = 」側と合わせて調整してください。  
3.もし公開サーバーで検証する場合は、phpのファイル名を複雑怪奇にしたり認証処理も追加するなどセキュリティを強化した方がよいと思います。  
  
# How It Works
  
  1.メイン処理は、backgroundWorker内に書いています。  
  2.メイン処理は、永久ループしています。外部からのトリガーによって処理を開始しています。  
  3.トリガーは、即時実行と日次実行があります。timer内でトリガーフラグを制御しています。    
  4.日次の場合は、設定した次回実行日時とシステム日を比較判定しています。
  5.phpは、受信したファイルのmd5値をprmと比較、最終ファイルなら結合をしています。  

# Tecnical Details
  
1.SharpZipLibでzip。  
2.ファイルをFileStreamでカット。  
3.md5取得。  
4.My.Computer.Network.UploadFileでupload。  
5.非同期処理。  
6.デリゲートでUI更新。  
7.php連携。  
  
# Note
  
実際に運用する場合は、もう少しセキュリティとエラー対策を強化する必要があります。サーバー負荷軽減の調整も必要です。  
コードはすべてwindows前提です。他OSはパス等適宜変更してください。  
バグがあるかもしれません。自己責任でご利用ください。また適宜コード変更してください。  
ご要望等がございましたらメール下さい。  
  
# License
  
MIT license。オリジナルコードの著作権は、Teamwindです。それ以外のライブラリ等の著作権は各々の所有者に帰属します。  

