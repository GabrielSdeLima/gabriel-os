using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GabrielOS.Presentation.Converters;

public class TensionColorConverter : IValueConverter
{
    private static readonly SolidColorBrush High   = new(Color.FromRgb(0xBF, 0x61, 0x6A)); // DangerColor
    private static readonly SolidColorBrush Low    = new(Color.FromRgb(0xA3, 0xBE, 0x8C)); // AccentColor
    private static readonly SolidColorBrush Medium = new(Color.FromRgb(0x80, 0x8A, 0x9F)); // TextSecondaryColor

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int tension)
        {
            if (tension >= 7) return High;
            if (tension <= 3) return Low;
        }
        return Medium;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
