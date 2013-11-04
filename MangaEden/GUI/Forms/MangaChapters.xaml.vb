Option Explicit On

Imports MangaEdenAPI
Imports System.Threading

Public Class MangaChapters

    Private _mangaInfo As Object
    Private _mangaID As String
    Private _flgMyManga As Boolean
    Private _mangaDetails As MangaDetails
    Private _mangaChapters As MangaChaptersDetails()

#Region "Thread and delegate function declaration"
    Private _loadThread As Thread

    Private Delegate Function LoadMangaDetails()
    Private _loadMangaDetails As LoadMangaDetails

    Private Delegate Function ExtractMangaChapters(ByVal chapters As Object)
    Private _extractMangaChapters As ExtractMangaChapters

#End Region

    Public Sub New(ByVal mangaData As Object, ByVal mangaID As String, ByVal flgMyManga As Boolean)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _mangaInfo = mangaData
        _mangaID = mangaID
        _flgMyManga = flgMyManga

        'Initializing thread and delegated routines
        _loadMangaDetails = New LoadMangaDetails(AddressOf collectMangaDetails)
        _extractMangaChapters = New ExtractMangaChapters(AddressOf collectMangaChapters)
        _loadThread = New Thread(AddressOf CollectAndShowMangaInfo)

    End Sub

    Private Sub MangaChapters_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If _flgMyManga Then
            lblMangaTitle.Content = String.Format("{0} - {1} chapters available", _mangaInfo.title, _mangaInfo.manga.chapters_len)
        Else
            lblMangaTitle.Content = String.Format("{0} - {1} chapters available", _mangaInfo.title, _mangaInfo.chapters_len)
        End If

        _loadThread.Start()
    End Sub

    Private Sub btClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btClose.Click
        Me.Close()
    End Sub

#Region "Thread and Delegated Routines"
    Private Function collectMangaChapters(ByVal chapters As List(Of List(Of Object))) As Boolean

        If Not chapters Is Nothing Then
            Dim iCount As Integer = 0

            ReDim Preserve _mangaChapters(chapters.Count - 1)
            For Each i As Object In chapters
                _mangaChapters(iCount) = New MangaChaptersDetails

                _mangaChapters(iCount).Number = i(0)
                _mangaChapters(iCount).chapter_date = i(1)
                _mangaChapters(iCount).Title = i(2)
                _mangaChapters(iCount).ChapterID = i(3)

                iCount += 1
            Next

            lstChapters.ItemsSource = _mangaChapters
        End If

        Return True
    End Function

    Private Function CollectAndShowMangaInfo() As Boolean

        'First I need to collect details of the manga
        Dispatcher.Invoke(_loadMangaDetails)

        Return True
    End Function

    Private Function collectMangaDetails() As Boolean
        Dim meApi As New API

        _mangaDetails = meApi.getMangaDetails(_mangaID)

        'Showing manga information
        Dispatcher.Invoke(_extractMangaChapters, _mangaDetails.chapters)

        Return True
    End Function
#End Region
End Class
