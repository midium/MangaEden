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
    ''' <param name="bRemotePath">(Optional, default true) This parameters tells the routine if the path given is a remote (web) path or a local (disk) path.
    ''' According to this it splits the string with the proper path separator "/" or "\"</param>
    ''' <returns>A string value containing just the path without the file name</returns>
    ''' <remarks></remarks>
    Public Function extractFileName(ByVal Path As String, Optional bRemotePath As Boolean = True) As String
        Dim result As String()

        If bRemotePath Then
            result = Path.Split("/")
        Else
            result = Path.Split("\")
        End If

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

    Public Function extractPathName(ByVal Path As String) As String
        Dim result As String() = Path.Split("\")
        If result(result.GetUpperBound(0)).Trim = "" Then
            'If the latest element is empty then the path give should have been similar to this: c:\test\
            '   I then get the element just before the latest
            Return result(result.GetUpperBound(0) - 1).Trim

        Else
            'If the latest element is not empty then the path give should have been similar to this: c:\test
            '   I then get the latest element
            Return result(result.GetUpperBound(0)).Trim

        End If
    End Function

End Class
