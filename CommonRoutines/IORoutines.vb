Option Explicit On

''' <summary>
''' This class contains IO, file, directory routines
''' 
''' Developed by: Matteo Loro
''' Email: matteo.loro@gmail.com
''' </summary>
''' <remarks></remarks>
Public Class IORoutines
    ''' <summary>
    ''' This function extract just the path from a full file path string
    ''' </summary>
    ''' <param name="Path">The full file path from where extract the path</param>
    ''' <returns>A string value containing just the path without the file name</returns>
    ''' <remarks></remarks>
    Public Function extractFileName(ByVal Path As String) As String
        Dim result As String() = Path.Split("/")
        Return result(result.GetUpperBound(0))
    End Function

    ''' <summary>
    ''' This function extract the file name from a full file path string
    ''' </summary>
    ''' <param name="Path">The full file path from where extract the path</param>
    ''' <returns>A string value containing just the file name without its path</returns>
    ''' <remarks></remarks>
    Public Function extractFileExtension(ByVal Path As String) As String
        Dim result As String() = Path.Split(".")
        Return result(result.GetUpperBound(0))
    End Function
End Class
