using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Presentation.Navigation;
using GabrielOS.Presentation.Services;

namespace GabrielOS.Presentation.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _navigation;
    private readonly ThemeService _themeService;

    [ObservableProperty] private ObservableObject? _currentViewModel;
    [ObservableProperty] private string _currentSection = "Dashboard";
    [ObservableProperty] private bool _isDarkTheme;
    [ObservableProperty] private bool _isSidebarCollapsed;

    public string TodayFormatted { get; } = DateTime.Today.ToString("dddd, d MMM").ToLower();

    public bool CanGoBack => _navigation.CanGoBack;

    public MainViewModel(INavigationService navigation, ThemeService themeService)
    {
        _navigation = navigation;
        _themeService = themeService;
        _navigation.CurrentViewModelChanged += () =>
        {
            CurrentViewModel = _navigation.CurrentViewModel;
            OnPropertyChanged(nameof(CanGoBack));
        };
        IsDarkTheme = _themeService.CurrentTheme == ThemeMode.Dark;
        _navigation.NavigateTo<DashboardViewModel>();
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        _themeService.Toggle();
        IsDarkTheme = _themeService.CurrentTheme == ThemeMode.Dark;
    }

    [RelayCommand]
    private void ToggleSidebar() => IsSidebarCollapsed = !IsSidebarCollapsed;

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private void GoBack() => _navigation.GoBack();

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
    [RelayCommand] private void NavigateToCalendar() { CurrentSection = "Calendar"; _navigation.NavigateTo<CalendarViewModel>(); }
    [RelayCommand] private void NavigateToSearch() { CurrentSection = "Search"; _navigation.NavigateTo<SearchViewModel>(); }
    [RelayCommand] private void NavigateToExport() { CurrentSection = "Export"; _navigation.NavigateTo<ExportViewModel>(); }
    [RelayCommand] private void NavigateToSettings() { CurrentSection = "Settings"; _navigation.NavigateTo<SettingsViewModel>(); }
}
