Option Explicit On

Imports System.Net
Imports SettingsManager
Imports System.IO
Imports System.Threading
Imports ICSharpCode.SharpZipLib.Core
Imports ICSharpCode.SharpZipLib.Zip

Public Class ChapterImagesDownloader
#Region "Declarations"
    Private WithEvents _webClient As CookieAwareWebClient = Nothing
    Private _chapterImages As String() = Nothing
    Private _settings As AppManager = Nothing
    Private _currentStatus As Status
    Private _mangaTitle As String
    Private _chapterTitle As String
    Private _chapterNumber As Integer
    Private _destinationFolder As String = ""
    Private _zipDestinationFolder As String = ""

    Private _imgBase As String = "http://cdn.mangaeden.com/mangasimg/{0}"
#End Region

#Region "Structure declarations"
    Public Structure Status
        Dim currentDownload As Integer
        Dim currentFileName As String
        Dim totalDownloads As Integer
    End Structure
#End Region

#Region "Events Declarations"
    Public Event CurrentDownload(ByVal sender As Object, ByVal DownloadStatus As Status)
    Public Event DownloadBegin(ByVal sender As Object, ByVal DownloadStatus As Status)
    Public Event DownloadEnd(ByVal sender As Object, ByVal DownloadStatus As Status)
    Public Event PageDownloadProgress(sender As Object, e As System.Net.DownloadProgressChangedEventArgs)
    Public Event PageDownloadStart(sender As Object)
    Public Event PageDownloadEnd(sender As Object, e As System.Net.DownloadDataCompletedEventArgs)
#End Region

#Region "Properties"
    Public ReadOnly Property DownloadedPathOrFile As String
        Get
            If Not _settings Is Nothing Then
                If _settings.DownloadMode = AppManager.eDownloadMode.Folder Then
                    Return _destinationFolder
                Else
                    Return _zipDestinationFolder
                End If
            Else
                Return ""
            End If
        End Get
    End Property
#End Region

#Region "Thread and Delegates"
    Private _run As Thread
#End Region

#Region "Constructor / Distructor"
    Public Sub New(ByVal chapterImages As String(), ByVal mangaTitle As String, ByVal chapterNumber As Integer, ByVal chapterTitle As String)
        _chapterImages = chapterImages
        _mangaTitle = mangaTitle
        _chapterNumber = chapterNumber
        _chapterTitle = chapterTitle

        _settings = New AppManager()
        _settings.LoadSettings()

        _webClient = New CookieAwareWebClient()

        _currentStatus.currentDownload = 0
        _currentStatus.currentFileName = ""
        _currentStatus.totalDownloads = _chapterImages.GetLength(0)

        _run = New Thread(AddressOf ExecuteDownload)

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        _settings = Nothing
        _webClient.Dispose()
    End Sub
#End Region

#Region "Work functions"
    Private Function startDownload() As Boolean
        RaiseEvent DownloadBegin(Me, _currentStatus)

        Return True
    End Function

    Private Function prepareFolder() As Boolean
        Dim result As Boolean = False

        If Not Directory.Exists(_settings.DownloadFolder) Then
            Throw New Exception("Specified download folder doesn't exists.")
            Return False

        Else
            'Checking and creating folder for the manga
            _destinationFolder = String.Format("{0}\{1}", _settings.DownloadFolder, _mangaTitle)
            If Not Directory.Exists(_destinationFolder) Then
                Directory.CreateDirectory(_destinationFolder)
            End If

            If _settings.DownloadMode = AppManager.eDownloadMode.Folder Then
                'As I need to download to a folder I check if it exists or not and eventually I create it
                _destinationFolder = String.Format("{0}\{1}", _destinationFolder, _chapterNumber)
                If Not Directory.Exists(_destinationFolder) Then
                    Directory.CreateDirectory(_destinationFolder)
                End If

            Else
                'Zip file, I save the images on temporary folder then I compress them into the zip file to be stored on real destination folder
                _zipDestinationFolder = _destinationFolder
                _destinationFolder = String.Format("{0}\{1}", My.Computer.FileSystem.SpecialDirectories.Temp, _mangaTitle & _chapterNumber)
                If Not Directory.Exists(_destinationFolder) Then
                    Directory.CreateDirectory(_destinationFolder)
                End If

            End If


            result = True

        End If

        Return result
    End Function

    Private Function downloadChapters() As Boolean
        If Not _chapterImages Is Nothing Then
            'Checking and eventually creating manga and chapter (if folder download and not zip) of the manga
            If prepareFolder() Then

                'Looping trough chapters images to download them
                For Each page As String In _chapterImages
                    'Updating status information holder
                    _currentStatus.currentDownload += 1
                    _currentStatus.currentFileName = page

                    'Preparing source and destination folders
                    Dim sourceFile As String = String.Format(_imgBase, page)
                    Dim extension As String = extractFileExtension(page)
                    Dim destinationFile As String = String.Format("{0}\{1}", _destinationFolder, _currentStatus.currentDownload.ToString("000") & "." & extension)

                    'Raising events
                    RaiseEvent PageDownloadStart(Me)
                    RaiseEvent CurrentDownload(Me, _currentStatus)

                    'Downloading file
                    Try
                        If Not File.Exists(destinationFile) Then
                            _webClient.DownloadFile(New Uri(sourceFile), destinationFile)
                        End If

                    Catch ex As Exception
                        'TODO: Implement eventual error management
                        Debug.Print(ex.Message)
                    End Try

                Next

                'Now if it is required a zip I create it
                If _settings.DownloadMode = AppManager.eDownloadMode.Zip Then
                    'Updating status information holder
                    _currentStatus.currentFileName = "Compressing downloaded images"
                    RaiseEvent CurrentDownload(Me, _currentStatus)

                    _zipDestinationFolder = CreateChapterZIP()

                    'Updating status information holder
                    _currentStatus.currentFileName = "Compression completed"
                    RaiseEvent CurrentDownload(Me, _currentStatus)
                End If

            End If
        End If
        Return True
    End Function

    Private Function CreateChapterZIP()
        Dim destinationFile As String = String.Format("{0}\{1}", _zipDestinationFolder, String.Format("{0}_-_{1}_-_{2}.zip", _mangaTitle, _chapterNumber, _chapterTitle.Replace(" ", "_")))
        Dim fileStream As New FileStream(destinationFile, FileMode.Create)
        Dim zipper As New ZipOutputStream(fileStream)

        ' This setting will strip the leading part of the folder path in the entries, to
        ' make the entries relative to the starting folder.
        ' To include the full path for each entry up to the drive root, assign folderOffset = 0.
        Dim folderOffset As Integer = _destinationFolder.Length + (If(_destinationFolder.EndsWith("\"), 0, 1))

        'Configuring compressor
        zipper.SetLevel(9) 'Max compression level. Valid values 0..9
        zipper.IsStreamOwner = True 'This will take control also of the fileStream object closing it within zipper close

        'Looping trough all the files downloaded
        For Each fl As String In Directory.GetFiles(_destinationFolder)
            If File.Exists(fl) Then
                Dim fi As New FileInfo(fl)

                Dim entryName As String = fl.Substring(folderOffset)  ' Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName)       ' Removes drive from name and fixes slash direction
                Dim newEntry As New ZipEntry(entryName)
                newEntry.DateTime = fi.LastWriteTime            ' Note the zip format stores 2 second granularity

                ' To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
                ' you need to do one of the following: Specify UseZip64.Off, or set the Size.
                ' If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
                ' but the zip will be in Zip64 format which not all utilities can understand.
                '   zipStream.UseZip64 = UseZip64.Off
                newEntry.Size = fi.Length

                zipper.PutNextEntry(newEntry)

                ' Zip the file in buffered chunks
                ' the "using" will close the stream even if an exception occurs
                Dim buffer As Byte() = New Byte(4095) {}
                Using streamReader As FileStream = File.OpenRead(fl)
                    StreamUtils.Copy(streamReader, zipper, buffer)
                End Using
                zipper.CloseEntry()
            End If
        Next

        zipper.Close()
        fileStream.Dispose()

        'Now that the zip is created I remove the temporary downloaded files
        Directory.Delete(_destinationFolder, True)

        Return destinationFile

    End Function

    Private Function endDownload() As Boolean
        _currentStatus.currentDownload = _currentStatus.totalDownloads
        _currentStatus.currentFileName = "Download Completed"
        RaiseEvent DownloadEnd(Me, _currentStatus)

        Return True
    End Function
#End Region

#Region "Web events"

    Private Sub _webClient_DownloadDataCompleted(sender As Object, e As System.Net.DownloadDataCompletedEventArgs) Handles _webClient.DownloadDataCompleted
        RaiseEvent PageDownloadEnd(sender, e)
    End Sub
    Private Sub _webClient_DownloadProgressChanged(sender As Object, e As System.Net.DownloadProgressChangedEventArgs) Handles _webClient.DownloadProgressChanged
        RaiseEvent PageDownloadProgress(sender, e)
    End Sub

#End Region

#Region "Routines"
    Private Function extractFileName(ByVal ImagePath As String) As String
        Dim result As String() = ImagePath.Split("/")
        Return result(result.GetUpperBound(0))
    End Function

    Private Function extractFileExtension(ByVal ImageFile As String) As String
        Dim result As String() = ImageFile.Split(".")
        Return result(result.GetUpperBound(0))
    End Function

    Private Sub ExecuteDownload()
        startDownload()
        downloadChapters()
        endDownload()

    End Sub

    Public Sub BeginDownload()
        _run.SetApartmentState(ApartmentState.MTA)
        _run.Start()
    End Sub
#End Region
End Class
