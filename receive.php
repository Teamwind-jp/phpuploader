以下、サンプルphpです。適宜変更してお使いください。    
    
    <?php
    //php start
    
    //uploadされた分割ファイルを受信し指定フォルダに転送結合するサンプルです
    //下記サンプルは、日次処理を前提にして曜日ごとに保管しています。つまり7日分保持しています。
    //※このサンプルはWindowsサーバーを前提にしています。Linuxサーバーの場合は適宜パス等を変更してください。

    //MIT license (c)teamwind n.h
    
    //保管フォルダ例
    $storagedir = "c:\\backup\\";

    //?
    if($_FILES["file"]["tmp_name"]==""){
        throw new \Exception($dir.$zipname."?no file");
        exit;
    }

    //保管フォルダ下位名を曜日で決める
    $week = array('sun','mon','tue','wed','thu','fri','sat');
    $date = date('w');

    //prm解析
    $prms = explode(',', mb_convert_encoding($_GET["prm"], "SJIS", "UTF-8"));

    //prmは次の通り
    //prm=zipファイル名+分割番号,分割番号,最終分割番号,当該md5,結合したzipのmd5
    //例 3つに分割した場合は次のprmでクライアントからuploadされてくる
    //abc.zip.000,2,xxxxxxxxxxxxxxxxxxxx(md5),xxxxxxxxxxxxxxxxxxxx(md5)
    //abc.zip.001,2,xxxxxxxxxxxxxxxxxxxx(md5),xxxxxxxxxxxxxxxxxxxx(md5)
    //abc.zip.002,2,xxxxxxxxxxxxxxxxxxxx(md5),xxxxxxxxxxxxxxxxxxxx(md5)
    $_sepname = $prms[0];
    $_no = (int)$prms[1];
    $_lastno = (int)$prms[2];
    $_md5 = $prms[3];
    $_zipmd5 = $prms[4];

    //保管フォルダの生成
    $dir = $storagedir."\\".$week[$date]."\\";
    if(file_exists($dir)){
    }else{
        mkdir($dir, 0777, true);
    }
    //保管フォルダへ移動
    move_uploaded_file($_FILES["file"]["tmp_name"], $dir.$_sepname);

    //md5チェック
    $md5 = md5_file($dir.$_sepname);
    if($_md5 === $md5){
    } else {
        throw new \Exception($dir.$_sepname."md5 error");
    }

    //もし最終ファイルなら結合開始
    if($_no == $_lastno){

        //file名は[.nnn]を除いたもの   abc.zip.000
        $zipname = substr($_sepname, 0, strlen($_sepname)-4);

        //ファイルリストを生成する
        //abc.zip.000
        //abc.zip.001
        //abc.zip.002
        for($i = 0; $i <= $_lastno; $i++){
            $files[$i] = $dir.$zipname.".".sprintf("%03d", $i);
        }

        //これらを結合
        if($_lastno == 0){
            //もし単一ならただのコピー
            copy($dir.$_sepname, $dir.$zipname);
            unlink($dir.$_sepname);
        } else {
            //出力先のファイル名
            $outputFile = $dir.$zipname;

            //出力ファイルを開く
            $outputHandle = fopen($dir.$zipname, 'wb');
            if(!$outputHandle){
                //NG
            } else {
                //結合
                //各ファイルを順番に読み込んで結合
                foreach ($files as $file) {
                    if (!file_exists($file)) {
                        continue;
                    }
                    $inputHandle = fopen($file, 'rb');
                    if (!$inputHandle) {
                        continue;
                    }
                    //ファイル内容を読み込んで出力ファイルに書き込む
                    while(!feof($inputHandle)) {
                        //64kbyte
                        $buffer = fread($inputHandle, 65536);
                        fwrite($outputHandle, $buffer);
                    }
                    fclose($inputHandle);
                    //元を消す
                    unlink($file);
                }
                //出力ファイルを閉じる
                fclose($outputHandle);
                //md5チェック
                $md5 = md5_file($dir.$zipname);
                if($_zipmd5 === $md5){
                } else {
                    throw new \Exception($dir.$zipname."md5 error);
                }
                //完了
            }
        }
    }

    //php end
    ?>

    <form action="./receive.php" method="POST" enctype="multipart/form-data"> 
      <input type="file" name="file"> 
      <input type="submit" value="phpuploader sample php"> 
    </form> 
