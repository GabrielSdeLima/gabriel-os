using System.Windows.Controls;
using GabrielOS.Presentation.ViewModels;

namespace GabrielOS.Presentation.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => SyncPasswordBox();
    }

    private void SyncPasswordBox()
    {
        if (DataContext is SettingsViewModel vm)
            ApiKeyBox.Password = vm.ApiKey;
    }

    private void ApiKeyBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is SettingsViewModel vm)
            vm.ApiKey = ApiKeyBox.Password;
    }
}
