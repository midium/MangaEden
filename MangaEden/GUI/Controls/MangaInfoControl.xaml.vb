Option Explicit On

Imports MangaEdenAPI
Imports System.Globalization
Imports System.Threading
Imports System.Drawing
Imports System.IO

Public Class MangaInfoControl
    Private _coverPath As String
    Private _mangaID As String
    Private _mangaDetails As MangaDetails
    Private _flgMyManga As Boolean

#Region "Events declaration"
    Public Event CollectInfoBegin(ByVal sender As Object)
    Public Event CollectInfoEnd(ByVal sender As Object)
#End Region

#Region "Thread and delegate function declaration"
    Private _loadThread As Thread

    Private Delegate Function DownloadCover(ByVal CoverPath As String)
    Private _downloadCover As DownloadCover

    Private Delegate Function PopulateMangaData()
    Private _populateMangaData As PopulateMangaData

    Private Delegate Function LoadMangaDetails()
    Private _loadMangaDetails As LoadMangaDetails

#End Region

#Region "Routines"
#Region "Thread and delegates"
    Private Function CollectAndShowMangaInfo() As Boolean
        RaiseEvent CollectInfoBegin(Me)

        'First I need to collect details of the manga
        Dispatcher.Invoke(_loadMangaDetails)

        'Trying download cover
        _coverPath = _mangaDetails.image
        Dispatcher.Invoke(_downloadCover, _coverPath)

        RaiseEvent CollectInfoEnd(Me)

        Return True
    End Function

    Private Function collectMangaDetails() As Boolean
        Dim meApi As New API

        _mangaDetails = meApi.getMangaDetails(_mangaID)

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
                    Dim image As BitmapImage

                    If File.Exists(localPath) Then
                        Dim localURI As New Uri(localPath)

                        image = New BitmapImage(localURI)
                    Else
                        image = Nothing
                    End If

                    imgCover.Source = Image

                    Image = Nothing

                End If

                End If

            Else
                imgCover.Source = Nothing

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
    Public Sub ShowMangaInfo(ByVal MangaID As String)
        'Saving image path and starting dowloading thread
        _flgMyManga = False
        _mangaID = MangaID

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
        _populateMangaData = New PopulateMangaData(AddressOf showMangaInfo)
        _loadMangaDetails = New LoadMangaDetails(AddressOf collectMangaDetails)

    End Sub

    Private Sub btChapters_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles btChapters.Click
        Dim frmChapter As MangaChapters

        frmChapter = New MangaChapters(_mangaDetails)

        frmChapter.ShowDialog()
        frmChapter.Close()
        frmChapter = Nothing
    End Sub

    Private Sub btPlay_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btPlay.Click
        Dim frmPlay As ChapterViewer = Nothing

        frmPlay = New ChapterViewer(_mangaDetails.chapters(0)(3), _mangaDetails.title, _mangaDetails.chapters(0)(2), _mangaDetails.chapters(0)(0))

        frmPlay.ShowDialog()
        frmPlay = Nothing
    End Sub
End Class
