Option Explicit On

Public Class SearchParams
    Public Property Title As String

    Public Sub New()
        Title = ""
    End Sub

    Public Sub New(ByVal sTitle As String)
        Title = sTitle
    End Sub

End Class

