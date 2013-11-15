Option Explicit On

Imports System.Net
Imports SettingsManager
Imports System.IO
Imports System.Threading

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
            End If


            result = True

        End If

        Return result
    End Function

    Private Function downloadChapters() As Boolean

        If Not _chapterImages Is Nothing Then
            'Checking and eventually creating manga and chapter (if folder download and not zip) of the manga
            If prepareFolder Then

                For Each page As String In _chapterImages
                    _currentStatus.currentDownload += 1
                    _currentStatus.currentFileName = page

                    Dim sourceFile As String = String.Format(_imgBase, page)
                    Dim destinationFile As String = String.Format("{0}\{1}", _destinationFolder, _currentStatus.currentDownload & ".jpg") 'extractFileName(page))

                    RaiseEvent PageDownloadStart(Me)
                    RaiseEvent CurrentDownload(Me, _currentStatus)
                    Try
                        If Not File.Exists(destinationFile) Then
                            _webClient.DownloadFile(New Uri(sourceFile), destinationFile)
                        End If

                    Catch ex As Exception
                        Debug.Print(ex.Message)
                    End Try

                Next

            End If
        End If
        Return True
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
