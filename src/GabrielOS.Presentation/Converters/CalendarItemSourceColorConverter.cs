using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GabrielOS.Application.Services;
using GabrielOS.Presentation.ViewModels;
using WpfApplication = System.Windows.Application;

namespace GabrielOS.Presentation.Converters;

/// <summary>
/// Converts a CalendarItem to a SolidColorBrush based on its source and event type string.
/// </summary>
public class CalendarItemSourceColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CalendarItem item)
        {
            return item.Source switch
            {
                CalendarItemSource.UserEvent => GetBrush(CalendarViewModel.GetColorKeyForType(item.EventType)),
                CalendarItemSource.GoalDeadline => GetBrush("AccentWarmBrush"),
                CalendarItemSource.GoalReview => GetBrush("AccentSecondaryBrush"),
                CalendarItemSource.TaskDue => GetBrush("SemanticWarningBrush"),
                CalendarItemSource.CheckIn => GetBrush("SemanticInfoBrush"),
                _ => GetBrush("TextSecondaryBrush"),
            };
        }
        return GetBrush("TextSecondaryBrush");
    }

    private static Brush GetBrush(string resourceKey)
    {
        if (WpfApplication.Current.Resources[resourceKey] is SolidColorBrush brush)
            return brush;
        return Brushes.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Converts a legend color key string to a SolidColorBrush (for dynamic legend).
/// </summary>
public class LegendColorKeyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string colorKey && WpfApplication.Current.Resources[colorKey] is SolidColorBrush brush)
            return brush;
        return Brushes.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Converts CalendarItemSource to a localized source label.
/// </summary>
public class CalendarItemSourceLabelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CalendarItemSource source)
        {
            return source switch
            {
                CalendarItemSource.UserEvent => "Evento",
                CalendarItemSource.GoalDeadline => "Meta",
                CalendarItemSource.GoalReview => "Revisão",
                CalendarItemSource.TaskDue => "Tarefa",
                CalendarItemSource.CheckIn => "Check-in",
                _ => string.Empty,
            };
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
