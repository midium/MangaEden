Option Explicit On

Public Class SearchPanel

    Public Event Search(sender As Object, e As SearchParams)

    Private Sub btFind_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btFind.Click
        Dim params As SearchParams = New SearchParams

        params.Title = txtTitle.Text

        RaiseEvent Search(sender, params)

    End Sub
End Class
