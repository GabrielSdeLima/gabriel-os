using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace GabrielOS.Presentation.ViewModels;

public partial class PillarTrendViewModel : ObservableObject
{
    private readonly PillarService _pillarService;
    private readonly ReviewService _reviewService;
    private readonly IUserRepository _userRepo;

    [ObservableProperty] private PlotModel _pillarTrendModel = new();
    [ObservableProperty] private ObservableCollection<Pillar> _pillars = new();
    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private int _weeksToShow = 8;
    [ObservableProperty] private bool _hasData;

    public IReadOnlyList<int> WeekOptions { get; } = new[] { 4, 8, 12, 24 };

    public PillarTrendViewModel(PillarService pillarService, ReviewService reviewService, IUserRepository userRepo)
    {
        _pillarService = pillarService;
        _reviewService = reviewService;
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

            var reviews = await _reviewService.GetRecentAsync(user.Id, WeeksToShow);
            BuildChart(pillars, reviews);
        }
        finally { IsLoading = false; }
    }

    partial void OnWeeksToShowChanged(int value) => _ = LoadAsync();

    private void BuildChart(IReadOnlyList<Pillar> pillars, IReadOnlyList<WeeklyReview> reviews)
    {
        var model = new PlotModel
        {
            Background = OxyColor.FromRgb(0x1A, 0x1A, 0x2E),
            PlotAreaBorderColor = OxyColor.FromRgb(0x2D, 0x2D, 0x4E),
            TextColor = OxyColor.FromRgb(0xCC, 0xCC, 0xDD),
            TitleColor = OxyColor.FromRgb(0xEE, 0xEE, 0xFF),
        };

        model.Axes.Add(new DateTimeAxis
        {
            Position = AxisPosition.Bottom,
            StringFormat = "dd MMM",
            TextColor = OxyColor.FromRgb(0xAA, 0xAA, 0xBB),
            AxislineColor = OxyColor.FromRgb(0x2D, 0x2D, 0x4E),
            MajorGridlineStyle = LineStyle.Dot,
            MajorGridlineColor = OxyColor.FromRgb(0x2D, 0x2D, 0x4E),
        });

        model.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            Minimum = 0,
            Maximum = 10,
            MajorStep = 2,
            TextColor = OxyColor.FromRgb(0xAA, 0xAA, 0xBB),
            AxislineColor = OxyColor.FromRgb(0x2D, 0x2D, 0x4E),
            MajorGridlineStyle = LineStyle.Dot,
            MajorGridlineColor = OxyColor.FromRgb(0x2D, 0x2D, 0x4E),
        });

        var palette = new[]
        {
            OxyColor.FromRgb(0x6C, 0x63, 0xFF),
            OxyColor.FromRgb(0x00, 0xD4, 0xAA),
            OxyColor.FromRgb(0xFF, 0x6B, 0x6B),
            OxyColor.FromRgb(0xFF, 0xB8, 0x47),
            OxyColor.FromRgb(0x47, 0xC8, 0xFF),
            OxyColor.FromRgb(0xBD, 0xFF, 0x81),
            OxyColor.FromRgb(0xFF, 0x81, 0xCE),
        };

        var sortedReviews = reviews.OrderBy(r => r.WeekStart).ToList();

        for (int i = 0; i < pillars.Count; i++)
        {
            var pillar = pillars[i];
            var color = palette[i % palette.Length];
            var series = new LineSeries
            {
                Title = pillar.Name,
                Color = color,
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerFill = color,
                StrokeThickness = 2,
            };

            foreach (var review in sortedReviews)
            {
                if (string.IsNullOrWhiteSpace(review.PillarScoresJson)) continue;
                try
                {
                    var scores = JsonSerializer.Deserialize<Dictionary<string, int>>(review.PillarScoresJson);
                    if (scores != null && scores.TryGetValue(pillar.Id.ToString(), out var score))
                        series.Points.Add(DateTimeAxis.CreateDataPoint(review.WeekStart, score));
                }
                catch { }
            }

            if (series.Points.Any())
                model.Series.Add(series);
        }

        HasData = model.Series.Count > 0;
        PillarTrendModel = model;
    }
}
