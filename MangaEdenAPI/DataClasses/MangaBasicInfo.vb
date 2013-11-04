Option Explicit On

Public Class MangaBasicInfo
    Public a As String 'alias
    Public ld As Double 'last chapter date
    Public i As String 'ID
    Public h As Integer 'hits
    Public s As Integer 'status
    Public im As String 'image
    Public t As String 'title

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

    Public ReadOnly Property Status As Integer
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

    Public ReadOnly Property IsNew(ByVal daysOld As Integer) As Boolean
        Get
            If Format(Today.AddDays(Val("-" & daysOld)), "dd-MM-yyyy") <= LastChapterDate Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property


End Class
