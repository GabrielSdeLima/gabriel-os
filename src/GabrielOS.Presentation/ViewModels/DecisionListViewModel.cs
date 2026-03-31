using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Enums;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Presentation.ViewModels;

public partial class DecisionListViewModel : ObservableObject
{
    private readonly DecisionService _decisionService;
    private readonly PillarService _pillarService;
    private readonly IUserRepository _userRepo;
    private Guid _userId;
    private Guid? _editingId;

    [ObservableProperty] private ObservableCollection<Decision> _decisions = new();
    [ObservableProperty] private ObservableCollection<Pillar> _pillars = new();

    // Form fields
    [ObservableProperty] private string _newTitle = string.Empty;
    [ObservableProperty] private string _newContext = string.Empty;
    [ObservableProperty] private string _newChosenOption = string.Empty;
    [ObservableProperty] private string _newRationale = string.Empty;
    [ObservableProperty] private DecisionStatus _newStatus = DecisionStatus.Active;
    [ObservableProperty] private Pillar? _newPillar;

    // State
    [ObservableProperty] private bool _isEditMode;
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private bool _isLoading = true;

    public IReadOnlyList<DecisionStatus> StatusValues { get; } = Enum.GetValues<DecisionStatus>();
    public string FormTitle => IsEditMode ? "Edit Decision" : "Log Decision";
    public string SaveButtonText => IsEditMode ? "Save Changes" : "Log Decision";

    partial void OnIsEditModeChanged(bool value)
    {
        OnPropertyChanged(nameof(FormTitle));
        OnPropertyChanged(nameof(SaveButtonText));
    }

    public DecisionListViewModel(DecisionService decisionService, PillarService pillarService, IUserRepository userRepo)
    {
        _decisionService = decisionService;
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

            var pillars = await _pillarService.GetAllOrderedAsync(_userId);
            Pillars = new ObservableCollection<Pillar>(pillars);

            var decisions = await _decisionService.GetByUserAsync(_userId);
            Decisions = new ObservableCollection<Decision>(decisions);
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task SaveDecisionAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTitle) || string.IsNullOrWhiteSpace(NewContext)) return;

        if (IsEditMode && _editingId.HasValue)
        {
            var d = Decisions.FirstOrDefault(x => x.Id == _editingId.Value);
            if (d == null) return;

            d.Title = NewTitle.Trim();
            d.Context = NewContext.Trim();
            d.ChosenOption = string.IsNullOrWhiteSpace(NewChosenOption) ? null : NewChosenOption.Trim();
            d.Rationale = string.IsNullOrWhiteSpace(NewRationale) ? null : NewRationale.Trim();
            d.Status = NewStatus;
            d.PillarId = NewPillar?.Id;

            await _decisionService.UpdateAsync(d);
            CancelEdit();
        }
        else
        {
            var d = new Decision
            {
                UserId = _userId,
                Title = NewTitle.Trim(),
                Context = NewContext.Trim(),
                ChosenOption = string.IsNullOrWhiteSpace(NewChosenOption) ? null : NewChosenOption.Trim(),
                Rationale = string.IsNullOrWhiteSpace(NewRationale) ? null : NewRationale.Trim(),
                Status = NewStatus,
                PillarId = NewPillar?.Id,
            };
            await _decisionService.CreateAsync(d);
            ResetForm();
        }

        await LoadAsync();
    }

    [RelayCommand]
    private void BeginEditDecision(Decision? d)
    {
        if (d == null) return;
        _editingId = d.Id;
        NewTitle = d.Title;
        NewContext = d.Context;
        NewChosenOption = d.ChosenOption ?? string.Empty;
        NewRationale = d.Rationale ?? string.Empty;
        NewStatus = d.Status;
        NewPillar = Pillars.FirstOrDefault(p => p.Id == d.PillarId);
        IsEditMode = true;
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditMode = false;
        _editingId = null;
        ResetForm();
    }

    [RelayCommand]
    private async Task DeleteDecisionAsync(Decision? d)
    {
        if (d == null) return;
        var result = MessageBox.Show(
            $"Delete decision \"{d.Title}\"?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        await _decisionService.DeleteAsync(d.Id);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery)) { await LoadAsync(); return; }
        var results = await _decisionService.SearchAsync(_userId, SearchQuery);
        Decisions = new ObservableCollection<Decision>(results);
    }

    private void ResetForm()
    {
        NewTitle = string.Empty;
        NewContext = string.Empty;
        NewChosenOption = string.Empty;
        NewRationale = string.Empty;
        NewStatus = DecisionStatus.Active;
        NewPillar = null;
    }
}
