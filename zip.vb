Module zip


    Public Function zip_createZip(ByVal szFromPath As String, topath As String, psw As String) As Boolean

        Dim fastZip As New ICSharpCode.SharpZipLib.Zip.FastZip()
        '空のフォルダも書庫に入れるか。デフォルトはfalse 
        fastZip.CreateEmptyDirectories = True
        'ZIP64を使うか。デフォルトはDynamicで、状況に応じてZIP64を使う 
        '（大きなファイルはZIP64でしか圧縮できないが、対応していないアーカイバもある） 
        fastZip.UseZip64 = ICSharpCode.SharpZipLib.Zip.UseZip64.Dynamic
        'パスワードを設定するには次のようにする 
        fastZip.Password = psw

        Try
            fastZip.CreateZip(topath, szFromPath, True, Nothing)
        Catch ex As Exception
            Return False
        End Try


        Return True

    End Function

End Module
