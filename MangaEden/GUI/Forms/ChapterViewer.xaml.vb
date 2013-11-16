Option Explicit On

Imports MangaEdenAPI
Imports System.Threading
Imports System.IO
Imports ICSharpCode.SharpZipLib.Core
Imports ICSharpCode.SharpZipLib.Zip

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
        lblTitle.Content = String.Format("{0} - {1}: {2}", _mangaName, _chapterNumber, _chapterTitle)

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

        Else
            _chapterImages = _meAPI.getChapterImages(_mangaChapter)
            _chapterImages.Sort()

        End If

        LoadChapterImage(_imageIndex)

    End Sub

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
