Option Explicit On

Public Class DateTimeRoutines
    Public Function UnixTimeStamp_To_Date(ByVal unixTS As Double, Optional ByVal sFormat As String = "dd-MM-yyyy hh:mm:ss") As String
        'First make a System.DateTime equivalent to the UNIX Epoch.
        Dim dateTime As System.DateTime = New System.DateTime(1970, 1, 1, 0, 0, 0, 0)

        'Add the number of seconds in UNIX timestamp to be converted.
        dateTime = dateTime.AddSeconds(unixTS)

        'The dateTime now contains the right date/time so to format the string, 
        ' use the standard formatting methods of the DateTime object.
        Return Format(dateTime, sFormat)
    End Function

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
