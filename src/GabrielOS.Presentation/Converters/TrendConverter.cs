using System.Globalization;
using System.Windows.Data;
using GabrielOS.Domain.Enums;

namespace GabrielOS.Presentation.Converters;

public class TrendConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Trend trend)
        {
            return trend switch
            {
                Trend.Improving => "↑",
                Trend.Stable => "→",
                Trend.Declining => "↓",
                _ => "—"
            };
        }
        return "—";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
