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

   1:<?
   2:    //sample php for phpuploader. phpuploader用receivesample php
   3:    //Split file reception and merging process. 分割ファイル受信結合処理
   4:
   5:    //this store them by day of the week, meaning keep a 7-day supply.
   6:    //曜日ごとに保管しています。つまり7日分保持しています。
   7:
   8:    //Please change each setting as appropriate.
   9:    //各設定は、適宜変更してください。
  10:
  11:    //MIT license (c)teamwind n.h
  12:
  13:    //(Translation by Google)
  14:
  15:    //Storage directory. 保管dir
  16:    //for windows
  17:    $storagedir = "c:\\backup\\";
  18:
  19:    //?
  20:    if($_FILES["file"]["tmp_name"]==""){
  21:        throw new \Exception($dir.$zipname."?no file");
  22:        exit;
  23:    }
  24:
  25:    //Sub-dir name of storage dir. 保管dirの下位dir名
  26:    $week = array('sun','mon','tue','wed','thu','fri','sat');
  27:
  28:    $date = date('w');
  29:
  30:    //prm analysis. prm解析
  31:    $prms = explode(',', mb_convert_encoding($_GET["prm"], "SJIS", "UTF-8"));
  32:
  33:    //prm=zip File name+Division Number(000-nnn),Division Number(000-nnn),Final division number,this md5,zip md5
  34:    //prm=zipファイル名+分割番号,分割番号,最終分割番号,当該md5,結合したzipのmd5
  35:    //abc.zip.000,2,xxxxxxxxxxxxxxxxxxxx(md5),xxxxxxxxxxxxxxxxxxxx(md5)
  36:    //abc.zip.001,2,xxxxxxxxxxxxxxxxxxxx(md5),xxxxxxxxxxxxxxxxxxxx(md5)
  37:    //abc.zip.002,2,xxxxxxxxxxxxxxxxxxxx(md5),xxxxxxxxxxxxxxxxxxxx(md5)
  38:    $_sepname = $prms[0];
  39:    $_no = (int)$prms[1];
  40:    $_lastno = (int)$prms[2];
  41:    $_md5 = $prms[3];
  42:    $_zipmd5 = $prms[4];
  43:
  44:    //Create a storage directory. 保管dirの生成
  45:    $dir = $storagedir."\\".$week[$date]."\\";
  46:    if(file_exists($dir)){
  47:    }else{
  48:        mkdir($dir, 0777, true);
  49:    }
  50:    //move file. 移動
  51:    move_uploaded_file($_FILES["file"]["tmp_name"], $dir.$_sepname);
  52:
  53:    //md5 check. md5チェック
  54:    $md5 = md5_file($dir.$_sepname);
  55:    if($_md5 === $md5){
  56:    } else {
  57:        throw new \Exception($dir.$_sepname."md5 error");
  58:    }
  59:
  60:    //If it is the last file, start joining. もし最終ファイルなら結合開始
  61:    if($_no == $_lastno){
  62:
  63:        //Zip file name without [.nnn]. zip file名は[.nnn]を除いたもの   abc.zip.000
  64:        $zipname = substr($_sepname, 0, strlen($_sepname)-4);
  65:
  66:        //Generate a file list. ファイルリストを生成する
  67:        //abc.zip.000
  68:        //abc.zip.001
  69:        //abc.zip.002
  70:        for($i = 0; $i <= $_lastno; $i++){
  71:            $files[$i] = $dir.$zipname.".".sprintf("%03d", $i);
  72:        }
  73:
  74:        //Combine these. これらを結合
  75:        if($_lastno == 0){
  76:            //If it's single, it's just a copy. 単一ならただのコピー
  77:            copy($dir.$_sepname, $dir.$zipname);
  78:            unlink($dir.$_sepname);
  79:        } else {
  80:            //Destination file name. 出力先のファイル名
  81:            $outputFile = $dir.$zipname;
  82:
  83:            //Open the output file. 出力ファイルを開く
  84:            $outputHandle = fopen($dir.$zipname, 'wb');
  85:            if(!$outputHandle){
  86:                //NG
  87:            } else {
  88:                //join. 結合
  89:                //Read each file in turn and combine them.  各ファイルを順番に読み込んで結合
  90:                foreach ($files as $file) {
  91:                    if (!file_exists($file)) {
  92:                        continue;
  93:                    }
  94:                    $inputHandle = fopen($file, 'rb');
  95:                    if (!$inputHandle) {
  96:                        continue;
  97:                    }
  98:                    //Read the file contents and write them to the output file.  ファイル内容を読み込んで出力ファイルに書き込む
  99:                    while(!feof($inputHandle)) {
 100:                        //64kbyte
 101:                        $buffer = fread($inputHandle, 65536);
 102:                        fwrite($outputHandle, $buffer);
 103:                    }
 104:                    fclose($inputHandle);
 105:                    //Erase the original. 元を消す
 106:                    unlink($file);
 107:                }
 108:                //Close the output file.  出力ファイルを閉じる
 109:                fclose($outputHandle);
 110:                //md5 check. md5チェック
 111:                $md5 = md5_file($dir.$zipname);
 112:                if($_zipmd5 === $md5){
 113:                } else {
 114:                    throw new \Exception($dir.$zipname."md5 error);
 115:                }
 116:            }
 117:        }
 118:    }
 119:?>
 120:
 121:<form action="./receive.php" method="POST" enctype="multipart/form-data"> 
 122:  <input type="file" name="file"> 
 123:  <input type="submit" value="phpuploader sample php"> 
 124:</form> 

(Translation by Google)