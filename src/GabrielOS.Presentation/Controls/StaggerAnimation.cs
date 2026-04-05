using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace GabrielOS.Presentation.Controls;

/// <summary>
/// Attached property that staggers children of a Panel with a fade-in animation on load.
/// Usage: controls:StaggerAnimation.Enabled="True" on any StackPanel/WrapPanel/etc.
/// </summary>
public static class StaggerAnimation
{
    public static readonly DependencyProperty EnabledProperty =
        DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(StaggerAnimation),
            new PropertyMetadata(false, OnEnabledChanged));

    public static bool GetEnabled(DependencyObject obj) => (bool)obj.GetValue(EnabledProperty);
    public static void SetEnabled(DependencyObject obj, bool value) => obj.SetValue(EnabledProperty, value);

    private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Panel panel && (bool)e.NewValue)
            panel.Loaded += OnPanelLoaded;
    }

    private static void OnPanelLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not Panel panel) return;
        panel.Loaded -= OnPanelLoaded;

        var ease = new CubicEase { EasingMode = EasingMode.EaseOut };
        for (int i = 0; i < panel.Children.Count; i++)
        {
            if (panel.Children[i] is not UIElement child) continue;
            child.Opacity = 0;
            var fade = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(280)))
            {
                BeginTime = TimeSpan.FromMilliseconds(i * 45),
                EasingFunction = ease
            };
            child.BeginAnimation(UIElement.OpacityProperty, fade);
        }
    }
}
