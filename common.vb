Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Windows.Forms.AxHost
Module common

#Region "日付関連"


    Public Function cmn_datecmp(ByVal d1 As MyDateTime, ByVal d2 As MyDateTime) As Integer
        Return d1.myDate.CompareTo(d2.myDate)
    End Function

#End Region

#Region "乱数"

    '====================================================================
    '	ランダム値 min-max
    '====================================================================
    Function cmn_getRand(min As Integer, max As Integer) As Integer

        'Int32と同じサイズのバイト配列にランダムな値を設定する
        Dim bs As Byte() = New Byte(3) {}
        Dim rng As New System.Security.Cryptography.RNGCryptoServiceProvider()
        rng.GetBytes(bs)

        'Int32に変換する
        Return min + Math.Abs(System.BitConverter.ToInt32(bs, 0) Mod (max - min + 1))

    End Function


    '文字列
    Function cmn_generateSecurePassword(length As Integer) As String
        Dim chars As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()"
        Dim sb As New StringBuilder()
        Dim rng As New RNGCryptoServiceProvider()
        Dim data As Byte() = New Byte(0) {}

        For i As Integer = 1 To length
            rng.GetBytes(data)
            Dim index As Integer = data(0) Mod chars.Length
            sb.Append(chars(index))
        Next
        Return sb.ToString()
    End Function

#End Region

#Region "暗号化"

    ''' <summary>
    ''' 共有キー用に、バイト配列のサイズを変更する
    ''' </summary>
    ''' <param name="bytes">サイズを変更するバイト配列</param>
    ''' <param name="newSize">バイト配列の新しい大きさ</param>
    ''' <returns>サイズが変更されたバイト配列</returns>
    Private Function ResizeBytesArray(ByVal bytes() As Byte,
                                ByVal newSize As Integer) As Byte()
        Dim newBytes(newSize - 1) As Byte
        If bytes.Length <= newSize Then
            Dim i As Integer
            For i = 0 To bytes.Length - 1
                newBytes(i) = bytes(i)
            Next i
        Else
            Dim pos As Integer = 0
            Dim i As Integer
            For i = 0 To bytes.Length - 1
                newBytes(pos) = newBytes(pos) Xor bytes(i)
                pos += 1
                If pos >= newBytes.Length Then
                    pos = 0
                End If
            Next i
        End If
        Return newBytes
    End Function

    Public Sub cmn_encryptFile(ByVal fileName As String, ByVal key As String)

        '暗号化するファイルを読み込む
        Dim fsIn As New System.IO.FileStream(fileName,
            System.IO.FileMode.Open, System.IO.FileAccess.Read)
        'すべて読み込む
        Dim bytesIn(fsIn.Length - 1) As Byte
        fsIn.Read(bytesIn, 0, bytesIn.Length)
        '閉じる
        fsIn.Close()

        'DESCryptoServiceProviderオブジェクトの作成
        Dim des As New System.Security.Cryptography.DESCryptoServiceProvider

        '共有キーと初期化ベクタを決定
        'パスワードをバイト配列にする
        Dim bytesKey As Byte() = System.Text.Encoding.UTF8.GetBytes(key)
        '共有キーと初期化ベクタを設定
        des.Key = ResizeBytesArray(bytesKey, des.Key.Length)
        des.IV = ResizeBytesArray(bytesKey, des.IV.Length)

        '暗号化されたファイルの保存先
        Dim outFileName As String = fileName + ".enc"
        '暗号化されたファイルを書き出すためのFileStream
        Dim fsOut As New System.IO.FileStream(outFileName,
            System.IO.FileMode.Create, System.IO.FileAccess.Write)
        'DES暗号化オブジェクトの作成
        Dim desdecrypt As System.Security.Cryptography.ICryptoTransform =
            des.CreateEncryptor()
        '書き込むためのCryptoStreamの作成
        Dim cryptStreem As New System.Security.Cryptography.CryptoStream(
            fsOut, desdecrypt,
            System.Security.Cryptography.CryptoStreamMode.Write)
        '書き込む
        cryptStreem.Write(bytesIn, 0, bytesIn.Length)
        '閉じる
        cryptStreem.Close()
        fsOut.Close()

    End Sub

    Public Sub cmn_decryptFile(ByVal fileName As String, ByVal key As String)




        'DESCryptoServiceProviderオブジェクトの作成
        Dim des As New System.Security.Cryptography.DESCryptoServiceProvider

        '共有キーと初期化ベクタを決定
        'パスワードをバイト配列にする
        Dim bytesKey As Byte() = System.Text.Encoding.UTF8.GetBytes(key)
        '共有キーと初期化ベクタを設定
        des.Key = ResizeBytesArray(bytesKey, des.Key.Length)
        des.IV = ResizeBytesArray(bytesKey, des.IV.Length)

        '暗号化されたファイルを読み込むためのFileStream
        Dim fsIn As New System.IO.FileStream(fileName,
            System.IO.FileMode.Open, System.IO.FileAccess.Read)
        'DES復号化オブジェクトの作成
        Dim desdecrypt As System.Security.Cryptography.ICryptoTransform =
            des.CreateDecryptor()
        '読み込むためのCryptoStreamの作成
        Dim cryptStreem As New System.Security.Cryptography.CryptoStream(
            fsIn, desdecrypt,
            System.Security.Cryptography.CryptoStreamMode.Read)

        '復号化されたファイルの保存先
        Dim outFileName As String
        If fileName.ToLower().EndsWith(".enc") Then
            outFileName = fileName.Substring(0, fileName.Length - 4)
        Else
            outFileName = fileName + ".dec"
        End If '復号化されたファイルを書き出すためのFileStream
        Dim fsOut As New System.IO.FileStream(outFileName,
            System.IO.FileMode.Create, System.IO.FileAccess.Write)

        '復号化されたデータを書き出す
        Dim bs(1024) As Byte
        Dim readLen As Integer
        Do
            readLen = cryptStreem.Read(bs, 0, bs.Length)
            If readLen > 0 Then
                fsOut.Write(bs, 0, readLen)
            End If
        Loop While (readLen > 0)

        '閉じる
        cryptStreem.Close()
        fsIn.Close()
        fsOut.Close()

    End Sub



#End Region

#Region "ファイル分割　結合"

    '分割
    'SplitFile("example.txt", 1024 * 1024) ' 1MBごとに分割
    '結合
    'Dim files As New List(Of String) From {
    '"example.txt.000",
    '"example.txt.001",
    '"example.txt.002"
    '}
    'MergeFiles("merged_example.txt", files)


    Sub SplitFile(filePath As String, chunkSize As Integer, arr As ArrayList)
        Dim buffer(chunkSize - 1) As Byte
        Dim fileIndex As Integer = 0

        arr.Clear()

        Using inputFile As FileStream = New FileStream(filePath, FileMode.Open, FileAccess.Read)
            While inputFile.Position < inputFile.Length
                Dim bytesRead As Integer = inputFile.Read(buffer, 0, chunkSize)
                Dim chunkFileName As String = $"{filePath}.{fileIndex:D3}"
                Using outputFile As FileStream = New FileStream(chunkFileName, FileMode.Create, FileAccess.Write)
                    outputFile.Write(buffer, 0, bytesRead)
                End Using
                arr.Add(chunkFileName)
                fileIndex += 1
            End While
        End Using
    End Sub

    Sub MergeFiles(outputFilePath As String, inputFiles As List(Of String))
        Using outputFile As FileStream = New FileStream(outputFilePath, FileMode.Create, FileAccess.Write)
            For Each inputFile In inputFiles
                Using inputFileStream As FileStream = New FileStream(inputFile, FileMode.Open, FileAccess.Read)
                    inputFileStream.CopyTo(outputFile)
                End Using
            Next
        End Using
    End Sub


#End Region

#Region ""

    Public Function cmn_fileMD5(path As String) As String

        Dim result As New System.Text.StringBuilder()

        Try

            Dim fs As New System.IO.FileStream(path,
                                               System.IO.FileMode.Open,
                                               System.IO.FileAccess.Read,
                                               System.IO.FileShare.Read)

            'MD5CryptoServiceProviderオブジェクトを作成 
            Dim md5 As New System.Security.Cryptography.MD5CryptoServiceProvider()
            'または、次のようにもできる 
            'Dim md5 As System.Security.Cryptography.MD5 = _
            '    System.Security.Cryptography.MD5.Create()

            'ハッシュ値を計算する 
            Dim bs As Byte() = md5.ComputeHash(fs)

            'リソースを解放する
            md5.Clear()
            'ファイルを閉じる 
            fs.Close()

            'byte型配列を16進数の文字列に変換 
            For Each b As Byte In bs
                result.Append(b.ToString("x2"))
            Next



        Catch ex As Exception

        End Try

        Return result.ToString


    End Function


#End Region

#Region "text 出力"

    '====================================================================
    '	ログ
    '====================================================================
    Public Sub cmn_textOut(path As String, msg As String)

        Dim dt As New MyDateTime

        dt.init()

        Try
            Using writer As New StreamWriter(path, True, System.Text.Encoding.GetEncoding("Shift_JIS"))
                writer.WriteLine(dt.myDate.ToString("yyyy/MM/dd HH:mm ") + msg)
            End Using
        Catch e As Exception

        End Try

    End Sub

#End Region

End Module
