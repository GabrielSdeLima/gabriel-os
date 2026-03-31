using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Enums;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Presentation.ViewModels;

public partial class GoalListViewModel : ObservableObject
{
    private readonly GoalService _goalService;
    private readonly PillarService _pillarService;
    private readonly IUserRepository _userRepo;
    private Guid _userId;
    private Guid? _editingGoalId;

    [ObservableProperty] private ObservableCollection<Goal> _goals = new();
    [ObservableProperty] private ObservableCollection<Pillar> _pillars = new();

    // Form fields
    [ObservableProperty] private string _newTitle = string.Empty;
    [ObservableProperty] private string _newDescription = string.Empty;
    [ObservableProperty] private string _newNextAction = string.Empty;
    [ObservableProperty] private GoalStatus _newStatus = GoalStatus.Idea;
    [ObservableProperty] private GoalPriority _newPriority = GoalPriority.P3;
    [ObservableProperty] private HorizonType _newHorizonType = HorizonType.Quarterly;
    [ObservableProperty] private Pillar? _newPillar;

    // State
    [ObservableProperty] private bool _isEditMode;
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isLoading = true;

    public IReadOnlyList<GoalStatus> StatusValues { get; } = Enum.GetValues<GoalStatus>();
    public IReadOnlyList<GoalPriority> PriorityValues { get; } = Enum.GetValues<GoalPriority>();
    public IReadOnlyList<HorizonType> HorizonTypes { get; } = Enum.GetValues<HorizonType>();

    public string FormTitle => IsEditMode ? "Edit Goal" : "New Goal";
    public string SaveButtonText => IsEditMode ? "Save Changes" : "Add Goal";

    partial void OnIsEditModeChanged(bool value)
    {
        OnPropertyChanged(nameof(FormTitle));
        OnPropertyChanged(nameof(SaveButtonText));
    }

    public GoalListViewModel(GoalService goalService, PillarService pillarService, IUserRepository userRepo)
    {
        _goalService = goalService;
        _pillarService = pillarService;
        _userRepo = userRepo;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var user = await _userRepo.GetDefaultUserAsync();
            if (user == null) return;
            _userId = user.Id;

            var pillars = await _pillarService.GetAllOrderedAsync(_userId);
            Pillars = new ObservableCollection<Pillar>(pillars);
            if (NewPillar == null && Pillars.Any())
                NewPillar = Pillars[0];

            var goals = await _goalService.GetByUserAsync(_userId);
            Goals = new ObservableCollection<Goal>(goals);
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task SaveGoalAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTitle)) return;
        ErrorMessage = null;

        if (IsEditMode && _editingGoalId.HasValue)
        {
            var goal = Goals.FirstOrDefault(g => g.Id == _editingGoalId.Value);
            if (goal == null) return;

            goal.Title = NewTitle.Trim();
            goal.Description = string.IsNullOrWhiteSpace(NewDescription) ? null : NewDescription.Trim();
            goal.NextAction = string.IsNullOrWhiteSpace(NewNextAction) ? null : NewNextAction.Trim();
            goal.Status = NewStatus;
            goal.Priority = NewPriority;
            goal.HorizonType = NewHorizonType;
            if (NewPillar != null) goal.PillarId = NewPillar.Id;

            var (ok, err) = await _goalService.UpdateAsync(goal);
            if (!ok) { ErrorMessage = err; return; }
            CancelEdit();
        }
        else
        {
            var goal = new Goal
            {
                UserId = _userId,
                PillarId = NewPillar?.Id ?? Pillars.FirstOrDefault()?.Id ?? Guid.Empty,
                Title = NewTitle.Trim(),
                Description = string.IsNullOrWhiteSpace(NewDescription) ? null : NewDescription.Trim(),
                NextAction = string.IsNullOrWhiteSpace(NewNextAction) ? null : NewNextAction.Trim(),
                Status = NewStatus,
                Priority = NewPriority,
                HorizonType = NewHorizonType,
            };

            var (ok, err) = await _goalService.CreateAsync(goal);
            if (!ok) { ErrorMessage = err; return; }
            ResetForm();
        }

        await LoadAsync();
    }

    [RelayCommand]
    private void BeginEditGoal(Goal? goal)
    {
        if (goal == null) return;
        _editingGoalId = goal.Id;
        NewTitle = goal.Title;
        NewDescription = goal.Description ?? string.Empty;
        NewNextAction = goal.NextAction ?? string.Empty;
        NewStatus = goal.Status;
        NewPriority = goal.Priority;
        NewHorizonType = goal.HorizonType;
        NewPillar = Pillars.FirstOrDefault(p => p.Id == goal.PillarId);
        IsEditMode = true;
        ErrorMessage = null;
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditMode = false;
        _editingGoalId = null;
        ResetForm();
    }

    [RelayCommand]
    private async Task DeleteGoalAsync(Goal? goal)
    {
        if (goal == null) return;
        var result = MessageBox.Show(
            $"Delete goal \"{goal.Title}\"? This cannot be undone.",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        await _goalService.DeleteAsync(goal.Id);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery)) { await LoadAsync(); return; }
        var results = await _goalService.SearchAsync(_userId, SearchQuery);
        Goals = new ObservableCollection<Goal>(results);
    }

    private void ResetForm()
    {
        NewTitle = string.Empty;
        NewDescription = string.Empty;
        NewNextAction = string.Empty;
        NewStatus = GoalStatus.Idea;
        NewPriority = GoalPriority.P3;
        NewHorizonType = HorizonType.Quarterly;
        NewPillar = Pillars.FirstOrDefault();
        ErrorMessage = null;
    }
}
