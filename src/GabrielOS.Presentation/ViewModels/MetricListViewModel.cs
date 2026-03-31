using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Presentation.ViewModels;

public partial class MetricListViewModel : ObservableObject
{
    private readonly MetricService _metricService;
    private readonly PillarService _pillarService;
    private readonly IUserRepository _userRepo;
    private Guid _userId;

    [ObservableProperty] private ObservableCollection<Metric> _metrics = new();
    [ObservableProperty] private ObservableCollection<Pillar> _pillars = new();

    // Form
    [ObservableProperty] private string _newName = string.Empty;
    [ObservableProperty] private string _newValueText = string.Empty;
    [ObservableProperty] private string _newUnit = string.Empty;
    [ObservableProperty] private string _newNotes = string.Empty;
    [ObservableProperty] private Pillar? _newPillar;
    [ObservableProperty] private DateTime _newDate = DateTime.Today;

    [ObservableProperty] private bool _isLoading = true;

    public MetricListViewModel(MetricService metricService, PillarService pillarService, IUserRepository userRepo)
    {
        _metricService = metricService;
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
            if (NewPillar == null && Pillars.Any())
                NewPillar = Pillars[0];

            var metrics = await _metricService.GetByUserAsync(_userId);
            Metrics = new ObservableCollection<Metric>(metrics);
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task SaveMetricAsync()
    {
        if (string.IsNullOrWhiteSpace(NewName) || NewPillar == null) return;
        if (!decimal.TryParse(NewValueText.Replace(',', '.'),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var value))
        {
            MessageBox.Show("Invalid value. Please enter a number.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var metric = new Metric
        {
            UserId = _userId,
            PillarId = NewPillar.Id,
            Name = NewName.Trim(),
            Value = value,
            Unit = string.IsNullOrWhiteSpace(NewUnit) ? null : NewUnit.Trim(),
            Notes = string.IsNullOrWhiteSpace(NewNotes) ? null : NewNotes.Trim(),
            Date = NewDate,
        };

        await _metricService.CreateAsync(metric);
        NewName = string.Empty;
        NewValueText = string.Empty;
        NewUnit = string.Empty;
        NewNotes = string.Empty;
        NewDate = DateTime.Today;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteMetricAsync(Metric? m)
    {
        if (m == null) return;
        var result = MessageBox.Show($"Delete metric \"{m.Name}\"?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        await _metricService.DeleteAsync(m.Id);
        await LoadAsync();
    }
}
