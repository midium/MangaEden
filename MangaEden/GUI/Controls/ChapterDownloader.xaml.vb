Option Explicit On

Imports MangaEdenAPI
Imports WebElements
Imports System.Threading
Imports System.IO

Public Class ChapterDownloader
    Private _mangaTitle As String = ""
    Private _chapterInfo As MangaChaptersDetails
    Private _downloadedPath As String = ""

    Private _meAPI As API = Nothing
    Private _chapterImages As ChapterImages = Nothing

    Private _bOldDownload As Boolean = False

    Private WithEvents _downloader As ChapterImagesDownloader

    Public Event DownloadDeletedFromDisk(sender As Object)

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

    Private Delegate Sub EnableButtons(ByVal enabled As Boolean)
    Private _enableButtons As EnableButtons

#End Region

#Region "Properties"
    Public WriteOnly Property DownloadedPath As String
        Set(value As String)
            _downloadedPath = value
        End Set
    End Property

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
#End Region

#Region "Routines"
    Private Function isFolder(ByVal path As String) As Boolean
        Dim fl As FileAttribute = File.GetAttributes(path)
        Return ((fl And FileAttribute.Directory) = FileAttribute.Directory)
    End Function

    Private Sub UpdateTitle()
        lblTitle.Text = ""
        lblTitle.Inlines.Add("Manga: ")
        lblTitle.Inlines.Add(New Bold(New Run(_mangaTitle)))

        lblChapter.Text = ""
        lblChapter.Inlines.Add("Chapter: ")

        If Not _chapterInfo Is Nothing Then
            lblChapter.Inlines.Add(New Bold(New Run(String.Format("{0} - {1}", _chapterInfo.Number, _chapterInfo.Title))))
        End If

    End Sub

    Public Function startDownload()
        rctStatus.Fill = Brushes.Orange

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
#End Region

#Region "Events management"
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
        Dispatcher.Invoke(_enableButtons, True)
    End Sub
#End Region

    Public Sub New(Optional ByVal bOldDownload As Boolean = False)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _meAPI = New API
        _executeDownload = New ExecuteDownload(AddressOf startDownload)
        _run = New Thread(AddressOf Timer)
        _updateStatus = New UpdateStatus(AddressOf UpdateStatusDelegate)
        _updateProgress = New UpdateProgress(AddressOf UpdateProgressDelegate)
        _enableButtons = New EnableButtons(AddressOf EnableButtonsDelegate)

        _bOldDownload = bOldDownload
    End Sub

    Private Sub ChapterDownloader_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If Not _bOldDownload Then
            _run.Start()
        Else
            UpdateStatusDelegate("Status: Download Completed")
            UpdateProgressDelegate(pgbStatus.Maximum, pgbStatus.Maximum)
            EnableButtonsDelegate(True)
        End If
    End Sub

#Region "Thread and Delegated functions"
    Private Sub Timer()
        'TODO: Find better solution
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

    Private Sub EnableButtonsDelegate(ByVal enabled As Boolean)
        btDelete.IsEnabled = enabled
        btView.IsEnabled = enabled

        rctStatus.Fill = Brushes.Blue

    End Sub
#End Region

    Private Sub btDelete_Click(sender As Object, e As RoutedEventArgs) Handles btDelete.Click

        If MsgBox("You are about to delete the downloaded chapter. This action can't be undone." & vbCrLf & "Are you sure you want to remove it?", MsgBoxStyle.YesNo + MsgBoxStyle.Critical) = MsgBoxResult.Yes Then
            Dim fileOrFolder As String = ""
            If _bOldDownload Then
                fileOrFolder = _downloadedPath

            Else
                fileOrFolder = _downloader.DownloadedPathOrFile

            End If

            Try

                If isFolder(fileOrFolder) Then
                    'It has been download to folder
                    Directory.Delete(fileOrFolder, True)
                Else
                    'It has been download as zip file
                    File.Delete(fileOrFolder)
                End If

                MsgBox(String.Format("Chapter {0} of manga {1} correctly deleted.", _chapterInfo.Number, _mangaTitle), MsgBoxStyle.Information)

                RaiseEvent DownloadDeletedFromDisk(Me)
                Me.Visibility = Windows.Visibility.Collapsed

            Catch ex As Exception
                MsgBox("There have been problems removing the selected chapter." & vbCrLf & "Please check that the chapter isn't open by other programs")

            End Try

        End If

    End Sub

    Private Sub btView_Click(sender As Object, e As RoutedEventArgs) Handles btView.Click
        Dim frmPlay As ChapterViewer = Nothing
        Dim sourcePath As String = ""

        If _bOldDownload Then
            sourcePath = _downloadedPath
        Else
            sourcePath = _downloader.DownloadedPathOrFile
        End If
        frmPlay = New ChapterViewer(sourcePath, _mangaTitle, _chapterInfo.Title, _chapterInfo.Number, True)

        frmPlay.ShowDialog()
        frmPlay = Nothing
    End Sub
End Class
