Option Explicit On

Imports MangaEdenAPI
Imports System.Windows.Threading
Imports WPFFolderBrowser

Public Class Settings

    Private mangaAPI As API

    Private Function SetSettings() As Boolean
        Dim result As Boolean = True

        If rbtIT.IsChecked Then
            _settingsManager.Language = SettingsManager.AppManager.eLanguages.Italian
        End If
        If rbtUK.IsChecked Then
            _settingsManager.Language = SettingsManager.AppManager.eLanguages.English
        End If

        If txtFolder.Text.Trim = "" Then
            result = False
        Else
            _settingsManager.DownloadFolder = txtFolder.Text.Trim
        End If

        If rbtFolder.IsChecked Then
            _settingsManager.DownloadMode = SettingsManager.AppManager.eDownloadMode.Folder
        End If
        If rbtZip.IsChecked Then
            _settingsManager.DownloadMode = SettingsManager.AppManager.eDownloadMode.Zip
        End If

        _settingsManager.AutomaticLogin = chkAutoLogin.IsChecked
        _settingsManager.MyMangaOnTop = chkMyMangaOnTop.IsChecked
        _settingsManager.Username = txtUser.Text.Trim
        _settingsManager.Password = txtPassword.Password.Trim

        _settingsManager.OrderBy = cboOrderBy.SelectedIndex
        _settingsManager.NewChapterDays = Val(cboNewChapters.SelectedValue.Content)

        Return result
    End Function

    Private Sub btSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btSave.Click
        If SetSettings() Then
            _settingsManager.SaveSettings()

            MsgBox("Please note that some of the settings need the application to be restarted or waiting for next manga list update to be raised.", MsgBoxStyle.Information)

            Me.Close()
        Else
            MsgBox("Please set a valid download folder to proceed", MsgBoxStyle.Critical)

        End If
    End Sub

    Private Sub btDiscard_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btDiscard.Click
        Me.Close()
    End Sub

    Private Sub btLogin_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btLogin.Click
        If txtPassword.Password.Trim = "" Or txtUser.Text.Trim = "" Then
            MsgBox("You must specify username and password to login!", MsgBoxStyle.Exclamation)

        Else

            Me.Cursor = Cursors.Wait

            mangaAPI = New API
            lblVerify.Content = mangaAPI.Login(txtUser.Text.Trim, txtPassword.Password.Trim)

            Me.Cursor = Cursors.Arrow
        End If
    End Sub

    Private Sub btSave_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles btSave.Loaded

        If (_settingsManager.Language = SettingsManager.AppManager.eLanguages.Italian) Then
            rbtIT.IsChecked = True
        Else
            rbtUK.IsChecked = True
        End If

        txtFolder.Text = _settingsManager.DownloadFolder

        If (_settingsManager.DownloadMode = SettingsManager.AppManager.eDownloadMode.Zip) Then
            rbtZip.IsChecked = True
        Else
            rbtFolder.IsChecked = True
        End If

        chkAutoLogin.IsChecked = _settingsManager.AutomaticLogin
        chkMyMangaOnTop.IsChecked = _settingsManager.MyMangaOnTop

        txtUser.Text = _settingsManager.Username
        txtPassword.Password = _settingsManager.Password

        cboOrderBy.SelectedIndex = _settingsManager.OrderBy

        Dim iCount As Integer = 0
        For Each i As ComboBoxItem In cboNewChapters.Items
            If Val(i.Content) = _settingsManager.NewChapterDays Then
                Exit For
            End If
            iCount += 1
        Next
        cboNewChapters.SelectedIndex = iCount
        
    End Sub

    Private Sub btFolder_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btFolder.Click
        Dim dlg As New WPFFolderBrowserDialog("Select Folder")
        If dlg.ShowDialog Then
            txtFolder.Text = dlg.FileName
        End If
    End Sub
End Class
