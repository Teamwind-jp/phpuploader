Imports System.Security.Policy

Module ini

#Region "globals"

    Public g_url As String = ""
    Public g_psw As String = ""
    Public g_sepSize As Integer = 10

    Public g_targetPath() As String
    Public g_targetPaths As Integer = 0

#End Region

#Region "ini io"

    '============================================================
    '   読み込む 
    '============================================================
    Public Function ini_read() As Boolean

        Dim sztemp As String

        g_targetPaths = 0

        Try

            Dim StreamReader = New System.IO.StreamReader(My.Application.Info.DirectoryPath + "\prof.ini", System.Text.Encoding.GetEncoding(932))

            Do While StreamReader.Peek() >= 0

                '一行読込
                sztemp = StreamReader.ReadLine()

                'up url
                If Left$(sztemp, 4) = "url=" Then
                    g_url = Mid$(sztemp, 5)
                End If

                If Left$(sztemp, 4) = "psw=" Then
                    g_psw = Mid$(sztemp, 5)
                End If

                If Left$(sztemp, 4) = "sep=" Then
                    g_sepSize = Val(Mid$(sztemp, 5))
                End If

                If Left$(sztemp, 5) = "path=" Then
                    sztemp = Mid$(sztemp, 6)
                    ReDim Preserve g_targetPath(g_targetPaths + 1)
                    g_targetPath(g_targetPaths) = sztemp
                    g_targetPaths += 1
                End If
            Loop


            StreamReader.Close()

        Catch ex As Exception

        End Try


    End Function

    '============================================================
    '   ファイルに書き込む 
    '============================================================
    Public Function ini_write() As Boolean

        Dim errcode As Integer
        Dim szbuf As String
        Dim szpath As String
        Dim i As Integer

        ini_write = False

        'EXEのパスを取得
        szpath = My.Application.Info.DirectoryPath + "\prof.ini"

        'ファイルを開く
        errcode = 0
        On Error GoTo err_open
        FileOpen(1, szpath, OpenMode.Output)
        If errcode <> 0 Then
            'なにもしない
            Exit Function
        End If

        PrintLine(1, "url=" & g_url)
        PrintLine(1, "psw=" & g_psw)
        PrintLine(1, "sep=" & g_sepSize.ToString)
        For i = 0 To g_targetPaths - 1
            PrintLine(1, "path=" & g_targetPath(i))
        Next


        'PrintLine(1, "Treeview=" & nTreeView.ToString)

        FileClose(1)

        ini_write = True

        Exit Function

err_open:
        errcode = 1
        Resume Next

    End Function


#End Region


End Module
