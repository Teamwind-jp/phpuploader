'using SharpZipLib from NuGet
'   (c)Teamwind japan h.hayashi

Module zip


    Public Function zip_createZip(ByVal szFromPath As String, topath As String, psw As String) As Boolean

        Dim fastZip As New ICSharpCode.SharpZipLib.Zip.FastZip()
        'Put empty folders in the archive. 空のフォルダも書庫に入れるか。デフォルトはfalse 
        fastZip.CreateEmptyDirectories = True
        'ZIP64を使うか。デフォルトはDynamicで、状況に応じてZIP64を使う 
        fastZip.UseZip64 = ICSharpCode.SharpZipLib.Zip.UseZip64.Dynamic
        'Set Password. パスワードを設定するには次のようにする 
        fastZip.Password = psw

        Try
            fastZip.CreateZip(topath, szFromPath, True, Nothing)
        Catch ex As Exception
            Return False
        End Try


        Return True

    End Function

End Module
