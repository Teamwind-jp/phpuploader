# phpuploader
windowsサーバー間のデータバックアップツールです。  
所謂バックエンドソフトです。  
個人で運用している本/予備サーバー同士のデータ交換に使用しています。  
受信する側は、phpを使用しています。
本exeの機能は、  
1.指定フォルダをzip。  
2.負荷のかからないサイズの複数ファイルにカット。  
3.順次他サーバーへ送信しています。  
4.以上を日次バッチまたは即時実行。  
の以上です。  
下記に受信側phpコードを載せてます。  
  
c#版も公開しています。  
  
# Requirement
特にありません。windowsであれば動きます。  
.net framework4.8を指定しているので適宜変更してください。  
VS2022でビルドしています。  
zipは、NuGetからSharpZipLibライブラリを使用しています。  
アップロードは、My.Computer.Network.UploadFileを使用しています。  
受信側は、php必須です。ただ下記phpはパスの設定などwindows前提のコードです。   
  
# Usage
![phpuploader](http://teamwind.serveblog.net/github/phpuploader/001.jpg)

1.データを受信するphpのURLを指定します。  
　サーバー側でポート制限したりURLやphpファイル名を複雑にするなどしてセキュリティを確保してください。  
2.バックアップするフォルダをD&Dします。5つまで指定可能。  
3.転送するサイズを指定します。  
　サーバーに負荷のかからないサイズを指定してください。また、webサーバーのアップロードサイズ制限と確認してください。  
4.zipに付けるパスワードを指定します。  
　万一の漏洩に備えて頑丈なパスワードを指定してください。  
5.開始時刻を指定します。  
　日次バッチ開始時刻を指定します。[now]は、一回限りです。バッチ時刻指定しても即時実行されます。  

# How It Works
1.timerでバッチ時刻監視しています。zipしてphpに送信する処理は、処理を独占するのでbackground workerで実行しています。  
2.background workerは、無限ループしています。timer内、バッチ時刻到達で立てたフラグを確認して処理を実行しています。  
3.フラグは、グローバルで保持していますがbackground workerからもアクセスしているので注意が必要かもしれません。（実稼働では問題は出ていません）

# Tecnical Details
主なコードは、以下の通りです。  
1.バックグラウンドワーカーを使用。  
2.デリゲート。  
3.SharpZipLib操作。  
4.ドラッグドロップ。  
5.ログファイルの読み書き。  
6.md5取得。  
7.ファイル分割。  
8.php連動。My.Computer.Network.UploadFile使用。  
9.phpで結合処理。

# License
MIT license. Copyright: Teamwind.
zip uses the SharpZipLib library.

MIT license。著作権は、Teamwindです。  
zipは、SharpZipLibライブラリ使用。  

# Note
バグがあるかもしれません。自己責任でご利用ください。また適宜コード変更してください。  
ご要望等がございましたらメール下さい。  

以下サンプルphpです。  

# PHP
    //php start
    
    //sample php for phpuploader. phpuploader用receivesample php
    //Split file reception and merging process. 分割ファイル受信結合処理

    //曜日ごとに保管しています。つまり7日分保持しています。
    //各設定は、適宜変更してください。
    //MIT license (c)teamwind n.h

    //Storage directory. 保管dir for windows
    $storagedir = "c:\\backup\\";

    //?
    if($_FILES["file"]["tmp_name"]==""){
        throw new \Exception($dir.$zipname."?no file");
        exit;
    }

    //保管dirの下位dir名　曜日ごとのサブdir名
    $week = array('sun','mon','tue','wed','thu','fri','sat');

    $date = date('w');

    //prm analysis. prm解析
    $prms = explode(',', mb_convert_encoding($_GET["prm"], "SJIS", "UTF-8"));

	//パラメタ解析
    //prm=zipファイル名+分割番号,当該分割番号,全体の最終分割番号,当該md5,結合したzipのmd5
	//サンプル
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
    //move file. 受信したファイルを保管先へ移動
    move_uploaded_file($_FILES["file"]["tmp_name"], $dir.$_sepname);

    //md5 check. md5チェック
    $md5 = md5_file($dir.$_sepname);
    if($_md5 === $md5){
    } else {
        throw new \Exception($dir.$_sepname."md5 error");
    }

    //If it is the last file, start joining. もし最終ファイルなら結合開始
    if($_no == $_lastno){

        //Zip file name without [.nnn]. zip file名は[.nnn]を除いたもの   abc.zip.000　なので後ろ4文字削除
        $zipname = substr($_sepname, 0, strlen($_sepname)-4);

        //Generate a file list. 結合するファイルリストを生成する
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



(Translation by Google)