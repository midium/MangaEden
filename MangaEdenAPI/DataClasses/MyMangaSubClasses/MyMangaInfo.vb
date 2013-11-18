Option Explicit On

Imports System.Drawing
Imports SettingsManager

Public Class MyMangaInfo
    Public Property last_chapter_read As Object
    Public Property new_chapter_to_read As Object
    Public Property manga As MyMangaDetails
    Public Property latest_chapter As MyMangaLatestChapter
    Public Property ID As String

    Private _settings As AppManager

    Public Overrides Function ToString() As String
        If Not manga Is Nothing Then
            Return manga.title
        Else
            Return MyBase.ToString()
        End If
    End Function

    Public ReadOnly Property Title As String
        Get
            If Not manga Is Nothing Then
                Return manga.title
            Else
                Return ""
            End If
        End Get
    End Property

    Public ReadOnly Property IsNew As Boolean
        Get
            Dim newDateLimit As Integer = 0 - _settings.NewChapterDays
            If (UnixTimeStamp_To_Date(latest_chapter.date) >= Now().Date.AddDays(newDateLimit)) Then
                'It is within the day limits
                Return True
            Else
                'It is not within the day limits
                Return False
            End If
        End Get
    End Property

    Public ReadOnly Property LastChapterDate As String
        Get
            If Not latest_chapter Is Nothing Then
                Return latest_chapter.LastChapterDate
            Else
                Return ""
            End If

        End Get
    End Property

    Public Sub New()
        _settings = New AppManager
        _settings.LoadSettings()
    End Sub
End Class
