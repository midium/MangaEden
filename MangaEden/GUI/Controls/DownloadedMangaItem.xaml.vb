Option Explicit On

Imports System.IO
Imports CommonRoutines

Public Class DownloadedMangaItem

#Region "Declarations"
    Private _chapterNumber As Integer
    Private _chapterName As String
    Private _chapterDate As String
    Private _chapterFileOrFolder As String
    Private _mangaName As String
    Private _iID As Integer

    Public Event ElementDeleted(ByVal iID As Integer, ByVal sender As Object)
#End Region

#Region "Properties"
    Public Property iID As Integer
        Get
            Return _iID
        End Get
        Set(value As Integer)
            _iID = value
        End Set
    End Property

    Public Property ChapterNumber As Integer
        Get
            Return _chapterNumber
        End Get
        Set(value As Integer)
            _chapterNumber = value

            UpdateInterface()
        End Set
    End Property

    Public Property MangaName As String
        Get
            Return _mangaName
        End Get
        Set(value As String)
            _mangaName = value
        End Set
    End Property

    Public Property ChapterName As String
        Get
            Return _chapterName
        End Get
        Set(value As String)
            _chapterName = value

            UpdateInterface()
        End Set
    End Property

    Public Property DownloadDate As String
        Get
            Return _chapterDate
        End Get
        Set(value As String)
            _chapterDate = value

            UpdateInterface()
        End Set
    End Property

    Public Property FileOrFolderName As String
        Get
            Return _chapterFileOrFolder
        End Get
        Set(value As String)
            _chapterFileOrFolder = value

            UpdateInterface()
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New(ByVal iID As Integer, ByVal iChapterNumber As Integer, ByVal sChapterName As String, ByVal sDownloadDate As String,
                   ByVal sFileOrFolder As String, ByVal sMangaName As String, ByVal localEventHandler As ElementDeletedEventHandler)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _iID = iID
        _chapterName = sChapterName
        _chapterNumber = iChapterNumber
        _chapterDate = sDownloadDate
        _chapterFileOrFolder = sFileOrFolder
        _mangaName = sMangaName

        UpdateInterface()

        AddHandler ElementDeleted, localEventHandler

    End Sub
#End Region

#Region "Routines"
    Private Sub UpdateInterface()
        txtChapterNumber.Text = ChapterNumber
        txtChapterDate.Text = DownloadDate
        txtChapterName.Text = ChapterName
    End Sub

    Private Sub PlayChapter()
        Dim frmPlay As ChapterViewer = Nothing
        Dim sourcePath As String = ""

        sourcePath = _chapterFileOrFolder

        frmPlay = New ChapterViewer(sourcePath, _mangaName, _chapterName, _chapterNumber, True)

        frmPlay.ShowDialog()
        frmPlay = Nothing
    End Sub

    Private Sub DeleteChapter()
        If MsgBox("You are about to delete the downloaded chapter. This action can't be undone." & vbCrLf & "Are you sure you want to remove it?", MsgBoxStyle.YesNo + MsgBoxStyle.Critical) = MsgBoxResult.Yes Then
            Try
                Dim ioSubs As New IORoutines
                If ioSubs.isFolder(_chapterFileOrFolder) Then
                    'It has been download to folder
                    Directory.Delete(_chapterFileOrFolder, True)
                Else
                    'It has been download as zip file
                    File.Delete(_chapterFileOrFolder)
                End If

                MsgBox(String.Format("Chapter {0} of manga {1} correctly deleted.", _chapterNumber, _mangaName), MsgBoxStyle.Information)

                'Me.Visibility = Windows.Visibility.Collapsed
                RaiseEvent ElementDeleted(_iID, Me)

            Catch ex As Exception
                MsgBox("There have been problems removing the selected chapter." & vbCrLf & "Please check that the chapter isn't open by other programs")

            End Try

        End If
    End Sub
#End Region

#Region "Components routines"
    Private Sub imgPlay_MouseMove(sender As Object, e As System.Windows.Input.MouseEventArgs) Handles imgPlay.MouseMove
        imgPlay.Cursor = Cursors.Hand
    End Sub

    Private Sub imgPlay_MouseUp(sender As Object, e As System.Windows.Input.MouseButtonEventArgs) Handles imgPlay.MouseUp
        PlayChapter()
    End Sub

    Private Sub imgDelete_MouseMove(sender As Object, e As System.Windows.Input.MouseEventArgs) Handles imgDelete.MouseMove
        imgDelete.Cursor = Cursors.Hand
    End Sub

    Private Sub imgDelete_MouseUp(sender As Object, e As System.Windows.Input.MouseButtonEventArgs) Handles imgDelete.MouseUp
        DeleteChapter()
    End Sub

#End Region

End Class
