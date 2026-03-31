using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Presentation.ViewModels;

public partial class CycleFocusViewModel : ObservableObject
{
    private readonly CycleFocusService _cycleFocusService;
    private readonly GoalService _goalService;
    private readonly IUserRepository _userRepo;
    private Guid _userId;

    [ObservableProperty] private CycleFocus? _activeCycleFocus;
    [ObservableProperty] private ObservableCollection<CycleFocus> _pastCycles = new();
    [ObservableProperty] private ObservableCollection<Goal> _availableGoals = new();

    // New cycle form
    [ObservableProperty] private string _newTitle = string.Empty;
    [ObservableProperty] private string _newThesis = string.Empty;
    [ObservableProperty] private DateTime _newStartDate = DateTime.Today;
    [ObservableProperty] private DateTime _newEndDate = DateTime.Today.AddDays(30);
    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private bool _isSaved;

    public CycleFocusViewModel(CycleFocusService cycleFocusService, GoalService goalService, IUserRepository userRepo)
    {
        _cycleFocusService = cycleFocusService;
        _goalService = goalService;
        _userRepo = userRepo;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        IsSaved = false;
        try
        {
            var user = await _userRepo.GetDefaultUserAsync();
            if (user == null) return;
            _userId = user.Id;

            ActiveCycleFocus = await _cycleFocusService.GetActiveAsync(_userId);
            var all = await _cycleFocusService.GetByUserAsync(_userId);
            PastCycles = new ObservableCollection<CycleFocus>(all.Where(c => !c.IsActive));

            var goals = await _goalService.GetByUserAsync(_userId);
            AvailableGoals = new ObservableCollection<Goal>(
                goals.Where(g => g.Status == Domain.Enums.GoalStatus.Active ||
                                 g.Status == Domain.Enums.GoalStatus.Incubating));
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task CreateCycleAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTitle)) return;

        var focus = new CycleFocus
        {
            UserId = _userId,
            Title = NewTitle.Trim(),
            Thesis = string.IsNullOrWhiteSpace(NewThesis) ? null : NewThesis.Trim(),
            StartDate = NewStartDate,
            EndDate = NewEndDate,
            IsActive = true,
        };

        var (ok, err) = await _cycleFocusService.CreateAsync(focus, new List<Guid>());
        if (!ok) { MessageBox.Show(err, "Error", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

        NewTitle = string.Empty;
        NewThesis = string.Empty;
        NewStartDate = DateTime.Today;
        NewEndDate = DateTime.Today.AddDays(30);
        IsSaved = true;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeactivateCycleAsync()
    {
        if (ActiveCycleFocus == null) return;
        var result = MessageBox.Show("Mark this cycle as complete?",
            "Complete Cycle", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes) return;

        ActiveCycleFocus.IsActive = false;
        await _cycleFocusService.UpdateAsync(ActiveCycleFocus);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteCycleAsync(CycleFocus? cycle)
    {
        if (cycle == null) return;
        var result = MessageBox.Show($"Delete cycle \"{cycle.Title}\"?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        await _cycleFocusService.DeleteAsync(cycle.Id);
        await LoadAsync();
    }
}
