using System.Windows;
using GabrielOS.Presentation.ViewModels;

namespace GabrielOS.Presentation;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
