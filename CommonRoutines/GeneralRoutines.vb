Option Explicit On

''' <summary>
''' This class contains general accounting routines
''' 
''' Developed by: Matteo Loro
''' Email: matteo.loro@gmail.com
''' </summary>
''' <remarks></remarks>
Public Class GeneralRoutines
    Public Enum Visibility
        Visible = 0
        Hidden = 1
        Collapsed = 2
    End Enum

    ''' <summary>
    ''' This function convert a boolean value into a Visibility compatible value.
    ''' This can be used to show/hide components on the UI according to the given boolean value.
    ''' </summary>
    ''' <param name="BoolVal">Boolean value telling if to show (true) or hide (false) the component</param>
    ''' <returns>The corresponding Visibility value</returns>
    ''' <remarks></remarks>
    Public Function BoolToVisibility(ByVal BoolVal As Boolean) As Visibility
        Dim result As Visibility = Visibility.Visible

        If BoolVal Then
            result = Visibility.Visible
        Else
            result = Visibility.Collapsed
        End If

        Return result
    End Function

End Class
