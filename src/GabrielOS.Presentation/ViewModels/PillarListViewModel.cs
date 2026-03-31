using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Enums;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Presentation.ViewModels;

public partial class PillarListViewModel : ObservableObject
{
    private readonly PillarService _pillarService;
    private readonly IUserRepository _userRepo;
    private Guid _userId;

    [ObservableProperty]
    private ObservableCollection<Pillar> _pillars = new();

    [ObservableProperty]
    private Pillar? _selectedPillar;

    [ObservableProperty]
    private string _newPillarName = string.Empty;

    [ObservableProperty]
    private bool _isLoading = true;

    public IReadOnlyList<PillarPriority> PillarPriorities { get; } = Enum.GetValues<PillarPriority>();

    public PillarListViewModel(PillarService pillarService, IUserRepository userRepo)
    {
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

            var pillars = await _pillarService.GetAllOrderedAsync(user.Id);
            Pillars = new ObservableCollection<Pillar>(pillars);
            if (SelectedPillar == null && Pillars.Count > 0)
                SelectedPillar = Pillars[0];
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SavePillarAsync()
    {
        if (SelectedPillar == null) return;
        var savedId = SelectedPillar.Id;
        await _pillarService.UpdateAsync(SelectedPillar);
        await LoadAsync();
        SelectedPillar = Pillars.FirstOrDefault(p => p.Id == savedId);
    }

    [RelayCommand]
    private async Task AddPillarAsync()
    {
        if (string.IsNullOrWhiteSpace(NewPillarName)) return;

        var maxOrder = Pillars.Count > 0 ? Pillars.Max(p => p.SortOrder) : -1;
        var pillar = new Pillar
        {
            UserId = _userId,
            Name = NewPillarName.Trim(),
            Priority = PillarPriority.Medium,
            Trend = Trend.Unknown,
            SortOrder = maxOrder + 1
        };

        await _pillarService.CreateAsync(pillar);
        NewPillarName = string.Empty;
        await LoadAsync();
        SelectedPillar = Pillars.FirstOrDefault(p => p.Id == pillar.Id);
    }

    [RelayCommand]
    private async Task DeletePillarAsync(Pillar? pillar)
    {
        if (pillar == null) return;

        var result = MessageBox.Show(
            $"Delete pillar '{pillar.Name}'? This cannot be undone.",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        if (SelectedPillar?.Id == pillar.Id)
            SelectedPillar = null;

        await _pillarService.DeleteAsync(pillar.Id);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task MovePillarUpAsync(Pillar? pillar)
    {
        if (pillar == null) return;
        var index = Pillars.IndexOf(pillar);
        if (index <= 0) return;

        var above = Pillars[index - 1];
        (pillar.SortOrder, above.SortOrder) = (above.SortOrder, pillar.SortOrder);

        await _pillarService.UpdateAsync(pillar);
        await _pillarService.UpdateAsync(above);

        var savedId = pillar.Id;
        await LoadAsync();
        SelectedPillar = Pillars.FirstOrDefault(p => p.Id == savedId);
    }

    [RelayCommand]
    private async Task MovePillarDownAsync(Pillar? pillar)
    {
        if (pillar == null) return;
        var index = Pillars.IndexOf(pillar);
        if (index >= Pillars.Count - 1) return;

        var below = Pillars[index + 1];
        (pillar.SortOrder, below.SortOrder) = (below.SortOrder, pillar.SortOrder);

        await _pillarService.UpdateAsync(pillar);
        await _pillarService.UpdateAsync(below);

        var savedId = pillar.Id;
        await LoadAsync();
        SelectedPillar = Pillars.FirstOrDefault(p => p.Id == savedId);
    }
}
