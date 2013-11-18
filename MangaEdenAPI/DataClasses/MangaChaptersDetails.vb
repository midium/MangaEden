Option Explicit On

Imports SettingsManager

Public Class MangaChaptersDetails
    Private _settings As AppManager

    Public Property Number As Integer
    Public Property chapter_date As Double
    Public Property Title As String
    Public Property ChapterID As String
    Public ReadOnly Property ChapterDate As String
        Get
            Return UnixTimeStamp_To_Date(chapter_date, "dd-MM-yyyy")
        End Get
    End Property
    Public ReadOnly Property IsNew As Boolean
        Get
            Dim newDateLimit As Integer = 0 - _settings.NewChapterDays
            If (UnixTimeStamp_To_Date(chapter_date) >= Now().Date.AddDays(newDateLimit)) Then
                'It is within the day limits
                Return True
            Else
                'It is not within the day limits
                Return False
            End If
        End Get
    End Property

    Public Sub New()
        _settings = New AppManager
        _settings.LoadSettings()
    End Sub
End Class
