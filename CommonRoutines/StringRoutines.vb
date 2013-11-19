Option Explicit On

Public Class StringRoutines
    Public Function CapitalizeFirstLetter(ByVal stringValue As String) As String
        Return StrConv(stringValue, VbStrConv.ProperCase)
    End Function
End Class
