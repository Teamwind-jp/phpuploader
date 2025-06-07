# phpuploader
This is a data backup tool between Windows servers.
I use it to exchange data between my personal main/backup servers.
The functions of this exe are:
1. Zip the specified folder.
2. Cut into multiple files of a size that does not put a load on the server.
3. Sequentially send to other servers.
4. Execute the above daily or immediately.
That's all.
The receiving PHP code is below.

windowsサーバー間のデータバックアップツールです。  
個人で運用している本/予備サーバー同士のデータ交換に使用しています。  
本exeの機能は、  
1.指定フォルダをzip。  
2.負荷のかからないサイズの複数ファイルにカット。  
3.順次他サーバーへ送信しています。  
4.以上を日次または即時実行。  
の以上です。  
下記に受信側phpコードを載せます。  

# Requirement
Nothing in particular. It will work on Windows.
.Net Framework 4.8 is specified, so please change it as appropriate.
For zipping, I use the SharpZipLib library on NuGet.
For uploading, I use My.Computer.Network.UploadFile.
PHP is required on the receiving side. However, the following PHP code is based on Windows.

特にありません。windowsであれば動きます。  
.net framework4.8を指定しているので適宜変更してください。  
zipは、NuGetでSharpZipLibライブラリを使用しています。
アップロードは、My.Computer.Network.UploadFileを使用しています。
受信側は、php必須です。ただ下記phpはwindows前提コードです。  

# License
MIT license. Copyright: Teamwind.
zip uses the SharpZipLib library.

MIT license。著作権は、Teamwindです。  
zipは、SharpZipLibライブラリ使用。

# Note
There may be bugs. Use at your own risk. Also, modify the code accordingly.
バグがあるかもしれません。自己責任でご利用ください。また適宜コード変更してください。
If you have any requests, please email us. 
ご要望等がございましたらメール下さい。

This is a sample php. 以下サンプルphpです。  

# Space
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
?>

<form action="./receive.php" method="POST" enctype="multipart/form-data"> 
  <input type="file" name="file"> 
  <input type="submit" value="phpuploader sample php"> 
</form> 



(Translation by Google)