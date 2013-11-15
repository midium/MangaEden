Option Explicit On

Imports Newtonsoft.Json

Public Class MangaDetails
    Enum MangaStatus
        NotFinished = 2
        Finished = 1
    End Enum

    Public Property status As Integer
    Public Property description As String
    Public Property image As String
    Public Property chapters() As List(Of List(Of Object))
    Public Property released As Object
    Public Property artist_kw As String()
    Public Property updatedKeywords As Boolean
    Public Property startsWith As String
    Public Property hits As Integer
    Public Property author_kw As String()
    Public Property language As Integer
    Public Property author As String
    Public Property last_chapter_date As Object
    Public Property artist As String
    Public Property chapters_len As Integer
    Public Property created As Double
    Public Property [alias] As String
    'Public Property aka-alias as string()
    Public Property categories As String()
    Public Property title As String
    Public Property aka As String()
    Public Property type As Integer
    Public Property title_kw As String()

    Public Overrides Function ToString() As String
        Return title
    End Function

    Public Function ChapterDate(ByVal ChapterIndex As Integer, ByVal sFormat As String) As String
        Dim result As String = ""

        If chapters.Count > 0 Then
            result = UnixTimeStamp_To_Date(DirectCast(chapters(ChapterIndex)(1), Double), sFormat)
        End If

        Return result
    End Function

    Public Function ChapterNumber(ByVal ChapterIndex As Integer) As Integer
        Dim result As Integer = 0

        If chapters.Count > 0 Then
            result = chapters(ChapterIndex)(0)
        End If

        Return result
    End Function

    Public Function ChapterTitle(ByVal ChapterIndex As Integer) As String
        Dim result As String = ""

        If chapters.Count > 0 Then
            result = chapters(ChapterIndex)(2)
        End If

        Return result
    End Function

    Public Function ChapterID(ByVal ChapterIndex As Integer) As String
        Dim result As String = ""

        If chapters.Count > 0 Then
            result = chapters(ChapterIndex)(3)
        End If

        Return result
    End Function

End Class
