using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Presentation.Navigation;

namespace GabrielOS.Presentation.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _navigation;

    [ObservableProperty] private ObservableObject? _currentViewModel;
    [ObservableProperty] private string _currentSection = "Dashboard";

    public string TodayFormatted { get; } = DateTime.Today.ToString("dddd, d MMM").ToLower();

    public MainViewModel(INavigationService navigation)
    {
        _navigation = navigation;
        _navigation.CurrentViewModelChanged += () => CurrentViewModel = _navigation.CurrentViewModel;
        _navigation.NavigateTo<DashboardViewModel>();
    }

    [RelayCommand] private void NavigateToDashboard() { CurrentSection = "Dashboard"; _navigation.NavigateTo<DashboardViewModel>(); }
    [RelayCommand] private void NavigateToPillars() { CurrentSection = "Pillars"; _navigation.NavigateTo<PillarListViewModel>(); }
    [RelayCommand] private void NavigateToCheckIn() { CurrentSection = "Check-in"; _navigation.NavigateTo<CheckInViewModel>(); }
    [RelayCommand] private void NavigateToJournal() { CurrentSection = "Journal"; _navigation.NavigateTo<JournalListViewModel>(); }
    [RelayCommand] private void NavigateToGoals() { CurrentSection = "Goals"; _navigation.NavigateTo<GoalListViewModel>(); }
    [RelayCommand] private void NavigateToDecisions() { CurrentSection = "Decisions"; _navigation.NavigateTo<DecisionListViewModel>(); }
    [RelayCommand] private void NavigateToWeeklyReview() { CurrentSection = "Weekly Review"; _navigation.NavigateTo<WeeklyReviewViewModel>(); }
    [RelayCommand] private void NavigateToMonthlyReview() { CurrentSection = "Monthly Review"; _navigation.NavigateTo<MonthlyReviewViewModel>(); }
    [RelayCommand] private void NavigateToCycleFocus() { CurrentSection = "Cycle Focus"; _navigation.NavigateTo<CycleFocusViewModel>(); }
    [RelayCommand] private void NavigateToTasks() { CurrentSection = "Tasks"; _navigation.NavigateTo<TaskListViewModel>(); }
    [RelayCommand] private void NavigateToPatterns() { CurrentSection = "Patterns"; _navigation.NavigateTo<PatternListViewModel>(); }
    [RelayCommand] private void NavigateToMetrics() { CurrentSection = "Metrics"; _navigation.NavigateTo<MetricListViewModel>(); }
    [RelayCommand] private void NavigateToPillarTrends() { CurrentSection = "Pillar Trends"; _navigation.NavigateTo<PillarTrendViewModel>(); }
    [RelayCommand] private void NavigateToSearch() { CurrentSection = "Search"; _navigation.NavigateTo<SearchViewModel>(); }
    [RelayCommand] private void NavigateToExport() { CurrentSection = "Export"; _navigation.NavigateTo<ExportViewModel>(); }
    [RelayCommand] private void NavigateToSettings() { CurrentSection = "Settings"; _navigation.NavigateTo<SettingsViewModel>(); }
}
