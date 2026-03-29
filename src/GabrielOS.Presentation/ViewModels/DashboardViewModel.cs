using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Rules;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Presentation.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly PillarService _pillarService;
    private readonly CheckInService _checkInService;
    private readonly JournalService _journalService;
    private readonly IUserRepository _userRepo;

    [ObservableProperty]
    private ObservableCollection<Pillar> _pillars = new();

    [ObservableProperty]
    private CheckIn? _todayCheckIn;

    [ObservableProperty]
    private string _suggestedModeText = "No check-in yet today.";

    [ObservableProperty]
    private int _weeklyJournalCount;

    [ObservableProperty]
    private bool _isLoading = true;

    public DashboardViewModel(
        PillarService pillarService,
        CheckInService checkInService,
        JournalService journalService,
        IUserRepository userRepo)
    {
        _pillarService = pillarService;
        _checkInService = checkInService;
        _journalService = journalService;
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
            SuggestedModeText = TodayCheckIn?.SuggestedMode != null
                ? $"{TodayCheckIn.SuggestedMode} — {ModeCalculator.GetModeDescription(TodayCheckIn.SuggestedMode.Value)}"
                : "No check-in yet today. Start your day with a quick check-in.";

            WeeklyJournalCount = await _journalService.GetWeeklyCountAsync(user.Id);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
