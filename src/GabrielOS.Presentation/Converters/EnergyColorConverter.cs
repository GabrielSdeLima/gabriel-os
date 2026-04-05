using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace GabrielOS.Presentation.Converters;

/// <summary>Returns a semi-transparent tinted brush for calendar day cells based on energy level (1-10).
/// Reads theme colors at conversion time so it adapts to the active theme.</summary>
public class EnergyColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int energy)
        {
            if (energy >= 8) return WithOpacity("SemanticSuccessBrush", 90);
            if (energy >= 6) return WithOpacity("SemanticInfoBrush", 70);
            if (energy >= 4) return WithOpacity("SemanticWarningBrush", 70);
            return WithOpacity("SemanticErrorBrush", 80);
        }
        return Brushes.Transparent;
    }

    private static SolidColorBrush WithOpacity(string resourceKey, byte alpha)
    {
        if (System.Windows.Application.Current?.TryFindResource(resourceKey) is SolidColorBrush brush)
        {
            var c = brush.Color;
            return new SolidColorBrush(Color.FromArgb(alpha, c.R, c.G, c.B));
        }
        return new SolidColorBrush(Colors.Transparent);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
