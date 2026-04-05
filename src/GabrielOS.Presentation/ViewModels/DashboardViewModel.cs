using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Rules;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Enums;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Presentation.ViewModels;

public class RecentDay
{
    public DateTime Date { get; init; }
    public int? Energy { get; init; }
    public bool IsToday { get; init; }
}

public partial class DashboardViewModel : ObservableObject
{
    private readonly PillarService _pillarService;
    private readonly CheckInService _checkInService;
    private readonly JournalService _journalService;
    private readonly GoalService _goalService;
    private readonly AlertService _alertService;
    private readonly CycleFocusService _cycleFocusService;
    private readonly TaskItemService _taskItemService;
    private readonly IUserRepository _userRepo;

    [ObservableProperty] private ObservableCollection<Pillar> _pillars = new();
    [ObservableProperty] private ObservableCollection<Goal> _topGoals = new();
    [ObservableProperty] private ObservableCollection<Alert> _alerts = new();
    [ObservableProperty] private CycleFocus? _activeCycleFocus;
    [ObservableProperty] private CheckIn? _todayCheckIn;
    [ObservableProperty] private string _suggestedModeText = "Nenhum check-in ainda. Comece seu dia!";
    [ObservableProperty] private int _weeklyJournalCount;
    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private bool _hasCheckInToday;

    // Cycle Command Center
    [ObservableProperty] private ObservableCollection<RecentDay> _recentDays = new();
    [ObservableProperty] private ObservableCollection<Goal> _cycleGoals = new();
    [ObservableProperty] private ObservableCollection<TaskItem> _cycleTasks = new();
    [ObservableProperty] private ObservableCollection<Pillar> _affectedPillars = new();
    [ObservableProperty] private double _cycleProgressPercent;
    [ObservableProperty] private int _cycleDaysRemaining;
    [ObservableProperty] private int _cycleDaysTotal;
    [ObservableProperty] private bool _hasCycleGoals;

    public DashboardViewModel(
        PillarService pillarService,
        CheckInService checkInService,
        JournalService journalService,
        GoalService goalService,
        AlertService alertService,
        CycleFocusService cycleFocusService,
        TaskItemService taskItemService,
        IUserRepository userRepo)
    {
        _pillarService = pillarService;
        _checkInService = checkInService;
        _journalService = journalService;
        _goalService = goalService;
        _alertService = alertService;
        _cycleFocusService = cycleFocusService;
        _taskItemService = taskItemService;
        _userRepo = userRepo;

        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        IReadOnlyList<CheckIn> recentCheckIns = Array.Empty<CheckIn>();
        IReadOnlyList<Goal> allGoals = Array.Empty<Goal>();
        IReadOnlyList<TaskItem> allTasks = Array.Empty<TaskItem>();
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

            allGoals = await _goalService.GetByUserAsync(user.Id);
            TopGoals = new ObservableCollection<Goal>(
                allGoals.Where(g => g.Status == GoalStatus.Active && g.Priority == GoalPriority.P1)
                     .Take(3));

            ActiveCycleFocus = await _cycleFocusService.GetActiveAsync(user.Id);

            var alerts = await _alertService.GetAlertsAsync(user.Id);
            Alerts = new ObservableCollection<Alert>(alerts.Take(5));

            recentCheckIns = await _checkInService.GetRecentAsync(user.Id, 14);
            allTasks = await _taskItemService.GetByUserAsync(user.Id);
        }
        finally
        {
            BuildRecentDays(recentCheckIns);
            BuildCycleCommandCenter(allGoals, allTasks);
            IsLoading = false;
        }
    }

    private void BuildCycleCommandCenter(IReadOnlyList<Goal> allGoals, IReadOnlyList<TaskItem> allTasks)
    {
        if (ActiveCycleFocus == null)
        {
            CycleGoals = new();
            CycleTasks = new();
            AffectedPillars = new();
            CycleProgressPercent = 0;
            CycleDaysRemaining = 0;
            CycleDaysTotal = 0;
            HasCycleGoals = false;
            return;
        }

        // Progress
        var today = DateTime.Now.Date;
        var totalDays = (ActiveCycleFocus.EndDate.Date - ActiveCycleFocus.StartDate.Date).Days;
        var elapsed = (today - ActiveCycleFocus.StartDate.Date).Days;
        CycleDaysTotal = Math.Max(totalDays, 1);
        CycleDaysRemaining = Math.Max(0, totalDays - elapsed);
        CycleProgressPercent = Math.Clamp((double)elapsed / CycleDaysTotal * 100, 0, 100);

        // Goals: prefer linked CycleFocusGoals, fallback to active P1/P2
        var linkedGoalIds = ActiveCycleFocus.CycleFocusGoals
            .Select(cfg => cfg.GoalId)
            .ToHashSet();

        List<Goal> cycleGoals;
        if (linkedGoalIds.Count > 0)
        {
            cycleGoals = allGoals
                .Where(g => linkedGoalIds.Contains(g.Id))
                .ToList();
        }
        else
        {
            cycleGoals = allGoals
                .Where(g => g.Status == GoalStatus.Active
                    && (g.Priority == GoalPriority.P1 || g.Priority == GoalPriority.P2))
                .OrderBy(g => g.Priority)
                .Take(5)
                .ToList();
        }

        CycleGoals = new ObservableCollection<Goal>(cycleGoals);
        HasCycleGoals = cycleGoals.Count > 0;

        // Tasks: Next/Doing, prefer those linked to cycle goals
        var cycleGoalIds = cycleGoals.Select(g => g.Id).ToHashSet();
        var activeTasks = allTasks
            .Where(t => t.Status == TaskItemStatus.Next || t.Status == TaskItemStatus.Doing)
            .OrderByDescending(t => cycleGoalIds.Contains(t.GoalId ?? Guid.Empty))
            .ThenBy(t => t.Status == TaskItemStatus.Doing ? 0 : 1)
            .ThenBy(t => t.Priority)
            .Take(8)
            .ToList();

        CycleTasks = new ObservableCollection<TaskItem>(activeTasks);

        // Affected pillars: unique from cycle goals
        var affectedPillarIds = cycleGoals
            .Select(g => g.PillarId)
            .Distinct()
            .ToHashSet();

        var affected = Pillars
            .Where(p => affectedPillarIds.Contains(p.Id))
            .ToList();

        AffectedPillars = new ObservableCollection<Pillar>(affected);
    }

    private void BuildRecentDays(IReadOnlyList<CheckIn> checkIns)
    {
        var today = DateTime.Now.Date;
        var byDate = checkIns.ToDictionary(c => c.Date.Date);
        var days = new List<RecentDay>();

        for (int i = 13; i >= 0; i--)
        {
            var date = today.AddDays(-i);
            byDate.TryGetValue(date, out var ci);
            days.Add(new RecentDay
            {
                Date = date,
                Energy = ci?.Energy,
                IsToday = date == today
            });
        }

        RecentDays = new ObservableCollection<RecentDay>(days);
    }
}
