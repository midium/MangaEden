Option Explicit On

Imports SettingsManager

Public Class MangaBasicInfo
    Public a As String 'alias
    Public ld As Double 'last chapter date
    Public i As String 'ID
    Public h As Integer 'hits
    Public s As Integer 'status
    Public im As String 'image
    Public t As String 'title

    Private _settings As AppManager

    Enum MangaStatus
        NotFinished = 2
        Finished = 1
    End Enum

    Public ReadOnly Property Title As String
        Get
            Return t
        End Get
    End Property

    Public ReadOnly Property Image As String
        Get
            Return im
        End Get
    End Property

    Public ReadOnly Property Status As MangaStatus
        Get
            Return s
        End Get
    End Property

    Public ReadOnly Property Hits As Integer
        Get
            Return h
        End Get
    End Property

    Public ReadOnly Property ID As String
        Get
            Return i
        End Get
    End Property

    Public ReadOnly Property LastChapterDate As String
        Get
            Return UnixTimeStamp_To_Date(ld, "dd-MM-yyyy")
        End Get
    End Property

    Public ReadOnly Property [Alias] As String
        Get
            Return a
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return t
    End Function

    Public ReadOnly Property IsNew As Boolean
        Get
            Dim newDateLimit As Integer = 0 - _settings.NewChapterDays
            If (UnixTimeStamp_To_DateTime(ld) >= Now().Date.AddDays(newDateLimit)) Then
                'It is within the day limits
                Return True
            Else
                'It is not within the day limits
                Return False
            End If
        End Get
    End Property

    Public Sub New()
        _settings = New AppManager
        _settings.LoadSettings()
    End Sub
End Class
