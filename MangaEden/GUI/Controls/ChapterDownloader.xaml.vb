Option Explicit On

Imports MangaEdenAPI
Imports WebElements
Imports System.Threading

Public Class ChapterDownloader
    Private _mangaTitle As String = ""
    Private _chapterInfo As MangaChaptersDetails

    Private _meAPI As API = Nothing
    Private _chapterImages As ChapterImages = Nothing

    Private WithEvents _downloader As ChapterImagesDownloader

#Region "Thread and Delegates"
    Private _run As Thread
    Private _timerCount As Integer = 0

    Private Delegate Function ExecuteDownload()
    Private _executeDownload As ExecuteDownload

    Private Delegate Sub UpdateStatus(ByVal Status As String)
    Private _updateStatus As UpdateStatus

    Private Delegate Sub UpdateProgress(ByVal current As Integer, ByVal max As Integer)
    Private _updateProgress As UpdateProgress

    Private Delegate Sub RunDownload()
    Private _runDownload As RunDownload

#End Region

    Public WriteOnly Property MangaTitle As String
        Set(value As String)
            _mangaTitle = value
            UpdateTitle()
        End Set
    End Property

    Public WriteOnly Property ChapterInfo As MangaChaptersDetails
        Set(value As MangaChaptersDetails)
            _chapterInfo = value
            UpdateTitle()
        End Set
    End Property

    Public WriteOnly Property Status As String
        Set(value As String)
            lblStatus.Content = String.Format("Status: {0}", value)
        End Set
    End Property

    Private Sub UpdateTitle()
        If Not _chapterInfo Is Nothing Then
            lblTitle.Content = String.Format("{0} - {1}: {2}", _mangaTitle, _chapterInfo.Number, _chapterInfo.Title)
        Else
            lblTitle.Content = String.Format("{0}", _mangaTitle)
        End If

    End Sub

    Public Function startDownload()
        'First I get the chapter images
        _chapterImages = _meAPI.getChapterImages(_chapterInfo.ChapterID)
        _chapterImages.Sort()

        Dim chapterImages As String() = Nothing
        Dim i As Integer = -1
        For Each img As Object In _chapterImages.images
            i += 1
            ReDim Preserve chapterImages(i)
            chapterImages(i) = img(1)
        Next

        _downloader = New ChapterImagesDownloader(chapterImages, _mangaTitle, _chapterInfo.Number, _chapterInfo.Title)
        _runDownload = New RunDownload(AddressOf _downloader.BeginDownload)
        Try
            Dispatcher.Invoke(_runDownload)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

        Return True

    End Function

    Private Sub _downloader_CurrentDownload(sender As Object, DownloadStatus As WebElements.ChapterImagesDownloader.Status) Handles _downloader.CurrentDownload
        Dispatcher.Invoke(_updateStatus, String.Format("Status: {0}", String.Format("Page {0} or {1}", DownloadStatus.currentDownload, DownloadStatus.totalDownloads)))
        Dispatcher.Invoke(_updateProgress, DownloadStatus.currentDownload, DownloadStatus.totalDownloads)
    End Sub

    Private Sub _downloader_DownloadBegin(sender As Object, DownloadStatus As WebElements.ChapterImagesDownloader.Status) Handles _downloader.DownloadBegin
        Dispatcher.Invoke(_updateStatus, "Status: Download Started")
        Dispatcher.Invoke(_updateProgress, 0, DownloadStatus.totalDownloads)
    End Sub

    Private Sub _downloader_DownloadEnd(sender As Object, DownloadStatus As WebElements.ChapterImagesDownloader.Status) Handles _downloader.DownloadEnd
        Dispatcher.Invoke(_updateStatus, "Status: Download Completed")
        Dispatcher.Invoke(_updateProgress, DownloadStatus.totalDownloads, DownloadStatus.totalDownloads)
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _meAPI = New API
        _executeDownload = New ExecuteDownload(AddressOf startDownload)
        _run = New Thread(AddressOf Timer)
        _updateStatus = New UpdateStatus(AddressOf UpdateStatusDelegate)
        _updateProgress = New UpdateProgress(AddressOf UpdateProgressDelegate)
    End Sub

    Private Sub ChapterDownloader_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        _run.Start()
    End Sub

    Private Sub Timer()
        Do
            _timerCount += 1

            If _timerCount = 2 Then
                Dispatcher.BeginInvoke(_executeDownload, Windows.Threading.DispatcherPriority.ApplicationIdle, Nothing)
            End If

            Thread.Sleep(1000)
        Loop Until _timerCount = 2
    End Sub

    Private Sub UpdateStatusDelegate(ByVal Status As String)
        lblStatus.Content = Status
    End Sub

    Private Sub UpdateProgressDelegate(ByVal current As Integer, ByVal max As Integer)
        pgbStatus.Minimum = 0
        pgbStatus.Maximum = max
        pgbStatus.Value = current
    End Sub

End Class
