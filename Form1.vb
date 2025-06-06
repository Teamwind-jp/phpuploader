Imports System.ComponentModel
Imports System.Security.Policy
Imports System.Threading
Imports System.Windows.Forms.AxHost

Public Class Form1

#Region "my data"

    '入力フォルダ
    Private txtpath() As TextBox

    '監視フラグ
    Dim fwatch As Boolean = False

    'copy指示
    'タイマーで時刻監視
    'ただし開始ボタンで即on
    Dim fgo As Boolean = False

#End Region

#Region "デリゲートs"

    '============================================================
    '   go signe
    '============================================================
    Dim my_dlg_getGo As New pt_dlg_getGo(AddressOf _dlg_getGo)
    Delegate Function pt_dlg_getGo() As Boolean
    Public Function _dlg_getGo() As Boolean
        Return fgo
    End Function

    Dim my_dlg_setGo As New pt_dlg_setGo(AddressOf _dlg_setGo)
    Delegate Sub pt_dlg_setGo(b As Boolean)
    Public Sub _dlg_setGo(b As Boolean)
        fgo = b
    End Sub


    '============================================================
    '   バッチ時刻
    '   ComboBox1.SelectedIndex  
    '============================================================
    Dim my_dlg_ComboBox1_SelectedIndex As New pt_dlg_ComboBox1_SelectedIndex(AddressOf _dlg_ComboBox1_SelectedIndex)
    Delegate Function pt_dlg_ComboBox1_SelectedIndex() As Integer
    Public Function _dlg_ComboBox1_SelectedIndex() As Integer
        Return ComboBox1.SelectedIndex
    End Function

    '============================================================
    '   カットサイズ
    '   ComboBox2.SelectedIndex
    '============================================================
    Dim my_dlg_ComboBox2_SelectedIndex As New pt_dlg_ComboBox2_SelectedIndex(AddressOf _dlg_ComboBox2_SelectedIndex)
    Delegate Function pt_dlg_ComboBox2_SelectedIndex() As Integer
    Public Function _dlg_ComboBox2_SelectedIndex() As Integer
        Return ComboBox2.SelectedIndex
    End Function

    '============================================================
    '   パスワード
    '   txtPsw.Text
    '============================================================
    Dim my_dlg_txtPsw_Text As New pt_dlg_txtPsw_Text(AddressOf _dlg_txtPsw_Text)
    Delegate Function pt_dlg_txtPsw_Text() As String
    Public Function _dlg_txtPsw_Text() As String
        Return txtPsw.Text
    End Function

    '============================================================
    '   Timer
    '============================================================
    Dim my_dlg_Timer1_Enabled As New pt_dlg_Timer1_Enabled(AddressOf _dlg_Timer1_Enabled)
    Delegate Sub pt_dlg_Timer1_Enabled(b As Boolean)
    Public Sub _dlg_Timer1_Enabled(b As Boolean)
        Timer1.Enabled = b
    End Sub


    '============================================================
    '   開始ボタン変更
    '============================================================
    Dim my_dlg_Button1 As New pt_dlg_Button1(AddressOf _dlg_Button1)
    Delegate Sub pt_dlg_Button1(str As String)
    Public Sub _dlg_Button1(str As String)
        Button1.Text = str
    End Sub

    '============================================================
    '   メッセージラベル変更
    '============================================================
    Dim my_dlg_l4 As New pt_dlg_Button1(AddressOf _dlg_l4)
    Delegate Sub pt_dlg_l4(str As String)
    Public Sub _dlg_l4(str As String)
        lblmsg.Text = str
    End Sub


#End Region


#Region "メインスレッド"

    'メインスレッド
    Private Function threadUpload(ByVal worker As System.ComponentModel.BackgroundWorker, ByVal e As System.ComponentModel.DoWorkEventArgs) As Long

        Do

            Thread.Sleep(10 * 1000)

            'go指示ありか
            Dim bgo As Boolean = Me.Invoke(my_dlg_getGo, New Object() {})

            If bgo = False Then
                Continue Do
            End If

            'msg out
            Me.Invoke(my_dlg_Button1, New Object() {"処理中"})

            'zip and cutワークフォルダ作成
            Dim mywork = My.Application.Info.DirectoryPath + "\temp"
            Try
                Dim di As System.IO.DirectoryInfo = System.IO.Directory.CreateDirectory(mywork)
            Catch ex As Exception
                Continue Do
            End Try

            '処理済みzipファイル名格納用
            Dim z() As String
            Dim zs As Integer = 0

            '指定フォルダ毎に処理する
            For i = 0 To g_targetPaths - 1

                '対象フォルダからzip名決定
                Dim zipname As String = IO.Path.GetFileName(IO.Path.GetDirectoryName(g_targetPath(i) + "\"))
                If zipname.Length > 0 Then
                    'すでにある場合は、uploadすると上書きされるので別名にしておく
                    Dim b As Boolean = False
                    For j = 0 To zs - 1
                        If z(j) = zipname Then
                            b = True
                            Exit For
                        End If
                    Next
                    If b = True Then
                        'すでにあるので「_n」を付ける
                        zipname = zipname + "_" + i.ToString
                    End If
                    '↑の判定用に名前保管
                    ReDim Preserve z(zs + 1)
                    z(zs) = zipname
                    zs += 1

                    'zip開始msg
                    Me.Invoke(my_dlg_l4, New Object() {g_targetPath(i) + " zipping"})
                    '前回work内zip削除
                    Try
                        Kill(mywork + "\" + zipname + ".zip")
                    Catch ex As Exception
                    End Try
                    'zip
                    If zip_createZip(g_targetPath(i), mywork + "\" + zipname + ".zip", g_psw) = False Then
                        '失敗
                        cmn_textOut(My.Application.Info.DirectoryPath + "\log.txt", g_targetPath(i) + " zip失敗")
                        Continue For
                    End If

                    '送信準備

                    'zip md5取得
                    Dim zipmd5 = cmn_fileMD5(mywork + "\" + zipname + ".zip")
                    cmn_textOut(My.Application.Info.DirectoryPath + "\log.txt", g_targetPath(i) + " zip成功 MD5=" + zipmd5)

                    'カットサイズ
                    Dim sepsize As Integer = g_sepSize * 1024 * 1024

                    '分割後の送信ファイル名一覧保管
                    Dim arrfiles As New ArrayList
                    arrfiles.Clear()
                    arrfiles.Add(mywork + "\" + zipname + ".zip")

                    'zipを指定サイズに分割

                    'ファイルのサイズを取得
                    Dim fi As New System.IO.FileInfo(mywork + "\" + zipname + ".zip")
                    Dim l As Long = fi.Length
                    'カットサイズ以上は分割
                    If sepsize > 0 And l > sepsize Then
                        '送信ファイル名一覧はSplitFile内で再セットされる
                        SplitFile(mywork + "\" + zipname + ".zip", sepsize, arrfiles)
                    End If

                    '分割ファイルを順に送信
                    For j = 0 To arrfiles.Count - 1

                        'このカットファイルのmd5取得
                        Dim md5 = cmn_fileMD5(arrfiles.Item(j))
                        'phpに渡すprm生成
                        Dim prm = zipname + ".zip." + j.ToString("000") + "," + j.ToString("000") + "," + (arrfiles.Count - 1).ToString + "," + md5 + "," + zipmd5
                        'msg
                        Me.Invoke(my_dlg_l4, New Object() {arrfiles.Item(j) + " sending"})

                        Try
                            My.Computer.Network.UploadFile(arrfiles.Item(j),
                            g_url + "?prm=" + prm,
                            "username", "password",
                            True, 60 * 1000, FileIO.UICancelOption.DoNothing)
                            cmn_textOut(My.Application.Info.DirectoryPath + "\log.txt", "送信OK " + arrfiles.Item(j))
                        Catch ex As Exception
                            cmn_textOut(My.Application.Info.DirectoryPath + "\log.txt", "送信NG " + arrfiles.Item(j) + " " + ex.Message)
                        End Try


                    Next

                End If

            Next

            'go消す 次のタイミング待ち状態にする
            Me.Invoke(my_dlg_setGo, New Object() {False})
            'ボタンも戻す
            Me.Invoke(my_dlg_Button1, New Object() {"開始"})


        Loop

    End Function

    'メインスレッド起動
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        ' BackgroundWorkerの取得(スレッドを作成したオブジェクト)
        Dim objWorker As System.ComponentModel.BackgroundWorker = CType(sender, System.ComponentModel.BackgroundWorker)

        'ここから別世界

        ' 時間のかかる裏で動かしたい処理
        e.Result = threadUpload(objWorker, e)

    End Sub

    '============================================================
    '   スレッド処理終了
    '============================================================
    Private Sub backgroundworker1_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        ' 最初に、例外がスローされた場合の処理
        If Not (e.Error Is Nothing) Then
            'MessageBox.Show(e.Error.Message)
        ElseIf e.Cancelled Then
            ' 次に、ユーザーが計算をキャンセルした場合の処理
        Else
            ' 正常に完了した場合の処理
        End If
    End Sub

#End Region


#Region "開始ボタン処理"

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        g_url = txtUrl.Text
        g_targetPaths = 0
        For Each a As TextBox In txtpath
            If System.IO.Directory.Exists(a.Text) Then
                ReDim Preserve g_targetPath(g_targetPaths + 1)
                g_targetPath(g_targetPaths) = a.Text
                g_targetPaths += 1
            Else
            End If
        Next
        If g_url.Length = 0 Then
            MessageBox.Show("url指定無し", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If
        If g_targetPaths = 0 Then
            MessageBox.Show("フォルダ指定無し", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If

        If fwatch = True Then
            If MessageBox.Show("中断する？", Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = Windows.Forms.DialogResult.Yes Then
                fwatch = False
                Button1.Text = "開始"
                Return
            Else
                '継続
                Return
            End If
        Else
            If MessageBox.Show("開始する？", Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = Windows.Forms.DialogResult.Yes Then
            Else
                '中断のまま
                Return
            End If
        End If

        '入力内容書き込み
        g_sepSize = ComboBox2.SelectedIndex
        g_psw = txtPsw.Text
        ini_write()

        '監視timer開始
        dtbk.clear()
        fwatch = True

        Return


    End Sub

#End Region

#Region "onload"

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        '入力box
        txtpath = New System.Windows.Forms.TextBox() {TextBox2, TextBox3, TextBox4, TextBox5, TextBox6}

        '時刻指定
        ComboBox1.SelectedIndex = 24

        '分割サイズ
        Dim i As Integer
        For i = 0 To 1024
            ComboBox2.Items.Add(i.ToString)
        Next
        ComboBox2.SelectedIndex = g_sepSize

        '前回値取得
        ini_read()

        '前回値セット
        txtUrl.Text = g_url
        txtPsw.Text = g_psw
        For i = 0 To g_targetPaths - 1
            If i > 4 Then
                Exit For
            End If
            txtpath(i).Text = g_targetPath(i)
        Next

        '処理開始フラグ
        fwatch = False

        'バッチ監視処理開始
        Timer1.Enabled = True

        'copyスレ開始
        BackgroundWorker1.RunWorkerAsync()

    End Sub


#End Region

#Region "timer処理"

    '現在時刻用
    Private dt As New MyDateTime

    '前回処理日時
    Private dtbk As New MyDateTime

    'こいつは止めない
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick

        If fwatch = False Then
            '監視不要
            Return
        End If

        If fgo = True Then
            'copy中
            Return
        End If

        '現在時刻取得
        dt.init()

        'バッチ開始時刻
        Dim h As Integer = ComboBox1.SelectedIndex
        If h < 0 Or h > 23 Then
            '即時開始指示
            fgo = True
            '監視止める
            fwatch = False
            Button1.Text = "処理中"
            Return
        End If

        'バッチ時刻判定
        If dtbk.isInit = False Then
            'バッチ初回は強制実行 即時処理

            '次回処理日をセット
            '完了後　本日指定時刻が過ぎてなければ指定時刻に再実行するので日付移動はしない
            dtbk = dt
            If dt.hour < h Then
                '今日もう一回
            Else
                '過ぎたから明日
                dtbk.addDays(1)
            End If
            '指定時刻
            dtbk.setHour(h)
            dtbk.setMinute(0)
            '開始指示
            fgo = True
            '止めない
            Button1.Text = "処理中"
            Return
        End If

        '時間チェック
        If cmn_datecmp(dt, dtbk) < 0 Then
            Button1.Text = "次回" + dtbk.day.ToString + "日" + dtbk.hour.ToString + "時"
            Return
        End If

        'ここまできたら開始
        '次回処理日をセット
        dtbk = dt
        dtbk.addDays(1)
        dtbk.setHour(h)
        dtbk.setMinute(0)
        '即時
        fgo = True
        '止めない
        Button1.Text = "処理中"

    End Sub

#End Region

#Region "d&d処理"
    Private Sub TextBox2_DragDrop(sender As Object, e As DragEventArgs) Handles TextBox2.DragDrop, TextBox3.DragDrop, TextBox4.DragDrop, TextBox5.DragDrop, TextBox6.DragDrop

        'ドラッグされたファイル・フォルダのパスを格納します。
        Dim strFileName As String() = CType(e.Data.GetData(DataFormats.FileDrop, False), String())

        'ディレクトリの存在確認を行い、ある場合にのみ、
        'テキストボックスにパスを表示します。
        '（この処理でファイルを対象外にしています。）
        If System.IO.Directory.Exists(strFileName(0).ToString) = True Then
            sender.Text = strFileName(0).ToString
            'Me.TextBox2.Text = strFileName(0).ToString
        End If
    End Sub

    Private Sub TextBox2_DragEnter(sender As Object, e As DragEventArgs) Handles TextBox2.DragEnter, TextBox3.DragEnter, TextBox4.DragEnter, TextBox5.DragEnter, TextBox6.DragEnter
        'ファイル形式の場合のみ、ドラッグを受け付けます。
        If e.Data.GetDataPresent(DataFormats.FileDrop) = True Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If


    End Sub




#End Region

End Class
