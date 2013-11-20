Option Explicit On

''' <summary>
''' Class holding the parameters being used by the search process
''' </summary>
''' <remarks></remarks>
Public Class SearchParams
    Public Property Title As String

    Public Sub New()
        Title = ""
    End Sub

    Public Sub New(ByVal sTitle As String)
        Title = sTitle
    End Sub

End Class

