Option Explicit On

Public Class MangaChaptersDetails
    Public Property Number As Integer
    Public Property chapter_date As Double
    Public Property Title As String
    Public Property ChapterID As String
    Public ReadOnly Property ChapterDate As String
        Get
            Return UnixTimeStamp_To_Date(chapter_date, "dd-MM-yyyy")
        End Get
    End Property
End Class
