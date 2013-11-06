Option Explicit On

Imports Newtonsoft.Json
Imports WebElements
Imports System.IO

Public Class API
    Implements MangaEdenAPI.IApi

    Private DataDownloader As Downloader
    Private MyMangaLogin As Login

    Private _baseUrl As String = "https://www.mangaeden.com/api{0}"
    Private _mangaList As String = "/list/{0}"
    Private _mangaInfo As String = "/manga/{0}"
    Private _chapter As String = "/chapter/{0}"
    Private _login As String = "https://www.mangaeden.com/ajax/login/?username={0}&password={1}"
    Private _logout As String = "https://www.mangaeden.com/ajax/logout/"
    Private _myManga As String = "https://www.mangaeden.com/api/mymanga"
    Private _imgBase As String = "http://cdn.mangaeden.com/mangasimg/{0}"

    Private _isLoggedIn As Boolean = False

    Private _username As String = ""
    Private _password As String = ""

    Public Function getMangaList(ByVal eLanguage As IApi.Languages) As MangaList Implements IApi.getMangaList
        'Downloading the page
        Dim jsonPage As String = DataDownloader.getWebPage(String.Format(_baseUrl, String.Format(_mangaList, CInt(eLanguage))))

        'As I'm loading the manga list from MangaEden API, I'm being returned JSON so I need to deserialize it.
        Dim Mangas As MangaList = JsonConvert.DeserializeObject(Of MangaList)(jsonPage)

        Return Mangas
    End Function

    Public Function getMangaDetails(ByVal mangaID As String) As MangaDetails Implements IApi.getMangaDetails
        'Downloading the page
        Dim jsonPage As String = DataDownloader.getWebPage(String.Format(_baseUrl, String.Format(_mangaInfo, mangaID)))

        'As I'm loading the manga list from MangaEden API, I'm being returned JSON so I need to deserialize it.
        Dim Details As MangaDetails = JsonConvert.DeserializeObject(Of MangaDetails)(jsonPage)

        Return Details
    End Function

    Public Function Login(ByVal UserName As String, ByVal Password As String) As String Implements IApi.Login
        _username = UserName
        _password = Password

        Dim sLoginUrl As String = String.Format(_login, UserName, Password)

        Dim result As String = MyMangaLogin.doLogin(sLoginUrl)

        If result = "OK" Then
            _isLoggedIn = True
        Else
            _isLoggedIn = False
        End If

        Return result
    End Function

    Public Function Logout() As Boolean Implements IApi.Logout
        Return MyMangaLogin.doLogout(_logout)
    End Function

    Public Function getMyMangas() As MyManga Implements IApi.MyMangas

        'Doing login
        Dim sLoginUrl As String = String.Format(_login, _username, _password)

        'Downloading the page
        Dim jsonPage As String = DataDownloader.getMyMangaWebPage(sLoginUrl, _myManga)

        'As I'm loading the manga list from MangaEden API, I'm being returned JSON so I need to deserialize it.
        Dim Mangas As MyManga = JsonConvert.DeserializeObject(Of MyManga)(jsonPage)

        Return Mangas

    End Function

    ''' <summary>
    ''' This routine download an image using the MangaEden API protocol into a temporary folder
    ''' </summary>
    ''' <param name="ImagePath">The remote image path</param>
    ''' <returns>The path to the temporary file</returns>
    ''' <remarks></remarks>
    Public Function getImage(ByVal ImagePath As String) As String
        Dim result As String = ""
        Dim remoteImagePath As String = String.Format(_imgBase, ImagePath)
        Dim localImagePath As String = String.Format("{0}\{1}", System.IO.Path.GetTempPath, extractFileName(ImagePath))

        If File.Exists(localImagePath) Then
            'If the image has already been downloaded then I simply return the local path
            result = localImagePath
        Else
            'If the image hasn't already been downloaded then I download it
            If DataDownloader.getImage(remoteImagePath, localImagePath) Then
                result = localImagePath
            End If
        End If

        Return result
    End Function

    Public Sub New()
        DataDownloader = New Downloader
        MyMangaLogin = New Login
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        DataDownloader = Nothing
    End Sub
End Class
