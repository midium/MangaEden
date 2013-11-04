Option Explicit On

<Serializable()> _
Public Class AppSettings
    Public Property Language As Integer
    Public Property DownloadFolder As String
    Public Property DownloadMode As Integer
    Public Property AutomaticLogin As Integer
    Public Property MyMangaOnTop As Integer
    Public Property UserName As String
    Public Property Password As String
    Public Property OrderBy As Integer
    Public Property NewChapterDays As Integer
End Class
