Public Class About

    Private Sub About_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        lblVersion.Content = "v.: " & My.Application.Info.Version.ToString
    End Sub

    Private Sub btOK_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btOK.Click
        Me.Close()
    End Sub
End Class
