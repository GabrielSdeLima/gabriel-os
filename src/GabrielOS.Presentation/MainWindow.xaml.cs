using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using GabrielOS.Presentation.ViewModels;

namespace GabrielOS.Presentation;

public partial class MainWindow : Window
{
    private static readonly Duration SidebarDuration = new(TimeSpan.FromMilliseconds(250));
    private static readonly IEasingFunction SidebarEase = new CubicEase { EasingMode = EasingMode.EaseInOut };

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        StateChanged += (_, _) => MaximizeBtn.Content = WindowState == WindowState.Maximized ? "❐" : "□";
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.IsSidebarCollapsed))
            AnimateSidebar(((MainViewModel)DataContext).IsSidebarCollapsed);
    }

    private void AnimateSidebar(bool collapse)
    {
        double target = collapse ? 56 : 220;
        var anim = new DoubleAnimation(target, SidebarDuration) { EasingFunction = SidebarEase };
        SidebarBorder.BeginAnimation(WidthProperty, anim);
        TitleBrandingBorder.BeginAnimation(WidthProperty, anim.Clone());
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
