using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Rules;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Enums;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Presentation.ViewModels;

public class CalendarDay
{
    public DateTime Date { get; init; }
    public bool IsCurrentMonth { get; init; }
    public bool IsToday { get; init; }
    public bool IsInCycle { get; init; }
    public int? Energy { get; init; }
    public bool HasCheckIn => Energy.HasValue;
}

public partial class DashboardViewModel : ObservableObject
{
    private readonly PillarService _pillarService;
    private readonly CheckInService _checkInService;
    private readonly JournalService _journalService;
    private readonly GoalService _goalService;
    private readonly AlertService _alertService;
    private readonly CycleFocusService _cycleFocusService;
    private readonly IUserRepository _userRepo;

    [ObservableProperty] private ObservableCollection<Pillar> _pillars = new();
    [ObservableProperty] private ObservableCollection<Goal> _topGoals = new();
    [ObservableProperty] private ObservableCollection<Alert> _alerts = new();
    [ObservableProperty] private CycleFocus? _activeCycleFocus;
    [ObservableProperty] private CheckIn? _todayCheckIn;
    [ObservableProperty] private string _suggestedModeText = "Nenhum check-in ainda. Comece seu dia!";
    [ObservableProperty] private int _weeklyJournalCount;
    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private ObservableCollection<CalendarDay> _calendarDays = new();
    [ObservableProperty] private string _calendarTitle = string.Empty;
    [ObservableProperty] private bool _hasCheckInToday;

    public DashboardViewModel(
        PillarService pillarService,
        CheckInService checkInService,
        JournalService journalService,
        GoalService goalService,
        AlertService alertService,
        CycleFocusService cycleFocusService,
        IUserRepository userRepo)
    {
        _pillarService = pillarService;
        _checkInService = checkInService;
        _journalService = journalService;
        _goalService = goalService;
        _alertService = alertService;
        _cycleFocusService = cycleFocusService;
        _userRepo = userRepo;

        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var user = await _userRepo.GetDefaultUserAsync();
            if (user == null) return;

            var pillars = await _pillarService.GetAllOrderedAsync(user.Id);
            Pillars = new ObservableCollection<Pillar>(pillars);

            TodayCheckIn = await _checkInService.GetTodayAsync(user.Id);
            HasCheckInToday = TodayCheckIn != null;
            SuggestedModeText = TodayCheckIn?.SuggestedMode != null
                ? $"{TodayCheckIn.SuggestedMode} — {ModeCalculator.GetModeDescription(TodayCheckIn.SuggestedMode.Value)}"
                : "Nenhum check-in ainda. Comece seu dia!";

            WeeklyJournalCount = await _journalService.GetWeeklyCountAsync(user.Id);

            var goals = await _goalService.GetByUserAsync(user.Id);
            TopGoals = new ObservableCollection<Goal>(
                goals.Where(g => g.Status == GoalStatus.Active && g.Priority == GoalPriority.P1)
                     .Take(3));

            ActiveCycleFocus = await _cycleFocusService.GetActiveAsync(user.Id);

            var alerts = await _alertService.GetAlertsAsync(user.Id);
            Alerts = new ObservableCollection<Alert>(alerts.Take(5));

            var recentCheckIns = await _checkInService.GetRecentAsync(user.Id, 60);
            BuildCalendar(recentCheckIns);
        }
        finally { IsLoading = false; }
    }

    private void BuildCalendar(IReadOnlyList<CheckIn> checkIns)
    {
        var today = DateTime.Now.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        CalendarTitle = monthStart.ToString("MMMM yyyy");

        var byDate = checkIns.ToDictionary(c => c.Date.Date);

        // Pad to the Sunday before the 1st and Saturday after the last day
        var calStart = monthStart.AddDays(-(int)monthStart.DayOfWeek);
        var calEnd = monthEnd.AddDays(6 - (int)monthEnd.DayOfWeek);

        var days = new List<CalendarDay>();
        for (var d = calStart; d <= calEnd; d = d.AddDays(1))
        {
            byDate.TryGetValue(d, out var ci);
            var isInCycle = ActiveCycleFocus != null
                && d >= ActiveCycleFocus.StartDate.Date
                && d <= ActiveCycleFocus.EndDate.Date;

            days.Add(new CalendarDay
            {
                Date = d,
                IsCurrentMonth = d.Month == today.Month,
                IsToday = d == today,
                IsInCycle = isInCycle,
                Energy = ci?.Energy
            });
        }

        CalendarDays = new ObservableCollection<CalendarDay>(days);
    }
}
