Option Explicit On

Public Class GeneralRoutines
    Public Enum Visibility
        Visible = 0
        Hidden = 1
        Collapsed = 2
    End Enum

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
