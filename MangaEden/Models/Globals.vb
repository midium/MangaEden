Option Explicit On

Imports SettingsManager

Module Globals
    Public _settingsManager As AppManager
    Public _availableMangas As MangaEdenAPI.MangaList
    Public _myMangas As MangaEdenAPI.MyManga

    Public Function BoolToVisibility(ByVal BoolVal As Boolean) As Visibility
        Dim result As Visibility = Visibility.Visible

        If BoolVal Then
            result = Visibility.Visible
        Else
            result = Visibility.Collapsed
        End If

        Return result
    End Function

End Module
