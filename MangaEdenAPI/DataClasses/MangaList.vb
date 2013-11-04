Option Explicit On

Public Class MangaList
    Public Enum Sort
        Ascending = 0
        Descending = 1
    End Enum

    Public Property start As Integer
    Public Property total As Integer
    Public Property [end] As Integer
    Public Property page As Integer

    Public Property manga As MangaBasicInfo()

    Public Sub SortMangasByTitle(ByVal SortMethod As Sort)
        Dim tmpManga As MangaBasicInfo = Nothing

        Try
            For i As Integer = 0 To manga.GetUpperBound(0) - 1
                For j = i + 1 To manga.GetUpperBound(0)

                    If (SortMethod = Sort.Ascending) Then
                        If manga(i).Title > manga(j).Title Then
                            tmpManga = manga(i)
                            manga(i) = manga(j)
                            manga(j) = tmpManga
                        End If

                    Else
                        If manga(i).Title < manga(j).Title Then
                            tmpManga = manga(i)
                            manga(i) = manga(j)
                            manga(j) = tmpManga
                        End If

                    End If

                Next
            Next

        Catch ex As Exception
            Console.Write(ex.Message)

        End Try

        tmpManga = Nothing
    End Sub

    Public Sub SortMangasByLastUpdate(ByVal SortMethod As Sort)
        Dim tmpManga As MangaBasicInfo = Nothing

        Try
            For i As Integer = 0 To manga.GetUpperBound(0) - 1
                For j = i + 1 To manga.GetUpperBound(0)

                    If (SortMethod = Sort.Ascending) Then
                        If manga(i).ld > manga(j).ld Then
                            tmpManga = manga(i)
                            manga(i) = manga(j)
                            manga(j) = tmpManga
                        End If

                    Else
                        If manga(i).ld < manga(j).ld Then
                            tmpManga = manga(i)
                            manga(i) = manga(j)
                            manga(j) = tmpManga
                        End If

                    End If

                Next
            Next

        Catch ex As Exception
            Console.Write(ex.Message)

        End Try

        tmpManga = Nothing
    End Sub

End Class
