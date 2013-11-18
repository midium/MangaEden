Option Explicit On

Namespace Converters
    <ValueConversionAttribute(GetType(Boolean), GetType(Visibility))> _
    Public Class BoolToVisibilityConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
            If Not TypeOf (value) Is Boolean Then
                Return Nothing
            End If

            Return IIf(value, Visibility.Visible, Visibility.Hidden)
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
            If Equals(value, Visibility.Visible) Then
                Return True
            End If
            If Equals(value, Visibility.Hidden) Then
                Return False
            End If
            Return Nothing
        End Function
    End Class

End Namespace
