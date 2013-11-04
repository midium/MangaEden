Public Interface IApi
    Enum Languages
        English = 0
        Italian = 1
    End Enum

    Function getMangaList(ByVal eLanguage As Languages) As MangaList
    Function getMangaDetails(ByVal mangaID As String) As MangaDetails
    Function Login(ByVal UserName As String, ByVal Password As String) As String
    Function Logout() As Boolean
    Function MyMangas() As MyManga
End Interface
