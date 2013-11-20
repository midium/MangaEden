Option Explicit On

''' <summary>
''' Support class that will contains the list of the images paths that will be used by the chapter viewer or downloader
''' </summary>
''' <remarks></remarks>
Public Class ChapterImages
    'The paths container
    Public Property images() As List(Of List(Of Object))

    ''' <summary>
    ''' Images list sorter
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Sort()
        Dim tmp As Object = Nothing

        For i As Integer = 0 To images.Count - 2
            For j As Integer = i + 1 To images.Count - 1
                If CInt(images(i)(0)) > CInt(images(j)(0)) Then
                    tmp = images(j)
                    images(j) = images(i)
                    images(i) = tmp
                End If
            Next
        Next
    End Sub
End Class
