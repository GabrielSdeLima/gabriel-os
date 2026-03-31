using System.Windows;
using System.Windows.Media.Animation;

namespace GabrielOS.Presentation;

public partial class SplashWindow : Window
{
    public SplashWindow()
    {
        InitializeComponent();
    }

    public void PlayFadeOutAndClose()
    {
        var anim = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(350)))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
        };
        anim.Completed += (_, _) => Close();
        BeginAnimation(OpacityProperty, anim);
    }
}
