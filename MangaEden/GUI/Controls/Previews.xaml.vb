Option Explicit On

Imports System.IO
Imports System.Net
Imports System.ComponentModel
Imports System.Reflection
Imports MangaEdenAPI
Imports System.Windows.Media.Effects

Public Class Previews
    Private _imagesList As String() = Nothing
    Private _iCount As Integer = -1
    Private _images As Image() = Nothing
    Private _meAPI As API = Nothing

    Public Event PageSelected(ByVal imgSource As BitmapImage)

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _meAPI = New API

    End Sub

    Public Sub LoadImages(ByVal ImagesList As ChapterImages)
        Try
            _iCount = -1

            For i As Integer = 0 To ImagesList.images.Count - 1
                _iCount += 1

                Dim argument As New ItemData
                argument.ID = _iCount
                argument.Image = ImagesList.images(i)(1)

                Dim worker As New BackgroundWorker()
                AddHandler worker.DoWork, AddressOf worker_DoWorkOnline
                AddHandler worker.RunWorkerCompleted, AddressOf worker_RunWorkerCompleted
                worker.RunWorkerAsync(argument)

                argument = Nothing
            Next

        Catch ex As Exception
            'World gone away
        End Try
    End Sub

    Public Sub LoadImages(ByVal ImagesList As String())
        Try
            _iCount = -1

            For Each img As String In ImagesList
                _iCount += 1

                Dim argument As New ItemData
                argument.ID = _iCount
                argument.Image = img

                Dim worker As New BackgroundWorker()
                AddHandler worker.DoWork, AddressOf worker_DoWorkLocal
                AddHandler worker.RunWorkerCompleted, AddressOf worker_RunWorkerCompleted
                worker.RunWorkerAsync(argument)
            Next

        Catch ex As Exception
            'World gone away
        End Try
    End Sub

    Private Sub worker_DoWorkOnline(sender As Object, e As DoWorkEventArgs)
        Dim imageInfo As ItemData = DirectCast(e.Argument, ItemData)

        Try
            'TODO: Implement settings load for download path
            Dim localPath(1) As String
            localPath(0) = _meAPI.getImage(imageInfo.Image)
            localPath(1) = imageInfo.ID
            'We are done working send this to the WorkCompleted event
            e.Result = localPath

        Catch ex As Exception
            e.Result = Nothing
        End Try

    End Sub

    Private Sub worker_DoWorkLocal(sender As Object, e As DoWorkEventArgs)
        Dim imageInfo As ItemData = DirectCast(e.Argument, ItemData)

        Try
            'TODO: Implement settings load for download path
            Dim localPath(1) As String
            localPath(0) = imageInfo.Image
            localPath(1) = imageInfo.ID
            'We are done working send this to the WorkCompleted event
            e.Result = localPath

        Catch ex As Exception
            e.Result = Nothing
        End Try

    End Sub

    Private Sub worker_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs)
        Dim img As BitmapImage = Nothing

        If e.Result Is Nothing Then
            img = LoadBitmapFromResource("Images/ImageError.png")

        Else
            If e.Result(0).ToString() = "" Then
                img = LoadBitmapFromResource("Images/ImageError.png")

            Else
                img = New BitmapImage(New Uri(e.Result(0).ToString()))

            End If

        End If

        'Adding image inside the preview component
        Dim i As New Image()
        i.Source = img
        i.HorizontalAlignment = Windows.HorizontalAlignment.Left
        i.Width = 95
        i.Tag = New ItemData(e.Result(1), e.Result(0))
        i.Cursor = Cursors.Hand
        AddHandler i.MouseDown, AddressOf HandleImageClick

        i.Margin = New Thickness((e.Result(1) * 100) + 5, 0, 0, 0)
        gridContainer.Children.Add(i)

    End Sub

    ''' <summary>
    ''' Load a resource WPF-BitmapImage (png, bmp, ...) from embedded resource defined as 'Resource' not as 'Embedded resource'.
    ''' </summary>
    ''' <param name="pathInApplication">Path without starting slash</param>
    ''' <param name="assembly">Usually 'Assembly.GetExecutingAssembly()'. If not mentionned, I will use the calling assembly</param>
    ''' <returns></returns>
    Public Function LoadBitmapFromResource(ByVal pathInApplication As String, Optional ByVal assembly As Assembly = Nothing) As BitmapImage
        If (assembly Is Nothing) Then
            assembly = assembly.GetCallingAssembly()
        End If

        If (pathInApplication(0) = "/") Then
            pathInApplication = pathInApplication.Substring(1)
        End If
        Return New BitmapImage(New Uri("pack://application:,,,/" + assembly.GetName().Name + ";component/" + pathInApplication, UriKind.Absolute))
    End Function

    Public Function HandleImageClick(sender As Object, e As System.Windows.Input.MouseButtonEventArgs)
        RaiseEvent PageSelected(sender.Source)

        Return Me
    End Function

End Class

Class ItemData
    Public Property ID As Integer
    Public Property Image As String

    Public Sub New(Optional iID As Integer = 0, Optional sImage As String = "")
        ID = iID
        Image = sImage
    End Sub
End Class
