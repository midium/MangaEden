Option Explicit On

Imports System.IO
Imports CommonRoutines
Imports SettingsManager

Public Class DownloadedMangas

    Private _settings As AppManager

    Private Sub btClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btClose.Click
        Me.Close()
    End Sub

    Private Sub LoadDownloaded()
        Dim iCnt As Integer = 0
        Dim ioSubs As New IORoutines

        'I do something just if the dowload folder exists and has child folders related to mangas
        If Directory.Exists(_settings.DownloadFolder) Then
            'Collecting all available sub directories
            Dim fldrs As String() = Directory.GetDirectories(_settings.DownloadFolder)

            'I proceed just if there are some folders
            If fldrs IsNot Nothing AndAlso fldrs.Count > 0 Then

                For Each fld As String In fldrs
                    iCnt = 0
                    Dim sManga As String = ioSubs.extractPathName(fld)

                    Dim mangaTab As New TabItem
                    mangaTab.Header = sManga
                    mangaTab.Name = sManga.Replace(" ", "")
                    tabMangas.Items.Add(mangaTab)

                    Dim childGrid As New Grid
                    mangaTab.Content = childGrid

                    Dim list As New ListBox
                    childGrid.Children.Add(list)
                    list.Padding = New Thickness(0, 0, 0, 0)

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
                                Dim fldrInfo As New DirectoryInfo(chapterFolder)

                                Dim dmItem As New DownloadedMangaItem(iCnt, sChapterNumber, sChapterName,
                                                                      fldrInfo.CreationTime().ToString("dd-MM-yyyy"),
                                                                      chapterFolder, sManga, AddressOf DeleteRow)
                                dmItem.VerticalAlignment = Windows.VerticalAlignment.Top
                                dmItem.Padding = New Thickness(0, 0, 0, 0)
                                dmItem.Margin = New Thickness(0, 0, 0, 0)
                                list.Items.Add(dmItem)

                                fldrInfo = Nothing
                                dmItem = Nothing

                                iCnt += 1

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
                                    Dim flInfo As New FileInfo(fl)

                                    Dim dmItem As New DownloadedMangaItem(iCnt, sChapterNumber, sChapterName,
                                                                          flInfo.CreationTime().ToString("dd-MM-yyyy"), fl, sManga,
                                                                          AddressOf DeleteRow)
                                    dmItem.VerticalAlignment = Windows.VerticalAlignment.Top
                                    dmItem.Padding = New Thickness(0, 0, 0, 0)
                                    dmItem.Margin = New Thickness(0, 0, 0, 0)

                                    list.Items.Add(dmItem)

                                    flInfo = Nothing
                                    dmItem = Nothing

                                    iCnt += 1
                                End If
                            End If
                        Next
                    End If

                Next

            End If
        End If
    End Sub

    ''' <summary>
    ''' This is the delegated function being called when DownlaodedMangaItem raise its ElementDelete funtion
    ''' </summary>
    ''' <param name="RowID">(Integer) This is the ID of the row</param>
    ''' <param name="sender">(Object) This is the DownloadedMangaItem object</param>
    ''' <remarks></remarks>
    Private Sub DeleteRow(ByVal RowID As Integer, ByVal sender As Object)

        'Retrieving containing list and removing the element
        Dim lst As ListBox = DirectCast(sender.Parent, ListBox)
        lst.Items.Remove(sender)

    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _settings = New AppManager
        _settings.LoadSettings()

    End Sub

    Private Sub DownloadedMangas_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        LoadDownloaded()
    End Sub
End Class
