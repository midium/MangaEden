Option Explicit On

Imports MangaEdenAPI
Imports WebElements

Public Class ChapterDownloader
    Private _mangaTitle As String = ""
    Private _chapterInfo As MangaChaptersDetails

    Private _meAPI As API = Nothing
    Private _chapterImages As ChapterImages = Nothing

    Private WithEvents _downloader As ChapterImagesDownloader

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
        _downloader.BeginDownload()

        Return True

    End Function

    Private Sub _downloader_CurrentDownload(sender As Object, DownloadStatus As WebElements.ChapterImagesDownloader.Status) Handles _downloader.CurrentDownload
        lblStatus.Content = String.Format("Status: {0}", String.Format("Page {0} or {1}", DownloadStatus.currentDownload, DownloadStatus.totalDownloads))
    End Sub

    Private Sub _downloader_DownloadBegin(sender As Object, DownloadStatus As WebElements.ChapterImagesDownloader.Status) Handles _downloader.DownloadBegin
        lblStatus.Content = String.Format("Status: {0}", String.Format("Page {0} or {1}", DownloadStatus.currentDownload, DownloadStatus.totalDownloads))
    End Sub

    Private Sub _downloader_DownloadEnd(sender As Object, DownloadStatus As WebElements.ChapterImagesDownloader.Status) Handles _downloader.DownloadEnd
        lblStatus.Content = String.Format("Status: Download Completed")
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _meAPI = New API
    End Sub
End Class
