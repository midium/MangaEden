Option Explicit On

Imports MangaEdenAPI
Imports System.Threading

Public Class ChapterViewer

    Private _mangaID As String = ""
    Private _mangaName As String = ""
    Private _chapterTitle As String = ""
    Private _chapterNumber As Integer = -1

    Private _imageIndex As Integer = 0

    Private _meAPI As API = Nothing
    Private _chapterImages As ChapterImages = Nothing

    Public Sub New(ByVal ID As String, ByVal MangaName As String, ByVal ChapterTitle As String, ByVal ChapterNumber As Integer)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _mangaID = ID
        _mangaName = MangaName
        _chapterTitle = ChapterTitle
        _chapterNumber = ChapterNumber

        _meAPI = New API

    End Sub

    Private Sub ChapterViewer_KeyDown(sender As Object, e As System.Windows.Input.KeyEventArgs) Handles Me.KeyDown
        If e.Key = Key.Right Then
            NextImage()

        ElseIf e.Key = Key.Left Then
            PreviousImage()

        ElseIf e.Key = Key.Escape Then
            Me.Close()

        End If
    End Sub

    Private Sub ChapterViewer_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        lblTitle.Content = String.Format("{0} - {1}: {2}", _mangaName, _chapterNumber, _chapterTitle)

        _chapterImages = _meAPI.getChapterImages(_mangaID)
        _chapterImages.Sort()

        LoadChapterImage(_imageIndex)

    End Sub

    Private Function LoadChapterImage(ByVal imageIndex As Integer) As Boolean

        If imageIndex < 0 Then imageIndex = 0
        If imageIndex > _chapterImages.images.Count - 1 Then _imageIndex = _chapterImages.images.Count - 1

        Dim localPath As String = _meAPI.getImage(_chapterImages.images(imageIndex)(1))
        If localPath <> "" Then
            Dim localURI As New Uri(localPath)
            Dim image As New BitmapImage(localURI)

            imgPage.Source = image

            image = Nothing

        End If

        If imageIndex = 0 Then
            imgPrev.Visibility = Windows.Visibility.Collapsed
            imgNext.Visibility = Windows.Visibility.Visible

        ElseIf imageIndex > 0 And imageIndex < _chapterImages.images.Count - 1 Then
            imgPrev.Visibility = Windows.Visibility.Visible
            imgNext.Visibility = Windows.Visibility.Visible

        Else
            imgPrev.Visibility = Windows.Visibility.Visible
            imgNext.Visibility = Windows.Visibility.Collapsed

        End If

        Return Nothing
    End Function

    Private Sub PreviousImage()
        Me.Cursor = Cursors.Wait

        _imageIndex -= 1
        LoadChapterImage(_imageIndex)

        Me.Cursor = Cursors.Arrow
    End Sub

    Private Sub NextImage()
        Me.Cursor = Cursors.Wait

        _imageIndex += 1
        LoadChapterImage(_imageIndex)

        Me.Cursor = Cursors.Arrow

    End Sub

    Private Sub imgPrev_MouseDown(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs) Handles imgPrev.MouseDown
        PreviousImage()
    End Sub

    Private Sub imgNext_MouseDown(sender As Object, e As System.Windows.Input.MouseButtonEventArgs) Handles imgNext.MouseDown
        NextImage()
    End Sub
End Class
