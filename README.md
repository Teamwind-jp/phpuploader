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

This is a sample php. 以下サンプルphpです。  

# PHP
    //php start
    
    //sample php for phpuploader. phpuploader用receivesample php
    //Split file reception and merging process. 分割ファイル受信結合処理

    //this store them by day of the week, meaning keep a 7-day supply.
    //曜日ごとに保管しています。つまり7日分保持しています。

    //Please change each setting as appropriate.
    //各設定は、適宜変更してください。

    //MIT license (c)teamwind n.h

    //(Translation by Google)

    //Storage directory. 保管dir
    //for windows
    $storagedir = "c:\\backup\\";

    //?
    if($_FILES["file"]["tmp_name"]==""){
        throw new \Exception($dir.$zipname."?no file");
        exit;
    }

    //Sub-dir name of storage dir. 保管dirの下位dir名
    $week = array('sun','mon','tue','wed','thu','fri','sat');

    $date = date('w');

    //prm analysis. prm解析
    $prms = explode(',', mb_convert_encoding($_GET["prm"], "SJIS", "UTF-8"));

    //prm=zip File name+Division Number(000-nnn),Division Number(000-nnn),Final division number,this md5,zip md5
    //prm=zipファイル名+分割番号,分割番号,最終分割番号,当該md5,結合したzipのmd5
    //abc.zip.000,2,xxxxxxxxxxxxxxxxxxxx(md5),xxxxxxxxxxxxxxxxxxxx(md5)
    //abc.zip.001,2,xxxxxxxxxxxxxxxxxxxx(md5),xxxxxxxxxxxxxxxxxxxx(md5)
    //abc.zip.002,2,xxxxxxxxxxxxxxxxxxxx(md5),xxxxxxxxxxxxxxxxxxxx(md5)
    $_sepname = $prms[0];
    $_no = (int)$prms[1];
    $_lastno = (int)$prms[2];
    $_md5 = $prms[3];
    $_zipmd5 = $prms[4];

    //Create a storage directory. 保管dirの生成
    $dir = $storagedir."\\".$week[$date]."\\";
    if(file_exists($dir)){
    }else{
        mkdir($dir, 0777, true);
    }
    //move file. 移動
    move_uploaded_file($_FILES["file"]["tmp_name"], $dir.$_sepname);

    //md5 check. md5チェック
    $md5 = md5_file($dir.$_sepname);
    if($_md5 === $md5){
    } else {
        throw new \Exception($dir.$_sepname."md5 error");
    }

    //If it is the last file, start joining. もし最終ファイルなら結合開始
    if($_no == $_lastno){

        //Zip file name without [.nnn]. zip file名は[.nnn]を除いたもの   abc.zip.000
        $zipname = substr($_sepname, 0, strlen($_sepname)-4);

        //Generate a file list. ファイルリストを生成する
        //abc.zip.000
        //abc.zip.001
        //abc.zip.002
        for($i = 0; $i <= $_lastno; $i++){
            $files[$i] = $dir.$zipname.".".sprintf("%03d", $i);
        }

        //Combine these. これらを結合
        if($_lastno == 0){
            //If it's single, it's just a copy. 単一ならただのコピー
            copy($dir.$_sepname, $dir.$zipname);
            unlink($dir.$_sepname);
        } else {
            //Destination file name. 出力先のファイル名
            $outputFile = $dir.$zipname;

            //Open the output file. 出力ファイルを開く
            $outputHandle = fopen($dir.$zipname, 'wb');
            if(!$outputHandle){
                //NG
            } else {
                //join. 結合
                //Read each file in turn and combine them.  各ファイルを順番に読み込んで結合
                foreach ($files as $file) {
                    if (!file_exists($file)) {
                        continue;
                    }
                    $inputHandle = fopen($file, 'rb');
                    if (!$inputHandle) {
                        continue;
                    }
                    //Read the file contents and write them to the output file.  ファイル内容を読み込んで出力ファイルに書き込む
                    while(!feof($inputHandle)) {
                        //64kbyte
                        $buffer = fread($inputHandle, 65536);
                        fwrite($outputHandle, $buffer);
                    }
                    fclose($inputHandle);
                    //Erase the original. 元を消す
                    unlink($file);
                }
                //Close the output file.  出力ファイルを閉じる
                fclose($outputHandle);
                //md5 check. md5チェック
                $md5 = md5_file($dir.$zipname);
                if($_zipmd5 === $md5){
                } else {
                    throw new \Exception($dir.$zipname."md5 error);
                }
            }
        }
    }

    //php end

    <form action="./receive.php" method="POST" enctype="multipart/form-data"> 
      <input type="file" name="file"> 
      <input type="submit" value="phpuploader sample php"> 
    </form> 

