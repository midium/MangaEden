Option Explicit On

Imports MangaEdenAPI
Imports System.Threading

Public Class DownloadPage

#Region "Declarations"
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

    Private Delegate Function PrepareSingleDownload()
    Private _singleDownload As PrepareSingleDownload

    Private Delegate Function PrepareMultiDownload()
    Private _multiDownload As PrepareMultiDownload

#End Region

    Private Sub DownloadPage_ContentRendered(sender As Object, e As EventArgs) Handles Me.ContentRendered
        _flagRunned = True

        If Not _chapter Is Nothing Then
            ReDim _threads(0)
            _threads(0) = New Thread(AddressOf InitiateSingleDownload)
            _threads(0).Start()
        End If

        If Not _chapters Is Nothing Then
            ReDim _threads(0)
            _threads(0) = New Thread(AddressOf InitiateMultiDownload)
            _threads(0).Start()
        End If
    End Sub

#Region "Routines"
#Region "Single Download"
    Private Sub InitiateSingleDownload()
        Dispatcher.Invoke(_singleDownload)
    End Sub

    Private Function SingleDownload()
        Dispatcher.Invoke(_showDownload, gridContainer, 1, _chapter, _mangaName)

        Return True
    End Function

#End Region

#Region "Multi-Download"
    Private Sub InitiateMultiDownload()
        Dispatcher.Invoke(_multiDownload)
    End Sub

    Private Function MultiDownload()
        Dim iCnt As Integer = 1

        For Each chapt As MangaChaptersDetails In _chapters
            Dispatcher.Invoke(_showDownload, gridContainer, iCnt, chapt, _mangaName)

            iCnt += 10
        Next

        Return True
    End Function

#End Region

    Private Function NewDownload(ByVal objectContainer As Object, ByVal i As Integer, ByVal chapterInfo As MangaChaptersDetails, ByVal mangaName As String) As Boolean
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

        download = Nothing

        Return True
    End Function
#End Region

#Region "Initiation"
    Private Sub initDelegates()
        _showDownload = New ShowDownload(AddressOf NewDownload)
        _singleDownload = New PrepareSingleDownload(AddressOf SingleDownload)
        _multiDownload = New PrepareMultiDownload(AddressOf MultiDownload)
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

    End Sub

    Public Sub New(ByVal MangaName As String, ByVal Chapters() As MangaChaptersDetails)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        initDelegates()
        _chapters = Chapters
        _mangaName = MangaName

    End Sub
#End Region

    Private Sub btClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btClose.Click
        Me.Close()
    End Sub
End Class
