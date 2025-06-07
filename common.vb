'Subfunctions. サブ関数群
'   (c)Teamwind japan h.hayashi

Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Windows.Forms.AxHost
Module common

#Region "date"

    Public Function cmn_datecmp(ByVal d1 As MyDateTime, ByVal d2 As MyDateTime) As Integer
        Return d1.myDate.CompareTo(d2.myDate)
    End Function

#End Region

#Region "random"

    '====================================================================
    '	Random Value. ランダム値 min-max
    '====================================================================
    Function cmn_getRand(min As Integer, max As Integer) As Integer

        'Set random values ​​to a byte array of the same size as an Int32.
        'Int32と同じサイズのバイト配列にランダムな値を設定する
        Dim bs As Byte() = New Byte(3) {}
        Dim rng As New System.Security.Cryptography.RNGCryptoServiceProvider()
        rng.GetBytes(bs)

        'Convert to Int32. Int32に変換する
        Return min + Math.Abs(System.BitConverter.ToInt32(bs, 0) Mod (max - min + 1))

    End Function


    'String. 文字列
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

#Region "Splitting and merging files. ファイル分割　結合"

    'Split. 分割
    'SplitFile("example.txt", 1024 * 1024) ' 1MBごとに分割
    'join. 結合
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

#Region "MD5 "

    Public Function cmn_fileMD5(path As String) As String

        Dim result As New System.Text.StringBuilder()

        Try

            Dim fs As New System.IO.FileStream(path,
                                               System.IO.FileMode.Open,
                                               System.IO.FileAccess.Read,
                                               System.IO.FileShare.Read)

            'Create an MD5CryptoServiceProvider object. MD5CryptoServiceProviderオブジェクトを作成 
            Dim md5 As New System.Security.Cryptography.MD5CryptoServiceProvider()

            'Calculate the hash value. ハッシュ値を計算する 
            Dim bs As Byte() = md5.ComputeHash(fs)

            'Freeing up resources. リソースを解放する
            md5.Clear()
            'Close File. ファイルを閉じる 
            fs.Close()

            'Convert a byte array to a hexadecimal string. byte型配列を16進数の文字列に変換 
            For Each b As Byte In bs
                result.Append(b.ToString("x2"))
            Next

        Catch ex As Exception

        End Try

        Return result.ToString


    End Function


#End Region

#Region "Text Output. text 出力"

    '====================================================================
    '	Text Output
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
