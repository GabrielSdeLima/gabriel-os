using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Presentation.Navigation;

namespace GabrielOS.Presentation.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _navigation;

    [ObservableProperty]
    private ObservableObject? _currentViewModel;

    [ObservableProperty]
    private string _currentSection = "Dashboard";

    public MainViewModel(INavigationService navigation)
    {
        _navigation = navigation;
        _navigation.CurrentViewModelChanged += () => CurrentViewModel = _navigation.CurrentViewModel;

        // Start on Dashboard
        _navigation.NavigateTo<DashboardViewModel>();
    }

    [RelayCommand]
    private void NavigateToDashboard()
    {
        CurrentSection = "Dashboard";
        _navigation.NavigateTo<DashboardViewModel>();
    }

    [RelayCommand]
    private void NavigateToPillars()
    {
        CurrentSection = "Pillars";
        _navigation.NavigateTo<PillarListViewModel>();
    }

    [RelayCommand]
    private void NavigateToCheckIn()
    {
        CurrentSection = "Check-in";
        _navigation.NavigateTo<CheckInViewModel>();
    }

    [RelayCommand]
    private void NavigateToJournal()
    {
        CurrentSection = "Journal";
        _navigation.NavigateTo<JournalListViewModel>();
    }
}
