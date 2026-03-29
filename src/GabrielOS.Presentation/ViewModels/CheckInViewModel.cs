using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Rules;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Enums;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Presentation.ViewModels;

public partial class CheckInViewModel : ObservableObject
{
    private readonly CheckInService _checkInService;
    private readonly IUserRepository _userRepo;
    private Guid _userId;

    [ObservableProperty] private int _energy = 5;
    [ObservableProperty] private int _mood = 5;
    [ObservableProperty] private int _clarity = 5;
    [ObservableProperty] private int _tension = 5;
    [ObservableProperty] private string _physicalState = string.Empty;
    [ObservableProperty] private string _topConcern = string.Empty;
    [ObservableProperty] private string _topPriority = string.Empty;
    [ObservableProperty] private string _freeText = string.Empty;

    [ObservableProperty] private string _suggestedModeText = string.Empty;
    [ObservableProperty] private bool _isSaved;
    [ObservableProperty] private bool _isLoading = true;

    public CheckInViewModel(CheckInService checkInService, IUserRepository userRepo)
    {
        _checkInService = checkInService;
        _userRepo = userRepo;
        _ = LoadAsync();
    }

    partial void OnEnergyChanged(int value) => UpdatePreview();
    partial void OnMoodChanged(int value) => UpdatePreview();
    partial void OnClarityChanged(int value) => UpdatePreview();
    partial void OnTensionChanged(int value) => UpdatePreview();

    private void UpdatePreview()
    {
        var mode = ModeCalculator.Calculate(Energy, Mood, Clarity, Tension);
        SuggestedModeText = $"{mode} — {ModeCalculator.GetModeDescription(mode)}";
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

            var today = await _checkInService.GetTodayAsync(_userId);
            if (today != null)
            {
                Energy = today.Energy;
                Mood = today.Mood;
                Clarity = today.Clarity;
                Tension = today.Tension;
                PhysicalState = today.PhysicalState ?? string.Empty;
                TopConcern = today.TopConcern ?? string.Empty;
                TopPriority = today.TopPriority ?? string.Empty;
                FreeText = today.FreeText ?? string.Empty;
                IsSaved = true;
            }
            UpdatePreview();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var checkIn = new CheckIn
        {
            UserId = _userId,
            Date = DateTime.UtcNow.Date,
            Energy = Energy,
            Mood = Mood,
            Clarity = Clarity,
            Tension = Tension,
            PhysicalState = string.IsNullOrWhiteSpace(PhysicalState) ? null : PhysicalState,
            TopConcern = string.IsNullOrWhiteSpace(TopConcern) ? null : TopConcern,
            TopPriority = string.IsNullOrWhiteSpace(TopPriority) ? null : TopPriority,
            FreeText = string.IsNullOrWhiteSpace(FreeText) ? null : FreeText,
        };

        await _checkInService.SaveAsync(checkIn);
        IsSaved = true;
        UpdatePreview();
    }
}
