Option Explicit On

Imports MangaEdenAPI
Imports System.Threading

Public Class DownloadPage

#Region "Declarations"
    Private downloads As List(Of ChapterDownloader)
    Private _chapters As MangaChaptersDetails()
    Private _chapter As MangaChaptersDetails
    Private _mangaName As String
    Private _flagRunned As Boolean = False
#End Region

#Region "Thread and delegates"
    Private _threads As Thread()

    'Single download delegates
    Private Delegate Function ShowDownload(ByVal objectContainer As Object, ByVal i As Integer, ByVal chapterInfo As MangaChaptersDetails, ByVal mangaName As String)
    Private _showDownload As ShowDownload

    Private Delegate Function StartDownload()
    Private _startDownload As StartDownload

    Private Delegate Function PrepareSingleDownload()
    Private _singleDownload As PrepareSingleDownload

#End Region

    Private Sub DownloadPage_ContentRendered(sender As Object, e As EventArgs) Handles Me.ContentRendered
        _flagRunned = True

        If Not _chapter Is Nothing Then
            ReDim _threads(0)
            _threads(0) = New Thread(AddressOf InitiateSingleDownload)
            _threads(0).Start()
        End If

    End Sub

#Region "Routines"
    Private Sub InitiateSingleDownload()
        Dispatcher.Invoke(_singleDownload)
    End Sub

    Private Function SingleDownload()
        Dispatcher.Invoke(_showDownload, gridContainer, 1, _chapter, _mangaName)

        Return True
    End Function

    Private Function SingleDownloadStart()
        downloads(0).startDownload()

        Return True
    End Function

    Private Function NewDownload(ByVal objectContainer As Object, ByVal i As Integer, ByVal chapterInfo As MangaChaptersDetails, ByVal mangaName As String) As Boolean
        'If the list containing all the downloads is not inited then I init it.
        If downloads Is Nothing Then
            downloads = New List(Of ChapterDownloader)
        End If

        'Loading the new download module and showing it into the page
        Dim download As New ChapterDownloader
        objectContainer.Children.Add(download)
        download.MangaTitle = mangaName
        download.ChapterInfo = chapterInfo

        download.Name = "Test" & i
        download.Width = objectContainer.Width - 10
        download.Height = 87
        download.Margin = New Thickness(10, i * 10, 10, 0)
        download.VerticalAlignment = Windows.VerticalAlignment.Top

        'Adding it to the downloads list
        downloads.Add(download)

        download = Nothing

        Return True
    End Function
#End Region

#Region "Initiation"
    Private Sub initDelegates()
        _showDownload = New ShowDownload(AddressOf NewDownload)
        _startDownload = New StartDownload(AddressOf SingleDownloadStart)
        _singleDownload = New PrepareSingleDownload(AddressOf SingleDownload)
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        initDelegates()

    End Sub

    Public Sub New(ByVal MangaName As String, ByVal Chapter As MangaChaptersDetails)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        initDelegates()
        _chapter = Chapter
        _mangaName = MangaName

        downloads = New List(Of ChapterDownloader)
    End Sub

    Public Sub New(ByVal MangaName As String, ByVal Chapters() As MangaChaptersDetails)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        initDelegates()
        _chapters = Chapters
        _mangaName = MangaName

        downloads = New List(Of ChapterDownloader)
    End Sub
#End Region

End Class
