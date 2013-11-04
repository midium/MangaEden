Option Explicit On

Imports System.IO
Imports System.Xml.Serialization

Public Class AppManager
    Private _settings As AppSettings = Nothing
    Private _settingsPath As String = My.Application.Info.DirectoryPath & "\settings.dat"

    Enum eLanguages
        English = 0
        Italian = 1
    End Enum

    Enum eDownloadMode
        Folder = 0
        Zip = 1
    End Enum

    Enum eOrderBy
        Title = 0
        LastUpdate = 1
    End Enum

    Public Sub New()
        LoadSettings()
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        If Not _settings Is Nothing Then
            _settings = Nothing
        End If
    End Sub

#Region "Properties"
    Public Property Language As eLanguages
        Get
            Return _settings.Language
        End Get
        Set(value As eLanguages)
            _settings.Language = value
        End Set
    End Property

    Public Property DownloadFolder As String
        Get
            Return _settings.DownloadFolder
        End Get
        Set(value As String)
            _settings.DownloadFolder = value
        End Set
    End Property

    Public Property DownloadMode As eDownloadMode
        Get
            Return _settings.DownloadMode
        End Get
        Set(value As eDownloadMode)
            _settings.DownloadMode = value
        End Set
    End Property

    Public Property AutomaticLogin As Boolean
        Get
            Return _settings.AutomaticLogin
        End Get
        Set(value As Boolean)
            _settings.AutomaticLogin = value
        End Set
    End Property

    Public Property MyMangaOnTop As Boolean
        Get
            Return _settings.MyMangaOnTop
        End Get
        Set(value As Boolean)
            _settings.MyMangaOnTop = value
        End Set
    End Property

    Public Property Username As String
        Get
            Return _settings.UserName
        End Get
        Set(value As String)
            _settings.UserName = value
        End Set
    End Property

    Public Property Password As String
        Get
            Return _settings.Password
        End Get
        Set(value As String)
            _settings.Password = value
        End Set
    End Property

    Public Property OrderBy As eOrderBy
        Get
            Return _settings.OrderBy
        End Get
        Set(value As eOrderBy)
            _settings.OrderBy = value
        End Set
    End Property

    Public Property NewChapterDays As Integer
        Get
            Return _settings.NewChapterDays
        End Get
        Set(value As Integer)
            _settings.NewChapterDays = value
        End Set
    End Property
#End Region

#Region "Setting management"
    Public Sub SaveSettings()
        Try
            If Not _settings Is Nothing Then
                Dim serializedData As String = SerializeData()

                If SerializeData() = "" Then
                    MsgBox("There have been errors saving the application settings: serialization failed", MsgBoxStyle.Critical)
                    Exit Sub

                Else
                    Dim writer As StreamWriter
                    writer = New StreamWriter(_settingsPath)
                    writer.Write(serializedData)
                    writer.Close()
                    writer.Dispose()

                End If
            End If
        Catch ex As Exception
            MsgBox("There have been errors saving the application settings: " & ex.Message, MsgBoxStyle.Critical)

        End Try

    End Sub

    Public Sub LoadSettings()
        'I preventively set the default settings
        DefaultSettings()

        'Now I try to load any saved settings
        Try
            If File.Exists(_settingsPath) Then
                'There are saved settings, I try to open them
                Dim xml_serializer As New XmlSerializer(GetType(AppSettings))
                Dim string_reader As New StreamReader(_settingsPath)

                _settings = DirectCast(xml_serializer.Deserialize(string_reader), AppSettings)

                string_reader.Close()
                string_reader.Dispose()
            End If

        Catch ex As Exception
            MsgBox("There have been errors loading the application settings: " & ex.Message, MsgBoxStyle.Critical)
        End Try

    End Sub

    Private Sub DefaultSettings()
        If _settings Is Nothing Then
            _settings = New AppSettings
        End If

        _settings.Language = AppManager.eLanguages.Italian
        _settings.DownloadFolder = My.Computer.FileSystem.SpecialDirectories.MyPictures
        _settings.DownloadMode = AppManager.eDownloadMode.Zip
        _settings.AutomaticLogin = True
        _settings.MyMangaOnTop = False
        _settings.UserName = ""
        _settings.Password = ""
        _settings.OrderBy = eOrderBy.LastUpdate
        _settings.NewChapterDays = 5

    End Sub

    Private Function SerializeData() As String
        Dim result As String = ""

        Try
            Dim xml_serializer As New XmlSerializer(GetType(AppSettings))
            Dim string_writer As New StringWriter
            xml_serializer.Serialize(string_writer, _settings)

            result = string_writer.ToString()

            string_writer.Close()
            string_writer.Dispose()

        Catch ex As Exception
            result = ""

        End Try

        Return result
    End Function

#End Region
End Class
