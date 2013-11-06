Option Explicit On

Imports System.Drawing

Public Class MyMangaInfo
    Public Property last_chapter_read As Object
    Public Property new_chapter_to_read As Object
    Public Property manga As MyMangaDetails
    Public Property latest_chapter As MyMangaLatestChapter
    Public Property ID As String

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

    Public ReadOnly Property IsNew(ByVal daysOld As Integer) As Boolean
        Get
            If Format(Today.AddDays(Val("-" & daysOld)), "dd-MM-yyyy") <= LastChapterDate Then
                Return True
            Else
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


End Class
