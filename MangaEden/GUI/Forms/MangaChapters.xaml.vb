Option Explicit On

Imports MangaEdenAPI
Imports System.Threading

Public Class MangaChapters

    Private _mangaDetails As MangaDetails
    Private _mangaChapters As MangaChaptersDetails()

#Region "Thread and delegate function declaration"
    Private _loadThread As Thread

    Private Delegate Function LoadMangaDetails()
    Private _loadMangaDetails As LoadMangaDetails

    Private Delegate Function ExtractMangaChapters(ByVal chapters As Object)
    Private _extractMangaChapters As ExtractMangaChapters

#End Region

    Public Sub New(ByVal mangaData As MangaDetails)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _mangaDetails = mangaData
        _mangaChapters = Nothing

        'Initializing thread and delegated routines
        _loadMangaDetails = New LoadMangaDetails(AddressOf collectMangaDetails)
        _extractMangaChapters = New ExtractMangaChapters(AddressOf collectMangaChapters)
        _loadThread = New Thread(AddressOf CollectAndShowMangaInfo)

    End Sub

#Region "Form and control events"
    Private Sub MangaChapters_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        lblMangaTitle.Content = String.Format("{0} - {1} chapters available", _mangaDetails.title, _mangaDetails.chapters_len)
        
        _loadThread.Start()
    End Sub

    Private Sub btClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btClose.Click
        Me.Close()
    End Sub

    Private Sub lstChapters_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstChapters.SelectionChanged
        btDownload.IsEnabled = True
        btView.IsEnabled = True
    End Sub

    Private Sub btView_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btView.Click

        Dim Chapter As MangaChaptersDetails = lstChapters.SelectedItem

        Dim frmPlayer As ChapterViewer = New ChapterViewer(Chapter.ChapterID, _mangaDetails.title, Chapter.Title, Chapter.Number)
        frmPlayer.ShowDialog()
        frmPlayer = Nothing
    End Sub

#End Region

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

        'Showing manga information
        Dispatcher.Invoke(_extractMangaChapters, _mangaDetails.chapters)

        Return True
    End Function
#End Region

End Class
