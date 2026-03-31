using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Enums;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Presentation.ViewModels;

public partial class PatternListViewModel : ObservableObject
{
    private readonly PatternService _patternService;
    private readonly IUserRepository _userRepo;
    private Guid _userId;
    private Guid? _editingId;

    [ObservableProperty] private ObservableCollection<Pattern> _patterns = new();
    [ObservableProperty] private string _newName = string.Empty;
    [ObservableProperty] private string _newDescription = string.Empty;
    [ObservableProperty] private string _newTrigger = string.Empty;
    [ObservableProperty] private PatternStatus _newStatus = PatternStatus.Emerging;
    [ObservableProperty] private bool _isEditMode;
    [ObservableProperty] private bool _isLoading = true;

    public IReadOnlyList<PatternStatus> StatusValues { get; } = Enum.GetValues<PatternStatus>();
    public string FormTitle => IsEditMode ? "Edit Pattern" : "New Pattern";
    public string SaveButtonText => IsEditMode ? "Save Changes" : "Add Pattern";

    partial void OnIsEditModeChanged(bool value)
    {
        OnPropertyChanged(nameof(FormTitle));
        OnPropertyChanged(nameof(SaveButtonText));
    }

    public PatternListViewModel(PatternService patternService, IUserRepository userRepo)
    {
        _patternService = patternService;
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

            var patterns = await _patternService.GetByUserAsync(_userId);
            Patterns = new ObservableCollection<Pattern>(patterns);
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task SavePatternAsync()
    {
        if (string.IsNullOrWhiteSpace(NewName)) return;

        if (IsEditMode && _editingId.HasValue)
        {
            var p = Patterns.FirstOrDefault(x => x.Id == _editingId.Value);
            if (p == null) return;

            p.Name = NewName.Trim();
            p.Description = string.IsNullOrWhiteSpace(NewDescription) ? null : NewDescription.Trim();
            p.Trigger = string.IsNullOrWhiteSpace(NewTrigger) ? null : NewTrigger.Trim();
            p.Status = NewStatus;

            await _patternService.UpdateAsync(p);
            CancelEdit();
        }
        else
        {
            var p = new Pattern
            {
                UserId = _userId,
                Name = NewName.Trim(),
                Description = string.IsNullOrWhiteSpace(NewDescription) ? null : NewDescription.Trim(),
                Trigger = string.IsNullOrWhiteSpace(NewTrigger) ? null : NewTrigger.Trim(),
                Status = NewStatus,
            };
            await _patternService.CreateAsync(p);
            ResetForm();
        }

        await LoadAsync();
    }

    [RelayCommand]
    private void BeginEditPattern(Pattern? p)
    {
        if (p == null) return;
        _editingId = p.Id;
        NewName = p.Name;
        NewDescription = p.Description ?? string.Empty;
        NewTrigger = p.Trigger ?? string.Empty;
        NewStatus = p.Status;
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
    private async Task DeletePatternAsync(Pattern? p)
    {
        if (p == null) return;
        var result = MessageBox.Show($"Delete pattern \"{p.Name}\"?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        await _patternService.DeleteAsync(p.Id);
        await LoadAsync();
    }

    private void ResetForm()
    {
        NewName = string.Empty;
        NewDescription = string.Empty;
        NewTrigger = string.Empty;
        NewStatus = PatternStatus.Emerging;
    }
}
