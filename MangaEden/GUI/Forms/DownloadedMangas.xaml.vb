Option Explicit On

Imports System.IO
Imports CommonRoutines
Imports SettingsManager

Public Class DownloadedMangas

    Private _settings As AppManager

    'TODO: Try to implement ItemControl solution (http://rachel53461.wordpress.com/2011/09/17/wpf-itemscontrol-example/)
    '<ItemsControl ItemsSource="{Binding MyCollection}">
    '    <ItemsControl.ItemTemplate>
    '        <DataTemplate>
    '            <Button Content="{Binding }" />
    '        </DataTemplate>
    '    </ItemsControl.ItemTemplate>
    '</ItemsControl>

    Private Sub btClose_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btClose.Click
        Me.Close()
    End Sub

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
                    iCnt = 1
                    Dim sManga As String = ioSubs.extractPathName(fld)

                    Dim mangaTab As New TabItem
                    mangaTab.Header = sManga
                    mangaTab.Name = sManga.Replace(" ", "")
                    tabMangas.Items.Add(mangaTab)

                    Dim scroller As New ScrollViewer
                    mangaTab.Content = scroller

                    Dim childGrid As New Grid
                    scroller.Content = childGrid

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

                                Dim dmItem As New DownloadedMangaItem(sChapterNumber, sChapterName, fldrInfo.CreationTime().ToString("dd-MM-yyyy"), chapterFolder, sManga)
                                dmItem.VerticalAlignment = Windows.VerticalAlignment.Top
                                dmItem.Margin = New Thickness(0, iCnt + 22, 0, 0)
                                childGrid.Children.Add(dmItem)

                                fldrInfo = Nothing
                                dmItem = Nothing

                                iCnt += 22

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

                                    Dim dmItem As New DownloadedMangaItem(sChapterNumber, sChapterName, flInfo.CreationTime().ToString("dd-MM-yyyy"), fl, sManga)
                                    dmItem.VerticalAlignment = Windows.VerticalAlignment.Top
                                    dmItem.Margin = New Thickness(0, iCnt + 22, 0, 0)
                                    childGrid.Children.Add(dmItem)

                                    flInfo = Nothing
                                    dmItem = Nothing

                                    iCnt += 22
                                End If
                            End If
                        Next
                    End If

                Next

            End If
        End If
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
