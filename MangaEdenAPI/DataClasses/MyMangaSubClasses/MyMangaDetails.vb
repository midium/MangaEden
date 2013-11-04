Option Explicit On

Public Class MyMangaDetails
    Public Property image As String
    Public Property baka As Boolean
    Public Property startsWith As String
    Public Property artist_kw As String()
    Public Property author As String
    Public Property chapters_len As Integer
    'Public Property [aka-alias] as string() 'This name format isn't supported
    Public Property updatedKeywords As Boolean
    Public Property type As Integer
    Public Property status As Integer
    Public Property description As String
    Public Property released As Integer
    Public Property categories As String()
    Public Property hits As Integer
    Public Property author_kw As String()
    Public Property language As Integer
    Public Property artist As String
    Public Property last_chapter_date As Double
    Public Property created As Double
    Public Property [alias] As String
    Public Property title As String
    Public Property aka As String()
    Public Property title_kw As String()

    Public Overrides Function ToString() As String
        Return title
    End Function
End Class
