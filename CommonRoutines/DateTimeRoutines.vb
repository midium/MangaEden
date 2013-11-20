Option Explicit On

''' <summary>
''' This class contains common date / time routines of general usage.
''' 
''' Developed by: Matteo Loro
''' Email: matteo.loro@gmail.com
''' 
''' </summary>
''' <remarks></remarks>
Public Class DateTimeRoutines
    ''' <summary>
    ''' This function converts unix timestamp value into a date/time string with the possibility to set its format
    ''' </summary>
    ''' <param name="unixTS">The unix timestamp to be converted (double)</param>
    ''' <param name="sFormat">The format of the returned date/time. Default is "dd-MM-yyyy hh:mm:ss" (string)</param>
    ''' <returns>The date/time corresponding to the given unix timestamp formatted with the given format string</returns>
    ''' <remarks></remarks>
    Public Function UnixTimeStamp_To_Date(ByVal unixTS As Double, Optional ByVal sFormat As String = "dd-MM-yyyy hh:mm:ss") As String
        'First make a System.DateTime equivalent to the UNIX Epoch.
        Dim dateTime As System.DateTime = New System.DateTime(1970, 1, 1, 0, 0, 0, 0)

        'Add the number of seconds in UNIX timestamp to be converted.
        dateTime = dateTime.AddSeconds(unixTS)

        'The dateTime now contains the right date/time so to format the string, 
        ' use the standard formatting methods of the DateTime object.
        Return Format(dateTime, sFormat)
    End Function

    ''' <summary>
    ''' This function converts unix timestamp value into a date/time
    ''' </summary>
    ''' <param name="unixTS">The unix timestamp to be converted (double)</param>
    ''' <returns>The date/time corresponding to the given unix timestamp</returns>
    ''' <remarks></remarks>
    Public Function UnixTimeStamp_To_DateTime(ByVal unixTS As Double) As DateTime
        'First make a System.DateTime equivalent to the UNIX Epoch.
        Dim dateTime As System.DateTime = New System.DateTime(1970, 1, 1, 0, 0, 0, 0)

        'Add the number of seconds in UNIX timestamp to be converted.
        dateTime = dateTime.AddSeconds(unixTS)

        'The dateTime now contains the right date/time so to format the string, 
        ' use the standard formatting methods of the DateTime object.
        Return dateTime
    End Function
End Class
