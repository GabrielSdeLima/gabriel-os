using System.Windows;
using System.Windows.Media.Animation;
using GabrielOS.Application.Services;

namespace GabrielOS.Presentation.Services;

public enum ThemeMode { Light, Dark }

public class ThemeService
{
    private const string ThemeKey = "theme";
    private readonly SettingsService _settings;

    public ThemeMode CurrentTheme { get; private set; } = ThemeMode.Dark;

    public ThemeService(SettingsService settings)
    {
        _settings = settings;
    }

    public void SetTheme(ThemeMode mode)
    {
        var app = System.Windows.Application.Current;
        var window = app.MainWindow;

        // Brief fade-out
        if (window != null)
        {
            var fadeOut = new DoubleAnimation(1, 0.82, new Duration(TimeSpan.FromMilliseconds(110)));
            window.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        var toRemove = app.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source?.OriginalString.Contains("Theme.xaml") == true);
        if (toRemove != null)
            app.Resources.MergedDictionaries.Remove(toRemove);

        var uri = mode switch
        {
            ThemeMode.Light => new Uri("Themes/LightTheme.xaml", UriKind.Relative),
            ThemeMode.Dark => new Uri("Themes/DarkTheme.xaml", UriKind.Relative),
            _ => throw new ArgumentOutOfRangeException(nameof(mode))
        };
        app.Resources.MergedDictionaries.Insert(0, new ResourceDictionary { Source = uri });
        CurrentTheme = mode;
        _settings.SetTheme(mode.ToString());

        // Fade back in
        if (window != null)
        {
            var fadeIn = new DoubleAnimation(0.82, 1, new Duration(TimeSpan.FromMilliseconds(200)));
            window.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        }
    }

    public void Toggle() => SetTheme(CurrentTheme == ThemeMode.Light ? ThemeMode.Dark : ThemeMode.Light);

    public void LoadSavedTheme()
    {
        var saved = _settings.GetTheme();
        if (Enum.TryParse<ThemeMode>(saved, out var mode))
            SetTheme(mode);
        else
            SetTheme(ThemeMode.Dark);
    }
}
