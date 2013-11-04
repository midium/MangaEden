Option Explicit On

Imports System.Net

Public Class Login
    Private WithEvents _client As WebClient = Nothing

    Public Sub New()
        _client = New WebClient
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        _client.Dispose()
    End Sub

    Public Function doLogin(ByVal sUrl As String) As String
        Return getPage(sUrl)
    End Function

    Public Function doLogout(ByVal sUrl As String) As String
        Return getPage(sUrl)
    End Function

    Private Function getPage(ByVal sUrl As String) As String
        Dim result As String = ""

        Try
            result = _client.DownloadString(sUrl)
        Catch ex As Exception
            result = ex.Message
        End Try        

        Return result
    End Function
End Class
