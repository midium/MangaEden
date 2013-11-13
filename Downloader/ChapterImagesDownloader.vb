Option Explicit On

Imports System.Net
Imports SettingsManager
Imports System.IO

Public Class ChapterImagesDownloader
#Region "Declarations"
    Private WithEvents _webClient As CookieAwareWebClient = Nothing
    Private _chapterImages As String() = Nothing
    Private _settings As AppManager = Nothing
    Private _currentStatus As Status

    Private _imgBase As String = "http://cdn.mangaeden.com/mangasimg/{0}"
#End Region

#Region "Structure declarations"
    Public Structure Status
        Dim currentDownload As Integer
        Dim currentFileName As String
        Dim totalDownloads As Integer
    End Structure
#End Region

#Region "Events Declarations"
    Public Event CurrentDownload(ByVal sender As Object, ByVal DownloadStatus As Status)
    Public Event DownloadBegin(ByVal sender As Object, ByVal DownloadStatus As Status)
    Public Event DownloadEnd(ByVal sender As Object, ByVal DownloadStatus As Status)
    Public Event PageDownloadProgress(sender As Object, e As System.Net.DownloadProgressChangedEventArgs)
    Public Event PageDownloadStart(sender As Object)
    Public Event PageDownloadEnd(sender As Object, e As System.Net.DownloadDataCompletedEventArgs)
#End Region

#Region "Constructor / Distructor"
    Public Sub New(ByVal chapterImages As String())
        _chapterImages = chapterImages

        _settings = New AppManager()
        _settings.LoadSettings()

        _webClient = New CookieAwareWebClient()

        _currentStatus.currentDownload = 0
        _currentStatus.currentFileName = ""
        _currentStatus.totalDownloads = _chapterImages.GetLength(0)

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        _settings = Nothing
        _webClient.Dispose()
    End Sub
#End Region

#Region "Work functions"
    Private Function startDownload() As Boolean
        RaiseEvent DownloadBegin(Me, _currentStatus)

        Return True
    End Function

    Private Function downloadChapters() As Boolean

        For Each page As String In _chapterImages
            _currentStatus.currentDownload += 1
            _currentStatus.currentFileName = page

            Dim sourceFile As String = String.Format(_imgBase, page)
            Dim destinationFile As String = String.Format("{0}\{1}", _settings.DownloadFolder, extractFileName(page))

            RaiseEvent PageDownloadStart(Me)
            RaiseEvent CurrentDownload(Me, _currentStatus)
            Try
                If Not File.Exists(destinationFile) Then
                    _webClient.DownloadFile(New Uri(sourceFile), destinationFile)
                End If

            Catch ex As Exception
                Debug.Print(ex.Message)
            End Try

        Next

        Return True
    End Function

    Private Function endDownload() As Boolean
        _currentStatus.currentDownload = _currentStatus.totalDownloads
        _currentStatus.currentFileName = "Download Completed"
        RaiseEvent DownloadEnd(Me, _currentStatus)

        Return True
    End Function
#End Region

#Region "Web events"

    Private Sub _webClient_DownloadDataCompleted(sender As Object, e As System.Net.DownloadDataCompletedEventArgs) Handles _webClient.DownloadDataCompleted
        RaiseEvent PageDownloadEnd(sender, e)
    End Sub
    Private Sub _webClient_DownloadProgressChanged(sender As Object, e As System.Net.DownloadProgressChangedEventArgs) Handles _webClient.DownloadProgressChanged
        RaiseEvent PageDownloadProgress(sender, e)
    End Sub

#End Region

#Region "Routines"
    Private Function extractFileName(ByVal ImagePath As String) As String
        Dim result As String() = ImagePath.Split("/")
        Return result(result.GetUpperBound(0))
    End Function

    Public Sub BeginDownload()
        startDownload()
        downloadChapters()
        endDownload()
    End Sub
#End Region
End Class
