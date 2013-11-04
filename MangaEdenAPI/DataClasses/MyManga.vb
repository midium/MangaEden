Option Explicit On

Public Class MyManga
    Public Enum Sort
        Ascending = 0
        Descending = 1
    End Enum

    Public Property myManga As MyMangaInfo()

    Public Sub SortMyMangasByTitle(ByVal SortMethod As Sort)
        Dim tmpManga As MyMangaInfo = Nothing

        Try
            For i As Integer = 0 To myManga.GetUpperBound(0) - 1
                For j = i + 1 To myManga.GetUpperBound(0)

                    If (SortMethod = Sort.Ascending) Then
                        If myManga(i).Title > myManga(j).Title Then
                            tmpManga = myManga(i)
                            myManga(i) = myManga(j)
                            myManga(j) = tmpManga
                        End If

                    Else
                        If myManga(i).Title < myManga(j).Title Then
                            tmpManga = myManga(i)
                            myManga(i) = myManga(j)
                            myManga(j) = tmpManga
                        End If

                    End If

                Next
            Next

        Catch ex As Exception
            Console.Write(ex.Message)

        End Try

        tmpManga = Nothing
    End Sub

    Public Sub SortmymangasByLastUpdate(ByVal SortMethod As Sort)
        Dim tmpManga As MyMangaInfo = Nothing

        Try
            For i As Integer = 0 To myManga.GetUpperBound(0) - 1
                For j = i + 1 To myManga.GetUpperBound(0)

                    If (SortMethod = Sort.Ascending) Then
                        If myManga(i).latest_chapter.date > myManga(j).latest_chapter.date Then
                            tmpManga = myManga(i)
                            myManga(i) = myManga(j)
                            myManga(j) = tmpManga
                        End If

                    Else
                        If myManga(i).latest_chapter.date < myManga(j).latest_chapter.date Then
                            tmpManga = myManga(i)
                            myManga(i) = myManga(j)
                            myManga(j) = tmpManga
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
