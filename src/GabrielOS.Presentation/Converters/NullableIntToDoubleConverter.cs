using System.Globalization;
using System.Windows.Data;

namespace GabrielOS.Presentation.Converters;

/// <summary>
/// Converts int? to double for Slider binding. Null maps to 5.0 (neutral default).
/// </summary>
public class NullableIntToDoubleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int i ? (double)i : 5.0;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is double d ? (int?)((int)Math.Round(d)) : (int?)null;
}
