Option Explicit On

Public Class SearchPanel

    Public Event Search(sender As Object, e As SearchParams)

    Private Sub btFind_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btFind.Click
        Dim params As SearchParams = New SearchParams

        'Recording user search parameters
        params.Title = txtTitle.Text

        'Performin search
        RaiseEvent Search(sender, params)

    End Sub
End Class
