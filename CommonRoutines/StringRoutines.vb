Option Explicit On

''' <summary>
''' This class contains strings routines
''' 
''' Developed by: Matteo Loro
''' Email: matteo.loro@gmail.com
''' </summary>
''' <remarks></remarks>
Public Class StringRoutines
    ''' <summary>
    ''' This function returns the camel case version of the given string
    ''' </summary>
    ''' <param name="stringValue">The string to be converted into camel case format</param>
    ''' <returns>The camel case format of the given string</returns>
    ''' <remarks></remarks>
    Public Function CapitalizeFirstLetter(ByVal stringValue As String) As String
        Return StrConv(stringValue, VbStrConv.ProperCase)
    End Function
End Class
