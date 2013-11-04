Option Explicit On

Public Class SplashScreen

    Private WithEvents frmMain As MainWindow

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        lblVersion.Content = "v.: " & My.Application.Info.Version.ToString
        Me.Topmost = True
    End Sub

    Private Sub SplashScreen_ContentRendered(sender As Object, e As System.EventArgs) Handles Me.ContentRendered
        frmMain = New MainWindow
        frmMain.Show()

    End Sub

    Private Sub frmMain_LoadingCompleted() Handles frmMain.LoadingCompleted
        Me.Close()
    End Sub

    Private Sub frmMain_StatusUpdate(Status As String) Handles frmMain.StatusUpdate
        lblStatus.Content = "Status: " & Status
        lblStatus.InvalidateVisual()
    End Sub
End Class
