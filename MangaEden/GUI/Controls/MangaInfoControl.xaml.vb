Option Explicit On

Imports MangaEdenAPI
Imports System.Globalization
Imports System.Threading
Imports System.Drawing

Public Class MangaInfoControl
    Private _coverPath As String
    Private _myMangaInfo As MyMangaInfo
    Private _mangaInfo As MangaBasicInfo
    Private _mangaDetails As MangaDetails
    Private _flgMyManga As Boolean

#Region "Thread and delegate function declaration"
    Private _loadThread As Thread

    Private Delegate Function DownloadCover(ByVal CoverPath As String)
    Private _downloadCover As DownloadCover

    Private Delegate Function PopulateMyMangaData()
    Private _populateMyMangaData As PopulateMyMangaData

    Private Delegate Function PopulateMangaData()
    Private _populateMangaData As PopulateMangaData

    Private Delegate Function LoadMangaDetails()
    Private _loadMangaDetails As LoadMangaDetails

#End Region

#Region "Routines"
#Region "Thread and delegates"
    Private Function CollectAndShowMyMangaInfo() As Boolean

        'Showing manga information
        Dispatcher.Invoke(_populateMyMangaData)

        'Trying download cover
        _coverPath = _myMangaInfo.manga.image
        Dispatcher.Invoke(_downloadCover, _coverPath)

        Return True
    End Function

    Private Function CollectAndShowMangaInfo() As Boolean

        'First I need to collect details of the manga
        Dispatcher.Invoke(_loadMangaDetails)

        'Trying download cover
        _coverPath = _mangaInfo.im
        Dispatcher.Invoke(_downloadCover, _coverPath)

        Return True
    End Function

    Private Function collectMangaDetails() As Boolean
        Dim meApi As New API

        _mangaDetails = meApi.getMangaDetails(_mangaInfo.ID)

        'Showing manga information
        Dispatcher.Invoke(_populateMangaData)

        Return True
    End Function

    Private Function searchAndDownloadCover(ByVal CoverPath As String) As Boolean
        If Not CoverPath Is Nothing Then
            If CoverPath.Trim <> "" Then
                'If the cover path is empty then I simply don't download it, instead if it is not empty I proceed
                Dim meAPI As New API

                imgCover.Source = Nothing

                Dim localPath As String = meAPI.getImage(CoverPath)
                If localPath <> "" Then
                    Dim localURI As New Uri(localPath)
                    Dim image As New BitmapImage(localURI)

                    imgCover.Source = image

                    image = Nothing

                End If

            End If

        Else
            imgCover.Source = Nothing

        End If

        Return True
    End Function

    Private Function showMyMangaInfo() As Boolean
        lblTitolo.Content = _myMangaInfo.manga.title

        If Not _myMangaInfo.manga.author_kw Is Nothing And IsArray(_myMangaInfo.manga.author_kw) Then
            lblAuthor.Content = ComposeName(_myMangaInfo.manga.author_kw)
        Else
            lblAuthor.Content = _myMangaInfo.manga.author
        End If

        If Not _myMangaInfo.manga.author_kw Is Nothing And IsArray(_myMangaInfo.manga.artist_kw) Then
            lblArtist.Content = ComposeName(_myMangaInfo.manga.artist_kw)
        Else
            lblArtist.Content = _myMangaInfo.manga.artist
        End If

        lblReleased.Content = _myMangaInfo.manga.released

        If Not _myMangaInfo.manga.categories Is Nothing And IsArray(_myMangaInfo.manga.categories) Then
            lblCategories.Content = Join(_myMangaInfo.manga.categories, ", ")
        End If

        lblDescription.Text = System.Net.WebUtility.HtmlDecode(_myMangaInfo.manga.description)
        lblHits.Content = _myMangaInfo.manga.hits
        lblStatus.Content = IIf(_myMangaInfo.manga.status = 2, "Finished", "On Going")
        lblTotalChapters.Content = _myMangaInfo.manga.chapters_len

        If _myMangaInfo.manga.chapters_len > 0 Then
            btChapters.Visibility = Windows.Visibility.Visible
        Else
            btChapters.Visibility = Windows.Visibility.Collapsed
        End If

        If Not _myMangaInfo.latest_chapter Is Nothing Then
            lblLatestChapter.Content = String.Format("{0} - {1} [{2}]", _myMangaInfo.latest_chapter.number, _myMangaInfo.latest_chapter.title, _myMangaInfo.latest_chapter.LastChapterDate)
            btPlay.Visibility = Windows.Visibility.Visible
            btDownload.Visibility = Windows.Visibility.Visible
        Else
            lblLatestChapter.Content = ""
            btPlay.Visibility = Windows.Visibility.Collapsed
            btDownload.Visibility = Windows.Visibility.Collapsed
        End If

        Return True
    End Function

    Private Function showMangaInfo() As Boolean
        lblTitolo.Content = _mangaDetails.title

        If Not _mangaDetails.author_kw Is Nothing And IsArray(_mangaDetails.author_kw) Then
            lblAuthor.Content = ComposeName(_mangaDetails.author_kw)
        Else
            lblAuthor.Content = _mangaDetails.author
        End If

        If Not _mangaDetails.author_kw Is Nothing And IsArray(_mangaDetails.artist_kw) Then
            lblArtist.Content = ComposeName(_mangaDetails.artist_kw)
        Else
            lblArtist.Content = _mangaDetails.artist
        End If

        lblReleased.Content = _mangaDetails.released

        If Not _mangaDetails.categories Is Nothing And IsArray(_mangaDetails.categories) Then
            lblCategories.Content = Join(_mangaDetails.categories, ", ")
        End If

        lblDescription.Text = System.Net.WebUtility.HtmlDecode(_mangaDetails.description)
        lblHits.Content = _mangaDetails.hits
        lblStatus.Content = IIf(_mangaDetails.status = 2, "Finished", "On Going")
        lblTotalChapters.Content = _mangaDetails.chapters_len

        If _mangaDetails.chapters_len > 0 Then
            btChapters.Visibility = Windows.Visibility.Visible
        Else
            btChapters.Visibility = Windows.Visibility.Collapsed
        End If

        If Not _mangaDetails.chapters Is Nothing Then
            lblLatestChapter.Content = String.Format("{0} - {1} [{2}]", _mangaDetails.ChapterNumber(0), _mangaDetails.ChapterTitle(0), _mangaDetails.ChapterDate(0, "dd-MM-yyyy"))
            btPlay.Visibility = Windows.Visibility.Visible
            btDownload.Visibility = Windows.Visibility.Visible
        Else
            lblLatestChapter.Content = ""
            btPlay.Visibility = Windows.Visibility.Collapsed
            btDownload.Visibility = Windows.Visibility.Collapsed
        End If

        Return True
    End Function
#End Region

#Region "Public"
    Public Sub ShowMyMangaInfo(ByVal myManga As MyMangaInfo)
        'Saving image path and starting dowloading thread
        _flgMyManga = True
        _myMangaInfo = myManga

        _loadThread = New Thread(AddressOf CollectAndShowMyMangaInfo)
        _loadThread.Start()
    End Sub

    Public Sub ShowMangaInfo(ByVal Manga As MangaBasicInfo)
        'Saving image path and starting dowloading thread
        _flgMyManga = False
        _mangaInfo = Manga

        _loadThread = New Thread(AddressOf CollectAndShowMangaInfo)
        _loadThread.Start()
    End Sub
#End Region

#Region "Private"
    Private Function ComposeName(ByVal nameArray As String()) As String
        Dim result As String = ""

        For i As Integer = nameArray.GetUpperBound(0) To 0 Step -1
            result &= nameArray(i) & " "
        Next

        Return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(result.Trim)
    End Function
#End Region
#End Region

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _downloadCover = New DownloadCover(AddressOf searchAndDownloadCover)
        _populateMyMangaData = New PopulateMyMangaData(AddressOf showMyMangaInfo)
        _populateMangaData = New PopulateMangaData(AddressOf showMangaInfo)
        _loadMangaDetails = New LoadMangaDetails(AddressOf collectMangaDetails)

    End Sub

    Private Sub btChapters_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles btChapters.Click
        Dim frmChapter As MangaChapters

        If _flgMyManga Then
            frmChapter = New MangaChapters(_myMangaInfo, _myMangaInfo.ID, _flgMyManga)
        Else
            frmChapter = New MangaChapters(_mangaDetails, _mangaInfo.ID, _flgMyManga)
        End If

        frmChapter.ShowDialog()
        frmChapter.Close()
        frmChapter = Nothing
    End Sub
End Class
