Option Explicit On

Imports MangaEdenAPI
Imports SettingsManager
Imports System.Threading
Imports WebElements

Class MainWindow

    Private _api As API

#Region "Events declaration"
    Public Event StatusUpdate(ByVal Status As String)
    Public Event LoadingCompleted()
#End Region

#Region "Thread and delegates declaration"
    Private _loadThread As Thread

    Private Delegate Function LoadingFinished()
    Private _finish As LoadingFinished

    Private Delegate Function ShowDelegate(txt As String)
    Private _loader As ShowDelegate

    Private Delegate Function ApplySettings()
    Private _applySettings As ApplySettings
#End Region

    Public Function firstLoad() As Boolean
        Dispatcher.Invoke(_loader, "Initializing Manga Eden APIs ...")
        _api = New API

        Dispatcher.Invoke(_loader, "Loading application settings ...")
        _settingsManager = New AppManager
        _settingsManager.LoadSettings()

        If _settingsManager.AutomaticLogin Then
            Dispatcher.Invoke(_loader, "Performing user login ...")
            Dim login_result As String = _api.Login(_settingsManager.Username, _settingsManager.Password)

            Dispatcher.Invoke(_loader, "Login result: " & login_result)
            If login_result <> "OK" Then
                'If the login fail I allow the user to see the result stopping the thread for a couple of seconds
                Thread.Sleep(2000)

            Else
                'Loading MyMangas just if login works
                Dispatcher.Invoke(_loader, "Loading my Mangas ...")
                _myMangas = _api.MyMangas()

            End If

        End If

        'Dispatcher.Invoke(_loader, "Downloading updated manga list from MangaEden.com ...")
        _availableMangas = _api.getMangaList(_settingsManager.Language)

        Dispatcher.Invoke(_loader, "Applying app settings and showing mangas ...")
        Dispatcher.Invoke(_applySettings)

        Dispatcher.Invoke(_finish)

        Return True
    End Function

#Region "Form and controls routines"
    Private Sub btAbout_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btAbout.Click
        Dim frmAbout As New About()
        frmAbout.ShowDialog()
        frmAbout = Nothing
    End Sub

    Private Sub btSettings_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles btSettings.Click
        Dim frmSettings As New Settings
        frmSettings.ShowDialog()
        frmSettings = Nothing

        'I update the application according to the following settings
        applyAppSettings()
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        _loadThread.Start()
    End Sub
#End Region

#Region "Constructor / Distructor"
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        'Initializing delegated functions
        _loader = New ShowDelegate(AddressOf showStatus)
        _finish = New LoadingFinished(AddressOf loadingComplete)
        _applySettings = New ApplySettings(AddressOf applyAppSettings)

        'Initializing thread
        _loadThread = New Thread(AddressOf firstLoad)

    End Sub
#End Region

#Region "Delegated functions"
    Private Function applyAppSettings() As Boolean
        mlcMangas.ShowHideMyMangas(_settingsManager.MyMangaOnTop)

        If Not _availableMangas Is Nothing Then
            If _settingsManager.OrderBy = AppManager.eOrderBy.Title Then
                _availableMangas.SortMangasByTitle(MangaList.Sort.Ascending)
            Else
                _availableMangas.SortMangasByLastUpdate(MangaList.Sort.Descending)
            End If

            mlcMangas.showMangaList(_availableMangas)
        End If

        If Not _myMangas Is Nothing Then
            If _settingsManager.OrderBy = AppManager.eOrderBy.Title Then
                _myMangas.SortMyMangasByTitle(MyManga.Sort.Ascending)
            Else
                _myMangas.SortmymangasByLastUpdate(MyManga.Sort.Descending)
            End If

            mlcMangas.showMyMangaList(_myMangas)
        End If

        Return True
    End Function

    Private Function showStatus(ByVal text As String) As Boolean
        RaiseEvent StatusUpdate(text)

        Return True
    End Function

    Private Function loadingComplete() As Boolean
        RaiseEvent LoadingCompleted()

        Return False
    End Function
#End Region

    Private Sub mlcMangas_MangaSelected(sender As Object, e As MangaEdenAPI.MangaBasicInfo) Handles mlcMangas.MangaSelected
        micInfo.ShowMangaInfo(e)
    End Sub

    Private Sub mlcMangas_MyMangaSelected(sender As Object, e As MyMangaInfo) Handles mlcMangas.MyMangaSelected
        micInfo.ShowMyMangaInfo(e)
    End Sub
End Class
