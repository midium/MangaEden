Option Explicit On

Imports MangaEdenAPI
Imports System.Threading

Public Class DownloadPage

#Region "Declarations"
    Private downloads As List(Of ChapterDownloader)
    Private _chapters As MangaChaptersDetails()
    Private _chapter As MangaChaptersDetails
    Private _mangaName As String
#End Region

#Region "Thread and delegates"
    Private _threads As Thread()

    'Single download delegates
    Private Delegate Function ShowDownload(ByVal objectContainer As Object, ByVal i As Integer, ByVal chapterInfo As MangaChaptersDetails, ByVal mangaName As String)
    Private _showDownload As ShowDownload

    Private Delegate Function StartDownload()
    Private _startDownload As StartDownload


#End Region

    Private Sub DownloadPage_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        If Not _chapter Is Nothing Then
            ReDim _threads(0)
            _threads(0) = New Thread(AddressOf SingleDownload)
            _threads(0).SetApartmentState(ApartmentState.STA)
            _threads(0).Start()
        End If

    End Sub

#Region "Routines"
    Private Sub SingleDownload()
        Dispatcher.Invoke(_showDownload, gridContainer, 1, _chapter, _mangaName)
        Dispatcher.Invoke(_startDownload)
    End Sub

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
