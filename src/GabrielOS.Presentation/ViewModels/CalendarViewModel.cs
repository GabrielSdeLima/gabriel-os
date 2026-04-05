using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Enums;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Presentation.ViewModels;

public class CalendarDayViewModel : ObservableObject
{
    public DateTime Date { get; set; }
    public int DayNumber => Date.Day;
    public bool IsCurrentMonth { get; set; }
    public bool IsToday { get; set; }
    public bool IsSelected { get; set; }
    public int? CheckInEnergy { get; set; }
    public List<CalendarItem> Events { get; set; } = new();

    public bool HasEvents => Events.Any(e => e.Source != CalendarItemSource.CheckIn);
    public bool HasCheckIn => CheckInEnergy.HasValue;

    public List<CalendarItem> EventDots => Events
        .Where(e => e.Source != CalendarItemSource.CheckIn)
        .Take(4)
        .ToList();
}

/// <summary>Legend entry for dynamic legend rendering.</summary>
public class LegendEntry
{
    public string Label { get; set; } = string.Empty;
    public string ColorKey { get; set; } = string.Empty;
}

public partial class CalendarViewModel : ObservableObject
{
    private readonly CalendarService _calendarService;
    private readonly GoalService _goalService;
    private readonly PillarService _pillarService;
    private readonly IUserRepository _userRepo;
    private Guid _userId;
    private Guid? _editingId;
    private List<CalendarItem> _allMonthItems = new();

    // Month display
    [ObservableProperty] private DateTime _currentMonth = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    [ObservableProperty] private string _monthYearDisplay = string.Empty;
    [ObservableProperty] private ObservableCollection<CalendarDayViewModel> _days = new();

    // Selected date
    [ObservableProperty] private DateTime _selectedDate = DateTime.Today;
    [ObservableProperty] private string _selectedDateDisplay = string.Empty;
    [ObservableProperty] private ObservableCollection<CalendarItem> _selectedDateItems = new();

    // Event form
    [ObservableProperty] private bool _showEventForm;
    [ObservableProperty] private bool _isEditMode;
    [ObservableProperty] private string _newTitle = string.Empty;
    [ObservableProperty] private string _newEventType = "Compromisso";
    [ObservableProperty] private DateTime _newDate = DateTime.Today;
    [ObservableProperty] private string _newStartTime = string.Empty;
    [ObservableProperty] private string _newEndTime = string.Empty;
    [ObservableProperty] private bool _newIsAllDay = true;
    [ObservableProperty] private string _newDescription = string.Empty;
    [ObservableProperty] private string _newLocation = string.Empty;
    [ObservableProperty] private Goal? _newGoal;
    [ObservableProperty] private Pillar? _newPillar;
    [ObservableProperty] private GoalPriority? _newPriority;

    // Lookups
    [ObservableProperty] private ObservableCollection<string> _eventTypeValues = new();
    [ObservableProperty] private ObservableCollection<Goal> _goals = new();
    [ObservableProperty] private ObservableCollection<Pillar> _pillars = new();

    // Legend (dynamic based on current month)
    [ObservableProperty] private ObservableCollection<LegendEntry> _legendEntries = new();

    // Stats
    [ObservableProperty] private int _monthEventCount;
    [ObservableProperty] private int _upcomingCount;
    [ObservableProperty] private bool _isLoading = true;

    public string FormTitle => IsEditMode ? "Editar Evento" : "Novo Evento";
    public string SaveButtonText => IsEditMode ? "Salvar" : "Adicionar";
    public IReadOnlyList<GoalPriority?> PriorityValues { get; } = new GoalPriority?[] { null, GoalPriority.P1, GoalPriority.P2, GoalPriority.P3, GoalPriority.P4 };

    // Color mapping for event type strings
    internal static readonly Dictionary<string, string> TypeColorMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Prova"] = "SemanticErrorBrush",
        ["Prazo"] = "SemanticErrorBrush",
        ["Trabalho"] = "SemanticWarningBrush",
        ["Compromisso"] = "AccentCoolBrush",
        ["Reunião"] = "SemanticInfoBrush",
        ["Lembrete"] = "AccentSecondaryBrush",
        ["Pessoal"] = "SemanticSuccessBrush",
    };

    internal static string GetColorKeyForType(string? eventType)
    {
        if (eventType != null && TypeColorMap.TryGetValue(eventType, out var key))
            return key;
        return "AccentPrimaryBrush";
    }

    partial void OnIsEditModeChanged(bool value)
    {
        OnPropertyChanged(nameof(FormTitle));
        OnPropertyChanged(nameof(SaveButtonText));
    }

    public CalendarViewModel(
        CalendarService calendarService,
        GoalService goalService,
        PillarService pillarService,
        IUserRepository userRepo)
    {
        _calendarService = calendarService;
        _goalService = goalService;
        _pillarService = pillarService;
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
            _userId = user.Id;

            var goals = await _goalService.GetByUserAsync(_userId);
            Goals = new ObservableCollection<Goal>(goals.Where(g => g.Status == GoalStatus.Active));

            var pillars = await _pillarService.GetAllOrderedAsync(_userId);
            Pillars = new ObservableCollection<Pillar>(pillars);

            var types = await _calendarService.GetUserEventTypesAsync(_userId);
            EventTypeValues = new ObservableCollection<string>(types);

            await RefreshCalendarAsync();
        }
        finally { IsLoading = false; }
    }

    private async Task RefreshCalendarAsync()
    {
        _allMonthItems = (await _calendarService.GetMonthItemsAsync(
            _userId, CurrentMonth.Year, CurrentMonth.Month)).ToList();

        BuildCalendarGrid();
        UpdateSelectedDateItems();
        UpdateStats();
        UpdateDisplayStrings();
        UpdateLegend();
    }

    private void BuildCalendarGrid()
    {
        var days = new List<CalendarDayViewModel>();
        var firstOfMonth = new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1);

        int dayOfWeek = ((int)firstOfMonth.DayOfWeek + 6) % 7; // Monday=0
        var gridStart = firstOfMonth.AddDays(-dayOfWeek);

        for (int i = 0; i < 42; i++)
        {
            var date = gridStart.AddDays(i);
            var dayEvents = _allMonthItems.Where(item => item.Date.Date == date.Date).ToList();
            var checkIn = dayEvents.FirstOrDefault(e => e.Source == CalendarItemSource.CheckIn);

            days.Add(new CalendarDayViewModel
            {
                Date = date,
                IsCurrentMonth = date.Month == CurrentMonth.Month,
                IsToday = date.Date == DateTime.Today,
                IsSelected = date.Date == SelectedDate.Date,
                CheckInEnergy = checkIn?.CheckInEnergy,
                Events = dayEvents,
            });
        }

        Days = new ObservableCollection<CalendarDayViewModel>(days);
    }

    private void UpdateSelectedDateItems()
    {
        var items = _allMonthItems
            .Where(i => i.Date.Date == SelectedDate.Date && i.Source != CalendarItemSource.CheckIn)
            .OrderBy(i => i.StartTime)
            .ToList();
        SelectedDateItems = new ObservableCollection<CalendarItem>(items);
    }

    private void UpdateStats()
    {
        MonthEventCount = _allMonthItems.Count(i =>
            i.Source != CalendarItemSource.CheckIn &&
            i.Date.Month == CurrentMonth.Month);
        UpcomingCount = _allMonthItems.Count(i =>
            i.Source != CalendarItemSource.CheckIn &&
            i.Date.Date >= DateTime.Today &&
            i.Date.Date <= DateTime.Today.AddDays(7));
    }

    private void UpdateDisplayStrings()
    {
        var culture = new CultureInfo("pt-BR");
        MonthYearDisplay = CurrentMonth.ToString("MMMM yyyy", culture);
        MonthYearDisplay = char.ToUpper(MonthYearDisplay[0]) + MonthYearDisplay[1..];

        SelectedDateDisplay = SelectedDate.ToString("dddd, d 'de' MMMM", culture);
        SelectedDateDisplay = char.ToUpper(SelectedDateDisplay[0]) + SelectedDateDisplay[1..];
    }

    private void UpdateLegend()
    {
        var entries = new List<LegendEntry>();

        // Collect distinct event types present this month
        var monthUserTypes = _allMonthItems
            .Where(i => i.Source == CalendarItemSource.UserEvent && i.EventType != null)
            .Select(i => i.EventType!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var type in monthUserTypes)
            entries.Add(new LegendEntry { Label = type, ColorKey = GetColorKeyForType(type) });

        // System sources present this month
        if (_allMonthItems.Any(i => i.Source == CalendarItemSource.GoalDeadline))
            entries.Add(new LegendEntry { Label = "Meta (prazo)", ColorKey = "AccentWarmBrush" });
        if (_allMonthItems.Any(i => i.Source == CalendarItemSource.GoalReview))
            entries.Add(new LegendEntry { Label = "Meta (revisão)", ColorKey = "AccentSecondaryBrush" });
        if (_allMonthItems.Any(i => i.Source == CalendarItemSource.TaskDue))
            entries.Add(new LegendEntry { Label = "Tarefa", ColorKey = "SemanticWarningBrush" });

        LegendEntries = new ObservableCollection<LegendEntry>(entries);
    }

    [RelayCommand]
    private async Task PreviousMonthAsync()
    {
        CurrentMonth = CurrentMonth.AddMonths(-1);
        await RefreshCalendarAsync();
    }

    [RelayCommand]
    private async Task NextMonthAsync()
    {
        CurrentMonth = CurrentMonth.AddMonths(1);
        await RefreshCalendarAsync();
    }

    [RelayCommand]
    private async Task GoToTodayAsync()
    {
        SelectedDate = DateTime.Today;
        CurrentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        await RefreshCalendarAsync();
    }

    [RelayCommand]
    private async Task SelectDayAsync(CalendarDayViewModel? day)
    {
        if (day == null) return;

        SelectedDate = day.Date;

        if (day.Date.Month != CurrentMonth.Month || day.Date.Year != CurrentMonth.Year)
        {
            CurrentMonth = new DateTime(day.Date.Year, day.Date.Month, 1);
            await RefreshCalendarAsync();
        }
        else
        {
            foreach (var d in Days)
                d.IsSelected = d.Date.Date == SelectedDate.Date;
            OnPropertyChanged(nameof(Days));
            UpdateSelectedDateItems();
            UpdateDisplayStrings();
        }
    }

    [RelayCommand]
    private void ShowNewEventForm()
    {
        _editingId = null;
        IsEditMode = false;
        NewTitle = string.Empty;
        NewEventType = "Compromisso";
        NewDate = SelectedDate;
        NewStartTime = string.Empty;
        NewEndTime = string.Empty;
        NewIsAllDay = true;
        NewDescription = string.Empty;
        NewLocation = string.Empty;
        NewGoal = null;
        NewPillar = null;
        NewPriority = null;
        ShowEventForm = true;
    }

    [RelayCommand]
    private void BeginEditEvent(CalendarItem? item)
    {
        if (item == null || item.Source != CalendarItemSource.UserEvent || !item.SourceId.HasValue) return;

        _editingId = item.SourceId.Value;
        IsEditMode = true;
        NewTitle = item.Title;
        NewEventType = item.EventType ?? "Compromisso";
        NewDate = item.Date;
        NewStartTime = item.StartTime?.ToString(@"hh\:mm") ?? string.Empty;
        NewEndTime = item.EndTime?.ToString(@"hh\:mm") ?? string.Empty;
        NewIsAllDay = item.IsAllDay;
        NewDescription = item.Description ?? string.Empty;
        NewLocation = item.Location ?? string.Empty;
        NewGoal = Goals.FirstOrDefault(g => g.Title == item.GoalTitle);
        NewPillar = Pillars.FirstOrDefault(p => p.Name == item.PillarName);
        NewPriority = item.Priority;
        ShowEventForm = true;
    }

    [RelayCommand]
    private async Task SaveEventAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTitle)) return;

        var eventType = string.IsNullOrWhiteSpace(NewEventType) ? "Compromisso" : NewEventType.Trim();
        TimeSpan? startTime = TryParseTime(NewStartTime);
        TimeSpan? endTime = TryParseTime(NewEndTime);

        if (IsEditMode && _editingId.HasValue)
        {
            var existing = (await _calendarService.GetByDateRangeAsync(_userId, NewDate.AddDays(-365), NewDate.AddDays(365)))
                .FirstOrDefault(e => e.Id == _editingId.Value);
            if (existing == null) return;

            existing.Title = NewTitle.Trim();
            existing.EventType = eventType;
            existing.Date = NewDate.Date;
            existing.StartTime = NewIsAllDay ? null : startTime;
            existing.EndTime = NewIsAllDay ? null : endTime;
            existing.IsAllDay = NewIsAllDay;
            existing.Description = string.IsNullOrWhiteSpace(NewDescription) ? null : NewDescription.Trim();
            existing.Location = string.IsNullOrWhiteSpace(NewLocation) ? null : NewLocation.Trim();
            existing.GoalId = NewGoal?.Id;
            existing.PillarId = NewPillar?.Id;
            existing.Priority = NewPriority;

            await _calendarService.UpdateAsync(existing);
        }
        else
        {
            var calendarEvent = new CalendarEvent
            {
                UserId = _userId,
                Title = NewTitle.Trim(),
                EventType = eventType,
                Date = NewDate.Date,
                StartTime = NewIsAllDay ? null : startTime,
                EndTime = NewIsAllDay ? null : endTime,
                IsAllDay = NewIsAllDay,
                Description = string.IsNullOrWhiteSpace(NewDescription) ? null : NewDescription.Trim(),
                Location = string.IsNullOrWhiteSpace(NewLocation) ? null : NewLocation.Trim(),
                GoalId = NewGoal?.Id,
                PillarId = NewPillar?.Id,
                Priority = NewPriority,
            };
            await _calendarService.CreateAsync(calendarEvent);
        }

        ShowEventForm = false;

        // Refresh type list in case user typed a new custom type
        var types = await _calendarService.GetUserEventTypesAsync(_userId);
        EventTypeValues = new ObservableCollection<string>(types);

        await RefreshCalendarAsync();
    }

    [RelayCommand]
    private void CancelEventForm()
    {
        ShowEventForm = false;
        _editingId = null;
        IsEditMode = false;
    }

    [RelayCommand]
    private async Task DeleteEventAsync(CalendarItem? item)
    {
        if (item == null || item.Source != CalendarItemSource.UserEvent || !item.SourceId.HasValue) return;

        var result = MessageBox.Show($"Excluir \"{item.Title}\"?",
            "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        await _calendarService.DeleteAsync(item.SourceId.Value);
        await RefreshCalendarAsync();
    }

    [RelayCommand]
    private async Task ToggleCompleteAsync(CalendarItem? item)
    {
        if (item == null || item.Source != CalendarItemSource.UserEvent || !item.SourceId.HasValue) return;

        var events = await _calendarService.GetByDateRangeAsync(_userId, item.Date.AddDays(-1), item.Date.AddDays(1));
        var existing = events.FirstOrDefault(e => e.Id == item.SourceId.Value);
        if (existing == null) return;

        existing.IsCompleted = !existing.IsCompleted;
        await _calendarService.UpdateAsync(existing);
        await RefreshCalendarAsync();
    }

    private static TimeSpan? TryParseTime(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        if (TimeSpan.TryParse(input, out var ts)) return ts;
        if (DateTime.TryParse(input, out var dt)) return dt.TimeOfDay;
        return null;
    }
}
