using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace GabrielOS.Presentation.Controls;

public class NavigationContentControl : ContentControl
{
    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);
        if (newContent == null) return;

        Opacity = 0;
        var fade = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(300)))
        {
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
        };
        BeginAnimation(OpacityProperty, fade);
    }
}
