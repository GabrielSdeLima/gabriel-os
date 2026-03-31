using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GabrielOS.Presentation.Converters;

/// <summary>Visible when null / false / 0 / empty string — the opposite of NullToVisibilityConverter.</summary>
public class InverseToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b) return b ? Visibility.Collapsed : Visibility.Visible;
        if (value is int i) return i > 0 ? Visibility.Collapsed : Visibility.Visible;
        if (value is string s) return !string.IsNullOrWhiteSpace(s) ? Visibility.Collapsed : Visibility.Visible;
        return value == null ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
