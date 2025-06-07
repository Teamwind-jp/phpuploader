Public Class MyDateTime

    Implements ICloneable


    Private dt As DateTime
    Private finit As Boolean = False

    Public Sub New()
        finit = False
    End Sub

    Public Function Clone() As Object Implements ICloneable.Clone
        Return MemberwiseClone()
    End Function

    Sub clear()
        finit = False
    End Sub

    '指定日で初期化
    Function init(year As Integer, momth As Integer, day As Integer) As Boolean

        Try
            dt = New Date(year, momth, day)
            finit = True
        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

    Sub init()
        dt = Now
        finit = True
    End Sub

    '日付セット済みか
    Function isInit() As Boolean
        Return finit
    End Function

    Function myDate() As DateTime
        Return dt
    End Function

    Function year() As Integer
        If isInit() = False Then
            Return 0
        End If
        Return dt.Year
    End Function
    Function month() As Integer
        If isInit() = False Then
            Return 0
        End If
        Return dt.Month
    End Function
    Function day() As Integer
        If isInit() = False Then
            Return 0
        End If
        Return dt.Day
    End Function
    Function hour() As Integer
        If isInit() = False Then
            Return -1
        End If
        Return dt.Hour
    End Function

    Sub setHour(h As Integer)
        dt = DateTime.ParseExact(dt.ToString("yyyy/MM/dd " + h.ToString("00") + ":mm:ss"), "yyyy/MM/dd HH:mm:ss", Nothing)
    End Sub
    Sub setMinute(m As Integer)
        dt = DateTime.ParseExact(dt.ToString("yyyy/MM/dd HH:" + m.ToString("00") + ":ss"), "yyyy/MM/dd HH:mm:ss", Nothing)
    End Sub

    Sub addDays(days As Integer)
        dt = dt.AddDays(days)
    End Sub
    Function toFormatString(Optional fmt As String = "yyyy/MM/dd HH:mm:ss") As String
        Return dt.ToString(fmt)
    End Function


End Class
