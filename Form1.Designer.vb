<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.ComboBox2 = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtPsw = New System.Windows.Forms.TextBox()
        Me.lblmsg = New System.Windows.Forms.Label()
        Me.TextBox6 = New System.Windows.Forms.TextBox()
        Me.TextBox5 = New System.Windows.Forms.TextBox()
        Me.TextBox4 = New System.Windows.Forms.TextBox()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtUrl = New System.Windows.Forms.TextBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.SuspendLayout()
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(226, 272)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(38, 12)
        Me.Label7.TabIndex = 36
        Me.Label7.Text = "MByte"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(39, 272)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(45, 12)
        Me.Label6.TabIndex = 35
        Me.Label6.Text = "cut size"
        '
        'ComboBox2
        '
        Me.ComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox2.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ComboBox2.FormattingEnabled = True
        Me.ComboBox2.Location = New System.Drawing.Point(99, 263)
        Me.ComboBox2.Name = "ComboBox2"
        Me.ComboBox2.Size = New System.Drawing.Size(121, 27)
        Me.ComboBox2.TabIndex = 34
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(39, 304)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(43, 12)
        Me.Label5.TabIndex = 33
        Me.Label5.Text = "zip psw"
        '
        'txtPsw
        '
        Me.txtPsw.AllowDrop = True
        Me.txtPsw.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPsw.Location = New System.Drawing.Point(99, 296)
        Me.txtPsw.Margin = New System.Windows.Forms.Padding(4)
        Me.txtPsw.MaxLength = 100
        Me.txtPsw.Name = "txtPsw"
        Me.txtPsw.Size = New System.Drawing.Size(392, 23)
        Me.txtPsw.TabIndex = 32
        Me.txtPsw.Tag = "1"
        '
        'lblmsg
        '
        Me.lblmsg.Location = New System.Drawing.Point(42, 378)
        Me.lblmsg.Name = "lblmsg"
        Me.lblmsg.Size = New System.Drawing.Size(449, 21)
        Me.lblmsg.TabIndex = 31
        Me.lblmsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TextBox6
        '
        Me.TextBox6.AllowDrop = True
        Me.TextBox6.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBox6.Location = New System.Drawing.Point(37, 228)
        Me.TextBox6.Margin = New System.Windows.Forms.Padding(4)
        Me.TextBox6.MaxLength = 1024
        Me.TextBox6.Name = "TextBox6"
        Me.TextBox6.Size = New System.Drawing.Size(454, 23)
        Me.TextBox6.TabIndex = 30
        Me.TextBox6.Tag = "1"
        '
        'TextBox5
        '
        Me.TextBox5.AllowDrop = True
        Me.TextBox5.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBox5.Location = New System.Drawing.Point(37, 197)
        Me.TextBox5.Margin = New System.Windows.Forms.Padding(4)
        Me.TextBox5.MaxLength = 1024
        Me.TextBox5.Name = "TextBox5"
        Me.TextBox5.Size = New System.Drawing.Size(454, 23)
        Me.TextBox5.TabIndex = 29
        Me.TextBox5.Tag = "1"
        '
        'TextBox4
        '
        Me.TextBox4.AllowDrop = True
        Me.TextBox4.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBox4.Location = New System.Drawing.Point(37, 166)
        Me.TextBox4.Margin = New System.Windows.Forms.Padding(4)
        Me.TextBox4.MaxLength = 1024
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.Size = New System.Drawing.Size(454, 23)
        Me.TextBox4.TabIndex = 28
        Me.TextBox4.Tag = "1"
        '
        'TextBox3
        '
        Me.TextBox3.AllowDrop = True
        Me.TextBox3.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBox3.Location = New System.Drawing.Point(37, 135)
        Me.TextBox3.Margin = New System.Windows.Forms.Padding(4)
        Me.TextBox3.MaxLength = 1024
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.Size = New System.Drawing.Size(454, 23)
        Me.TextBox3.TabIndex = 27
        Me.TextBox3.Tag = "1"
        '
        'TextBox2
        '
        Me.TextBox2.AllowDrop = True
        Me.TextBox2.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBox2.Location = New System.Drawing.Point(37, 104)
        Me.TextBox2.Margin = New System.Windows.Forms.Padding(4)
        Me.TextBox2.MaxLength = 1024
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(454, 23)
        Me.TextBox2.TabIndex = 26
        Me.TextBox2.Tag = "1"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(41, 88)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(145, 12)
        Me.Label3.TabIndex = 25
        Me.Label3.Text = "backup folder ドラッグドロップ"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(39, 352)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(55, 12)
        Me.Label2.TabIndex = 24
        Me.Label2.Text = "start hour"
        '
        'ComboBox1
        '
        Me.ComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox1.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Items.AddRange(New Object() {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "即時"})
        Me.ComboBox1.Location = New System.Drawing.Point(97, 343)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(121, 27)
        Me.ComboBox1.TabIndex = 23
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(41, 28)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(252, 12)
        Me.Label1.TabIndex = 22
        Me.Label1.Text = "upload url phpまで「http://hoge.com/receive.php」"
        '
        'txtUrl
        '
        Me.txtUrl.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtUrl.Location = New System.Drawing.Point(37, 43)
        Me.txtUrl.MaxLength = 1024
        Me.txtUrl.Name = "txtUrl"
        Me.txtUrl.Size = New System.Drawing.Size(454, 26)
        Me.txtUrl.TabIndex = 21
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button1.Location = New System.Drawing.Point(223, 340)
        Me.Button1.Margin = New System.Windows.Forms.Padding(2)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(268, 31)
        Me.Button1.TabIndex = 20
        Me.Button1.Text = "開始"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Timer1
        '
        '
        'BackgroundWorker1
        '
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(534, 423)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.ComboBox2)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.txtPsw)
        Me.Controls.Add(Me.lblmsg)
        Me.Controls.Add(Me.TextBox6)
        Me.Controls.Add(Me.TextBox5)
        Me.Controls.Add(Me.TextBox4)
        Me.Controls.Add(Me.TextBox3)
        Me.Controls.Add(Me.TextBox2)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.ComboBox1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtUrl)
        Me.Controls.Add(Me.Button1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form1"
        Me.Text = "php連動アップロード"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label7 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents ComboBox2 As ComboBox
    Friend WithEvents Label5 As Label
    Friend WithEvents txtPsw As TextBox
    Friend WithEvents lblmsg As Label
    Friend WithEvents TextBox6 As TextBox
    Friend WithEvents TextBox5 As TextBox
    Friend WithEvents TextBox4 As TextBox
    Friend WithEvents TextBox3 As TextBox
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents ComboBox1 As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents txtUrl As TextBox
    Friend WithEvents Button1 As Button
    Friend WithEvents Timer1 As Timer
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
End Class
