Option Explicit On

Public Class Downloader
    Private WithEvents _client As CookieAwareWebClient = Nothing

    Public Sub New()
        _client = New CookieAwareWebClient
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        _client.Dispose()
    End Sub

    Public Function getWebPage(ByVal sPageUrl As String) As String
        Dim result As String = ""

        Try
            result = _client.DownloadString(sPageUrl)
        Catch ex As Exception

        End Try

        Return result
    End Function

    Public Function getMyMangaWebPage(ByVal sLoginUrl As String, ByVal sPageUrl As String) As String
        Dim result As String = ""

        Try
            result = _client.DownloadString(sLoginUrl)
            result = _client.DownloadString(sPageUrl)
        Catch ex As Exception

        End Try

        Return result
    End Function

    Public Function getImage(ByVal remoteImagePath As String, ByVal localImagePath As String) As Boolean
        Dim result As Boolean = False

        Try
            _client.DownloadFile(New Uri(remoteImagePath), localImagePath)

            result = True
        Catch ex As Exception

        End Try

        Return result
    End Function

End Class
