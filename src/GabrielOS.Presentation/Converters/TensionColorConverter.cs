using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace GabrielOS.Presentation.Converters;

/// <summary>Returns a color brush for tension level display.
/// High tension (>=7) = error/red, Low tension (<=3) = cool/green, else secondary text.</summary>
public class TensionColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int tension)
        {
            if (tension >= 7) return FindBrush("SemanticErrorBrush");
            if (tension <= 3) return FindBrush("AccentCoolBrush");
        }
        return FindBrush("TextSecondaryBrush");
    }

    private static SolidColorBrush FindBrush(string key)
    {
        if (System.Windows.Application.Current?.TryFindResource(key) is SolidColorBrush brush)
            return brush;
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
