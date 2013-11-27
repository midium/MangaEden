Option Explicit On

Imports MangaEdenAPI
Imports System.Threading
Imports SettingsManager
Imports System.IO
Imports CommonRoutines

Public Class DownloadPage

#Region "Declarations"
    Private _settings As AppManager
    Private _chapters As MangaChaptersDetails()
    Private _chapter As MangaChaptersDetails
    Private _mangaName As String
    Private _flagRunned As Boolean = False
#End Region

#Region "Thread and delegates"
    Private _threads As Thread

    'Single download delegates
    Private Delegate Function ShowDownload(ByVal objectContainer As Object, ByVal i As Integer, ByVal chapterInfo As MangaChaptersDetails, ByVal mangaName As String)
    Private _showDownload As ShowDownload

    Private Delegate Function ShowOldDownload(ByVal objectContainer As Object, ByVal i As Integer, ByVal chapterNumber As String, ByVal chapterName As String, ByVal mangaName As String, ByVal sPathOrFile As String)
    Private _showOldDownload As ShowOldDownload

    Private Delegate Function PrepareSingleDownload()
    Private _singleDownload As PrepareSingleDownload

    Private Delegate Function PrepareMultiDownload()
    Private _multiDownload As PrepareMultiDownload

#End Region

    Private Sub DownloadPage_ContentRendered(sender As Object, e As EventArgs) Handles Me.ContentRendered
        _flagRunned = True

        If Not _chapter Is Nothing Then
            _threads = New Thread(AddressOf InitiateSingleDownload)
            _threads.Start()
        End If

        If Not _chapters Is Nothing Then
            _threads = New Thread(AddressOf InitiateMultiDownload)
            _threads.Start()
        End If

        If (_chapter Is Nothing) And (_chapters Is Nothing) Then
            _threads = New Thread(AddressOf LoadDownloaded)
            _threads.Start()
        End If

    End Sub

#Region "Routines"
    Private Sub LoadDownloaded()
        Dim iCnt As Integer = 1
        Dim ioSubs As New IORoutines

        'I do something just if the dowload folder exists and has child folders related to mangas
        If Directory.Exists(_settings.DownloadFolder) Then
            'Collecting all available sub directories
            Dim fldrs As String() = Directory.GetDirectories(_settings.DownloadFolder)

            'I proceed just if there are some folders
            If fldrs IsNot Nothing AndAlso fldrs.Count > 0 Then

                For Each fld As String In fldrs
                    Dim sManga As String = ioSubs.extractPathName(fld)

                    'Collecting all files and folders under this path
                    Dim fls As String() = Directory.GetFiles(fld)
                    Dim subfldrs As String() = Directory.GetDirectories(fld)

                    'Checking if the subfolders are available
                    If subfldrs IsNot Nothing AndAlso subfldrs.Count > 0 Then
                        'Looping trough chapter folders (or at least supposed to be chapter folders)
                        For Each chapterFolder As String In subfldrs
                            Dim splittedFolder As String() = ioSubs.extractPathName(chapterFolder).Split("-")
                            Dim sChapterNumber As String = splittedFolder(0).Replace("_", "")
                            Dim sChapterName As String = splittedFolder(1).Replace("_", " ").Replace(".zip", "").Trim()


                            'The chapter path name must be a number
                            If IsNumeric(sChapterNumber) Then
                                Dispatcher.Invoke(_showOldDownload, gridContainer, iCnt, sChapterNumber, sChapterName, sManga, chapterFolder)
                                iCnt += 10
                            End If
                        Next
                    End If

                    'Now checking if files available
                    If fls IsNot Nothing AndAlso fls.Count > 0 Then
                        'Looping trough files
                        For Each fl As String In fls
                            'Checking if the file is a zip
                            If ioSubs.extractFileExtension(fl).ToLower = "zip" Then
                                Dim splittedFile As String() = fl.Split("-")
                                Dim sChapterNumber As String = splittedFile(1).Replace("_", "")
                                Dim sChapterName As String = splittedFile(2).Replace("_", " ").Replace(".zip", "").Trim()

                                'The chapter path name must be a number
                                If IsNumeric(sChapterNumber) Then
                                    Dispatcher.Invoke(_showOldDownload, gridContainer, iCnt, sChapterNumber, sChapterName, sManga, fl)
                                    iCnt += 10
                                End If
                            End If
                        Next
                    End If

                Next

            End If
        End If
    End Sub

#Region "Single Download"
    Private Sub InitiateSingleDownload()
        Dispatcher.Invoke(_singleDownload)
    End Sub

    Private Function SingleDownload()
        Dispatcher.Invoke(_showDownload, gridContainer, 1, _chapter, _mangaName)

        Return True
    End Function

#End Region

#Region "Multi-Download"
    Private Sub InitiateMultiDownload()
        Dispatcher.Invoke(_multiDownload)
    End Sub

    Private Function MultiDownload()
        Dim iCnt As Integer = 1

        For Each chapt As MangaChaptersDetails In _chapters
            Dispatcher.Invoke(_showDownload, gridContainer, iCnt, chapt, _mangaName)

            iCnt += 10
        Next

        Return True
    End Function

#End Region

    Private Function NewDownload(ByVal objectContainer As Object, ByVal i As Integer, ByVal chapterInfo As MangaChaptersDetails, ByVal mangaName As String) As Boolean
        'Loading the new download module and showing it into the page
        Dim download As New ChapterDownloader
        objectContainer.Children.Add(download)
        download.MangaTitle = mangaName
        download.ChapterInfo = chapterInfo

        download.Name = "Download" & i
        download.Width = objectContainer.Width - 10
        download.Height = 87
        download.Margin = New Thickness(10, i * 10, 10, 0)
        download.VerticalAlignment = Windows.VerticalAlignment.Top

        download = Nothing

        Return True
    End Function

    Private Function OldDownload(ByVal objectContainer As Object, ByVal i As Integer, ByVal chapterNumber As String, ByVal chapterName As String, ByVal mangaName As String, ByVal sPathOrFile As String) As Boolean
        'Loading the new download module and showing it into the page
        Dim download As New ChapterDownloader(True)
        objectContainer.Children.Add(download)
        download.MangaTitle = mangaName

        Dim chp As New MangaChaptersDetails()
        chp.Number = chapterNumber
        chp.Title = chapterName
        download.ChapterInfo = chp
        download.DownloadedPath = sPathOrFile

        download.Name = "Download" & i
        download.Width = objectContainer.Width - 10
        download.Height = 87
        download.Margin = New Thickness(10, i * 10, 10, 0)
        download.VerticalAlignment = Windows.VerticalAlignment.Top

        download = Nothing

        Return True
    End Function
#End Region

#Region "Initiation"
    Private Sub initDelegates()
        _showDownload = New ShowDownload(AddressOf NewDownload)
        _singleDownload = New PrepareSingleDownload(AddressOf SingleDownload)
        _multiDownload = New PrepareMultiDownload(AddressOf MultiDownload)
        _showOldDownload = New ShowOldDownload(AddressOf OldDownload)
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        initDelegates()
        _settings = New AppManager
        _settings.LoadSettings()

    End Sub

    Public Sub New(ByVal MangaName As String, ByVal Chapter As MangaChaptersDetails)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        initDelegates()
        _chapter = Chapter
        _mangaName = MangaName

        _settings = New AppManager
        _settings.LoadSettings()
    End Sub

    Public Sub New(ByVal MangaName As String, ByVal Chapters() As MangaChaptersDetails)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        initDelegates()
        _chapters = Chapters
        _mangaName = MangaName

        _settings = New AppManager
        _settings.LoadSettings()

    End Sub
#End Region

    Private Sub btClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btClose.Click
        Me.Close()
    End Sub
End Class
