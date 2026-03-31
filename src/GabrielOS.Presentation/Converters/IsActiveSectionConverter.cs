using System.Globalization;
using System.Windows.Data;

namespace GabrielOS.Presentation.Converters;

public class IsActiveSectionConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
        => values.Length == 2
           && values[0] is string current
           && values[1] is string tag
           && current == tag;

    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
