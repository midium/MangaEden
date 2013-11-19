Option Explicit On

Public Class IORoutines
    Public Function extractFileName(ByVal ImagePath As String) As String
        Dim result As String() = ImagePath.Split("/")
        Return result(result.GetUpperBound(0))
    End Function

    Public Function extractFileExtension(ByVal ImagePath As String) As String
        Dim result As String() = ImagePath.Split(".")
        Return result(result.GetUpperBound(0))
    End Function
End Class
