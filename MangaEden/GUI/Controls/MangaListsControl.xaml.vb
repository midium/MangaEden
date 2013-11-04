Option Explicit On

Imports MangaEdenAPI

Public Class MangaListsControl

#Region "Events declaration"
    Public Event MyMangaSelected(ByVal sender As Object, ByVal e As MyMangaInfo)
    Public Event MangaSelected(ByVal sender As Object, ByVal e As MangaBasicInfo)
#End Region

#Region "Variables declaration"
    Private _lblAvailableMargin As Thickness = Nothing
    Private _rctAvailableMargin As Thickness = Nothing
    Private _lstAvailableMargin As Thickness = Nothing
    Private _lblTitleMargin As Thickness = Nothing
    Private _lblLUMargin As Thickness = Nothing
#End Region

#Region "Routines"
    Public Function showMangaList(ByVal mangaList As MangaList) As Boolean
        lstMangas.ItemsSource = Nothing
        lstMangas.ItemsSource = mangaList.manga

        Return True
    End Function

    Public Function showMyMangaList(ByVal myMangaList As MyManga) As Boolean
        lstMyMangas.ItemsSource = Nothing
        lstMyMangas.ItemsSource = myMangaList.myManga

        Return True
    End Function

    Public Sub ShowHideMyMangas(ByVal bShow As Boolean)
        'Hiding my mangas
        lstMyMangas.Visibility = BoolToVisibility(bShow)
        lblMyManga.Visibility = BoolToVisibility(bShow)
        rctMyManga.Visibility = BoolToVisibility(bShow)
        lblLUMyMangas.Visibility = BoolToVisibility(bShow)
        lblTitleMyMangas.Visibility = BoolToVisibility(bShow)

        If Not bShow Then
            'Increasing available manga space
            lblAvailable.Margin = lblMyManga.Margin
            rctAvailable.Margin = rctMyManga.Margin
            lstMangas.Margin = lstMyMangas.Margin
            lblLUMangas.Margin = lblLUMyMangas.Margin
            lblTitleMangas.Margin = lblTitleMyMangas.Margin

        Else
            'Restoring original position
            lblAvailable.Margin = _lblAvailableMargin
            rctAvailable.Margin = _rctAvailableMargin
            lstMangas.Margin = _lstAvailableMargin

        End If

    End Sub
#End Region

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _lblAvailableMargin = lblAvailable.Margin
        _rctAvailableMargin = rctAvailable.Margin
        _lstAvailableMargin = lstMangas.Margin
        _lblLUMargin = lblLUMangas.Margin
        _lblTitleMargin = lblTitleMangas.Margin
    End Sub

    Private Sub lstMyMangas_GotFocus(sender As Object, e As System.Windows.RoutedEventArgs) Handles lstMyMangas.GotFocus
        lstMangas.SelectedItem = Nothing
    End Sub

    Private Sub lstMyMangas_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lstMyMangas.SelectionChanged
        If Not lstMyMangas.SelectedItem Is Nothing Then
            Dim myMangaSelected As MyMangaInfo = lstMyMangas.SelectedItem
            RaiseEvent MyMangaSelected(sender, myMangaSelected)
        End If
    End Sub

    Private Sub lstMangas_GotFocus(sender As Object, e As System.Windows.RoutedEventArgs) Handles lstMangas.GotFocus
        lstMyMangas.SelectedItem = Nothing
    End Sub

    Private Sub lstMangas_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstMangas.SelectionChanged
        If Not lstMangas.SelectedItem Is Nothing Then
            Dim mangaSelected As MangaBasicInfo = lstMangas.SelectedItem
            RaiseEvent MangaSelected(sender, mangaSelected)
        End If
    End Sub
End Class
