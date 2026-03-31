using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GabrielOS.Presentation.Converters;

/// <summary>Returns a semi-transparent tinted brush for calendar day cells based on energy level (1-10).</summary>
public class EnergyColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int energy)
        {
            // Green tint for high energy, yellow for mid, red for low
            if (energy >= 8) return new SolidColorBrush(Color.FromArgb(90, 163, 190, 140));  // AccentColor
            if (energy >= 6) return new SolidColorBrush(Color.FromArgb(70, 124, 158, 189));  // PrimaryColor
            if (energy >= 4) return new SolidColorBrush(Color.FromArgb(70, 235, 203, 139));  // WarningColor
            return new SolidColorBrush(Color.FromArgb(80, 191, 97, 106));                    // DangerColor
        }
        return Brushes.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
