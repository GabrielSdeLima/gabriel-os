using System.Windows;
using System.Windows.Media.Animation;
using GabrielOS.Presentation.ViewModels;

namespace GabrielOS.Presentation;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        StateChanged += (_, _) => MaximizeBtn.Content = WindowState == WindowState.Maximized ? "❐" : "□";
    }

    private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        => WindowState = WindowState.Minimized;

    private void MaximizeBtn_Click(object sender, RoutedEventArgs e)
        => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void CloseBtn_Click(object sender, RoutedEventArgs e)
        => Close();

    public void PlayFadeIn()
    {
        var anim = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(450)))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        BeginAnimation(OpacityProperty, anim);
    }
}
