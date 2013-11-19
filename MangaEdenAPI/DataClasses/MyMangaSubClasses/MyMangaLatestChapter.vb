Option Explicit On

Public Class MyMangaLatestChapter
    Public Property [date] As Double
    Public Property id As String
    Public Property number As Integer
    Public Property title As String

    Public ReadOnly Property LastChapterDate As String
        Get
            Return _dateTimeRoutines.UnixTimeStamp_To_Date([date], "dd-MM-yyyy")
        End Get
    End Property
End Class
