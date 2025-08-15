'   (c)Teamwind japan n.h
'
'   サーバーメンテナンスツール Windows Server Maintenance Tools
'

Imports System.ComponentModel
Imports System.Security.Policy
Imports System.Threading
Imports System.Windows.Forms.AxHost

Public Class Form1

#Region "my data"

    'zip folder 入力フォルダ
    Private txtpath() As TextBox

    'Processing start flag 監視フラグ
    Dim fwatch As Boolean = False

    'copy指示
    'タイマーで時刻監視
    'ただし開始ボタンで即on
    Dim fgo As Boolean = False

#End Region

#Region "delegates デリゲート共有"

	'============================================================
	'   go sign
	'============================================================
	Public Function _dlg_getGo() As Boolean
		If Me.InvokeRequired Then
			Return CType(Me.Invoke(New Func(Of Boolean)(AddressOf _dlg_getGo)), Boolean)
		Else
			Return fgo
		End If
	End Function

	Public Sub _dlg_setGo(value As Boolean)
		If Me.InvokeRequired Then
			Me.Invoke(New Action(Of Boolean)(AddressOf _dlg_setGo), value)
		Else
			fgo = value
		End If
	End Sub

	'============================================================
	'   Change start button str 開始ボタン変更
	'============================================================

	Public Sub _dlg_setButton1Text(value As String)
		If Button1.InvokeRequired Then
			Button1.Invoke(New Action(Of String)(AddressOf _dlg_setButton1Text), value)
		Else
			Button1.Text = value
		End If
	End Sub

	'============================================================
	'   Change message label メッセージラベル変更
	'============================================================
	Public Sub _dlg_setLabelText(value As String)
		If lblmsg.InvokeRequired Then
			lblmsg.Invoke(New Action(Of String)(AddressOf _dlg_setLabelText), value)
		Else
			lblmsg.Text = value
		End If
	End Sub

#End Region

#Region "Main Thread メインスレッド "

	'Main Thread メインスレッド 
	Private Function threadUpload(ByVal worker As System.ComponentModel.BackgroundWorker, ByVal e As System.ComponentModel.DoWorkEventArgs) As Long

        Do

            Thread.Sleep(10 * 1000)

			'Is there a go command? go指示有るか　
			Dim bgo As Boolean = _dlg_getGo()

			If bgo = False Then
                Continue Do
            End If

			'msg out
			_dlg_setButton1Text("処理中")

			'zip and cut Create a work folder. ワークフォルダ作成
			Dim mywork = My.Application.Info.DirectoryPath + "\temp"
            Try
                Dim di As System.IO.DirectoryInfo = System.IO.Directory.CreateDirectory(mywork)
            Catch ex As Exception
                Continue Do
            End Try

            'For storing the processed zip file name. 処理済みzipファイル名格納用
            Dim z() As String
            Dim zs As Integer = 0

            'Process each specified folder. 指定フォルダ毎に処理する
            For i = 0 To g_targetPaths - 1

                'Determine zip name from target folder. 対象フォルダからzip名決定
                Dim zipname As String = IO.Path.GetFileName(IO.Path.GetDirectoryName(g_targetPath(i) + "\"))
                If zipname.Length > 0 Then
                    'If it already exists, it will be overwritten when you upload it, so give it a different name.
                    'すでにある場合は、uploadすると上書きされるので別名にしておく
                    Dim b As Boolean = False
                    For j = 0 To zs - 1
                        If z(j) = zipname Then
                            b = True
                            Exit For
                        End If
                    Next
                    If b = True Then
                        'Since it already exists, add "_n". すでにあるので「_n」を付ける
                        zipname = zipname + "_" + i.ToString
                    End If
                    'Name is stored for the purpose of determining the above. ↑の判定用に名前保管
                    ReDim Preserve z(zs + 1)
                    z(zs) = zipname
                    zs += 1

					'zip start msg zip開始msg
					_dlg_setLabelText(g_targetPath(i) + " zipping")
					'Delete zip file from previous work 前回work内zip削除
					Try
                        Kill(mywork + "\" + zipname + ".zip")
                    Catch ex As Exception
                    End Try
                    'zip
                    If zip_createZip(g_targetPath(i), mywork + "\" + zipname + ".zip", g_psw) = False Then
                        'failure 失敗
                        cmn_textOut(My.Application.Info.DirectoryPath + "\log.txt", g_targetPath(i) + "failure zip")
                        Continue For
                    End If

                    'Prepare to send 送信準備

                    'zip md5 acquisition. zip md5取得
                    Dim zipmd5 = cmn_fileMD5(mywork + "\" + zipname + ".zip")
                    cmn_textOut(My.Application.Info.DirectoryPath + "\log.txt", g_targetPath(i) + "success zip MD5=" + zipmd5)

                    'Cut size カットサイズ
                    Dim sepsize As Integer = g_sepSize * 1024 * 1024

                    'Store list of file names after splitting. 分割後の送信ファイル名一覧保管
                    Dim arrfiles As New ArrayList
                    arrfiles.Clear()
                    arrfiles.Add(mywork + "\" + zipname + ".zip")

                    'Split zip into specified size. zipを指定サイズに分割

                    'Get the size of a file. ファイルのサイズを取得
                    Dim fi As New System.IO.FileInfo(mywork + "\" + zipname + ".zip")
                    Dim l As Long = fi.Length
                    'Divide anything over the cut size. カットサイズ以上は分割
                    If sepsize > 0 And l > sepsize Then
                        'The list of sending file names is reset in SplitFile(). 送信ファイル名一覧はSplitFile内で再セットされる
                        SplitFile(mywork + "\" + zipname + ".zip", sepsize, arrfiles)
                    End If

                    'Send split files in sequence. 分割ファイルを順に送信
                    For j = 0 To arrfiles.Count - 1

                        'Get the md5 of this cut file このカットファイルのmd5取得
                        Dim md5 = cmn_fileMD5(arrfiles.Item(j))
                        'Generate prm to pass to php. phpに渡すprm生成
                        Dim prm = zipname + ".zip." + j.ToString("000") + "," + j.ToString("000") + "," + (arrfiles.Count - 1).ToString + "," + md5 + "," + zipmd5
						'msg
						_dlg_setLabelText(arrfiles.Item(j) + " sending")

						Try
                            My.Computer.Network.UploadFile(arrfiles.Item(j),
                            g_url + "?prm=" + prm,
                            "username", "password",
                            True, 60 * 1000, FileIO.UICancelOption.DoNothing)
                            cmn_textOut(My.Application.Info.DirectoryPath + "\log.txt", "send OK " + arrfiles.Item(j))
                        Catch ex As Exception
                            cmn_textOut(My.Application.Info.DirectoryPath + "\log.txt", "send NG " + arrfiles.Item(j) + " " + ex.Message)
                        End Try


                    Next

                End If

            Next

            'log out
            cmn_textOut(My.Application.Info.DirectoryPath + "\log.txt", "done")
            'msg
            Dim dt As New MyDateTime
            dt.init()
			_dlg_setLabelText(dt.toFormatString + " done")

			'Erase go Wait for next timing. go消す 次のタイミング待ち状態にする
			_dlg_setGo(False)

			'Return the button. ボタンも戻す
			_dlg_setButton1Text("開始")


		Loop

    End Function

    'Main thread startup. メインスレッド起動
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        ' BackgroundWorkerの取得
        Dim objWorker As System.ComponentModel.BackgroundWorker = CType(sender, System.ComponentModel.BackgroundWorker)

        'ここから別世界

        ' 時間のかかる裏で動かしたい処理
        e.Result = threadUpload(objWorker, e)

    End Sub

    '============================================================
    '   Thread process finished.スレッド処理終了
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


#Region "Start button processing. 開始ボタン処理"

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
            MessageBox.Show("No URL specified. url指定無し", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If
        If g_targetPaths = 0 Then
            MessageBox.Show("No folder specified. フォルダ指定無し", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If

        If fwatch = True Then
            If MessageBox.Show("Abort? 中断する？", Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = Windows.Forms.DialogResult.Yes Then
                fwatch = False
                Button1.Text = "Start"
                Return
            Else
                '継続
                Return
            End If
        Else
            If MessageBox.Show("get started? 開始する？", Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = Windows.Forms.DialogResult.Yes Then
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

        'Input box array. 入力box配列化
        txtpath = New System.Windows.Forms.TextBox() {TextBox2, TextBox3, TextBox4, TextBox5, TextBox6}

        'time specification. 時刻指定
        ComboBox1.SelectedIndex = 24

        'Split Size 分割サイズ
        Dim i As Integer
        For i = 0 To 1024
            ComboBox2.Items.Add(i.ToString)
        Next
        ComboBox2.SelectedIndex = g_sepSize

        'Get previous value. 前回値取得
        ini_read()

        'Previous value set. 前回値セット
        txtUrl.Text = g_url
        txtPsw.Text = g_psw
        For i = 0 To g_targetPaths - 1
            If i > 4 Then
                Exit For
            End If
            txtpath(i).Text = g_targetPath(i)
        Next

        'Processing start flag. 処理開始フラグ
        fwatch = False

        'Batch monitoring process started. バッチ監視処理開始
        Timer1.Enabled = True

        'Copy thread start. copyスレ開始
        BackgroundWorker1.RunWorkerAsync()

    End Sub


#End Region

#Region "Timer processing. timer処理"

    'For current time. 現在時刻用
    Private dt As New MyDateTime

    'Next processing date and time. 次回処理日時
    Private dtbk As New MyDateTime

    'This won't stop. こいつは止めない
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick

        If fwatch = False Then
            'No monitoring required. 監視不要
            Return
        End If

        If fgo = True Then
            'copying in progress. copy中
            Return
        End If

        'Get current time. 現在時刻取得
        dt.init()

        'Batch Start Time. バッチ開始時刻
        Dim h As Integer = ComboBox1.SelectedIndex
        If h < 0 Or h > 23 Then
            'Immediate start instruction. 即時開始指示
            fgo = True
            'Stop monitoring. 監視止める
            fwatch = False
            Button1.Text = "Processing"
            Return
        End If

        'Batch time determination. バッチ時刻判定
        If dtbk.isInit = False Then
            'Force execution of first batch Immediate processing. バッチ初回は強制実行 即時処理

            'Set next processing date. 次回処理日をセット
            'After completion, if the specified time has not passed today,
            'it will be executed again at the specified time, so the date will not be moved.
            '完了後　本日指定時刻が過ぎてなければ指定時刻に再実行するので日付移動はしない
            dtbk = dt.Clone
            If dt.hour < h Then
                'One more time today. 今日もう一回
            Else
                'It's past so tomorrow. 過ぎたから明日
                dtbk.addDays(1)
            End If
            'Specified time. 指定時刻
            dtbk.setHour(h)
            dtbk.setMinute(0)
            'Start instructions. 開始指示
            fgo = True
            'Don't stop. 止めない
            Button1.Text = "Processing"
            Return
        End If

        'Time Check. 時間チェック
        If cmn_datecmp(dt, dtbk) < 0 Then
			Button1.Text = "next " + dtbk.day.ToString + "　" + dtbk.hour.ToString("00") + ":00"
			Return
        End If

        'Start here. ここまできたら開始
        'Set next processing date. 次回処理日をセット
        dtbk = dt.Clone
        dtbk.addDays(1)
        dtbk.setHour(h)
        dtbk.setMinute(0)
        'Immediately. 即時
        fgo = True

        Button1.Text = "Processing"

        'timer continuation. timerは継続

    End Sub

#End Region

#Region "d&d process. d&d処理"
    Private Sub TextBox2_DragDrop(sender As Object, e As DragEventArgs) Handles TextBox2.DragDrop, TextBox3.DragDrop, TextBox4.DragDrop, TextBox5.DragDrop, TextBox6.DragDrop

        'Stores the path of the dragged file/folder.
        'ドラッグされたファイル・フォルダのパスを格納します。
        Dim strFileName As String() = CType(e.Data.GetData(DataFormats.FileDrop, False), String())

        'The existence of the directory is checked, and only if it exists is the path displayed in the text box.
        'ディレクトリの存在確認を行い、ある場合にのみ、
        'テキストボックスにパスを表示します。
        '（この処理でファイルを対象外にしています。）
        If System.IO.Directory.Exists(strFileName(0).ToString) = True Then
            sender.Text = strFileName(0).ToString
            'Me.TextBox2.Text = strFileName(0).ToString
        End If
    End Sub

    Private Sub TextBox2_DragEnter(sender As Object, e As DragEventArgs) Handles TextBox2.DragEnter, TextBox3.DragEnter, TextBox4.DragEnter, TextBox5.DragEnter, TextBox6.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) = True Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If


    End Sub




#End Region

End Class
