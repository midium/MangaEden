Option Explicit On

Imports MangaEdenAPI
Imports System.Threading
Imports System.IO
Imports ICSharpCode.SharpZipLib.Core
Imports ICSharpCode.SharpZipLib.Zip
Imports SettingsManager

Public Class ChapterViewer

    Private _mangaChapter As String = ""
    Private _mangaName As String = ""
    Private _chapterTitle As String = ""
    Private _chapterNumber As Integer = -1

    Private _imageIndex As Integer = 0

    Private _meAPI As API = Nothing
    Private _chapterImages As ChapterImages = Nothing
    Private _localFolder As String = ""
    Private _localChapterImages As String()

    Private _fileOrFolder As String = ""
    Private _flgFromDisk As Boolean = False
    Private _flgImageLoading As Boolean = False 'This will let me know if I'm already opening an image (key or icon navigation) or if I've choose from the combobox

    Private _settings As AppManager

    Public Sub New(ByVal Chapter As String, ByVal MangaName As String, ByVal ChapterTitle As String, ByVal ChapterNumber As Integer, Optional ByVal FromDisk As Boolean = False)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _mangaChapter = Chapter 'According to the FromDisk flag this will be interpreted ad the manga id or as the local path 
        _mangaName = MangaName
        _chapterTitle = ChapterTitle
        _chapterNumber = ChapterNumber
        _flgFromDisk = FromDisk
        _localFolder = ""

        If Not FromDisk Then
            _meAPI = New API
        End If

        _settings = New AppManager
        _settings.LoadSettings()

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

    Private Function isFolder(ByVal path As String) As Boolean
        Dim fl As FileAttribute = File.GetAttributes(path)
        Return ((fl And FileAttribute.Directory) = FileAttribute.Directory)
    End Function

    Private Sub ChapterViewer_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        'I first check if this chapter is available on local disk in one of the allowed formats
        checkOnDisk()

        'Updating title
        lblTitle.Content = String.Format("{0} - {1}: {2} [{3}]", _mangaName, _chapterNumber, _chapterTitle, IIf(_flgFromDisk, "Local", "Online"))

        'If available on disk or required to show the local one
        If _flgFromDisk Then
            If isFolder(_mangaChapter) Then
                'Folder
                _localFolder = _mangaChapter
                _localChapterImages = Directory.GetFiles(_mangaChapter)

            Else
                'ZIP
                _localFolder = String.Format("{0}\{1}-{2}", My.Computer.FileSystem.SpecialDirectories.Temp, _mangaName, _chapterNumber)
                If Not Directory.Exists(_localFolder) Then
                    Directory.CreateDirectory(_localFolder)
                End If

                'Decompressing file to folder
                Dim zipper As ZipFile = Nothing

                Dim fs As FileStream = File.OpenRead(_mangaChapter)
                zipper = New ZipFile(fs)

                Dim iCounter As Integer = -1

                For Each zipEntry As ZipEntry In zipper
                    iCounter += 1
                    If Not zipEntry.IsFile Then     ' Ignore directories
                        Continue For
                    End If
                    Dim entryFileName As [String] = zipEntry.Name
                    ' to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    ' Optionally match entrynames against a selection list here to skip as desired.
                    ' The unpacked length is available in the zipEntry.Size property.

                    Dim buffer As Byte() = New Byte(4095) {}    ' 4K is optimum
                    Dim zipStream As Stream = zipper.GetInputStream(zipEntry)

                    ' Manipulate the output filename here as desired.
                    Dim fullZipToPath As [String] = Path.Combine(_localFolder, entryFileName)
                    Dim directoryName As String = Path.GetDirectoryName(fullZipToPath)
                    If directoryName.Length > 0 Then
                        Directory.CreateDirectory(directoryName)
                    End If

                    ' Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    ' of the file, but does not waste memory.
                    ' The "Using" will close the stream even if an exception occurs.
                    Using streamWriter As FileStream = File.Create(fullZipToPath)
                        StreamUtils.Copy(zipStream, streamWriter, buffer)
                    End Using

                    'Saving decompressed file path for player
                    ReDim Preserve _localChapterImages(iCounter)
                    _localChapterImages(iCounter) = fullZipToPath
                Next

                zipper.IsStreamOwner = True
                zipper.Close()

            End If

            fillPageCombo(_localChapterImages.Length)

        Else
            'Online reading.

            'Not on local disk, showing from online source
            _chapterImages = _meAPI.getChapterImages(_mangaChapter)
            _chapterImages.Sort()

            fillPageCombo(_chapterImages.images.Count)

        End If

        LoadChapterImage(_imageIndex)

    End Sub

    Private Sub fillPageCombo(ByVal totalPages As Integer)
        For i As Integer = 1 To totalPages
            cboPage.Items.Add(i)
        Next
    End Sub

    Private Function checkOnDisk() As Boolean
        Dim chapterFolder As String = String.Format("{0}\{1}", _settings.DownloadFolder, _mangaName)
        Dim zipFileName As String = String.Format("{0}_-_{1}_-_{2}.zip", _mangaName, _chapterNumber, _chapterTitle.Replace(" ", "_"))

        'Checking that the folder related to this manga exists, then I proceed, 
        '   otherwise it means it is not available online in any format
        If Directory.Exists(chapterFolder) Then
            'Now I check if inside the manga folder there is a zip for this chapter, if not I proceed with next check,
            '   otherwise I set the values as if I've tried to load from local source since the beginning
            If File.Exists(String.Format("{0}\{1}", chapterFolder, zipFileName)) Then
                _mangaChapter = String.Format("{0}\{1}", chapterFolder, zipFileName)
                _flgFromDisk = True

                Return True
            End If

            'Now I check if exists the chapter folder inside the manga one. If so I set the values as if required to show
            '   the local source since the beginning
            If Directory.Exists(String.Format("{0}\{1}", chapterFolder, _chapterNumber)) Then
                _mangaChapter = String.Format("{0}\{1}", chapterFolder, _chapterNumber)
                _flgFromDisk = True
            End If
        End If

        Return False
    End Function

    Private Function LoadChapterImage(ByVal imageIndex As Integer) As Boolean
        Dim localPath As String = ""
        Dim itemsCount As Integer = 0

        If imageIndex < 0 Then _imageIndex = 0

        If _flgFromDisk Then
            itemsCount = _localChapterImages.GetUpperBound(0)
            If imageIndex > itemsCount Then _imageIndex = itemsCount

            localPath = _localChapterImages(_imageIndex)

        Else
            itemsCount = _chapterImages.images.Count - 1
            If imageIndex > itemsCount Then _imageIndex = itemsCount

            localPath = _meAPI.getImage(_chapterImages.images(_imageIndex)(1))

        End If

        If localPath <> "" Then
            Dim localURI As New Uri(localPath)
            Dim image As New BitmapImage(localURI)

            imgPage.Source = image

            image = Nothing

        End If

        If _imageIndex = 0 Then
            imgPrev.Visibility = Windows.Visibility.Collapsed
            imgNext.Visibility = Windows.Visibility.Visible

        ElseIf _imageIndex > 0 And _imageIndex < itemsCount Then
            imgPrev.Visibility = Windows.Visibility.Visible
            imgNext.Visibility = Windows.Visibility.Visible

        Else
            imgPrev.Visibility = Windows.Visibility.Visible
            imgNext.Visibility = Windows.Visibility.Collapsed

        End If

        _flgImageLoading = True
        cboPage.SelectedValue = _imageIndex + 1
        _flgImageLoading = False

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

    Private Sub cboPage_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboPage.SelectionChanged
        If Not _flgImageLoading Then
            If Not cboPage.SelectedValue Is Nothing Then
                Me.Cursor = Cursors.Wait

                _imageIndex = Int(cboPage.SelectedValue) - 1
                LoadChapterImage(_imageIndex)

                Me.Cursor = Cursors.Arrow
            End If
        End If
    End Sub
End Class
