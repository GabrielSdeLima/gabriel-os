using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Interfaces;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Presentation.ViewModels;

public class PillarScore : ObservableObject
{
    public Guid PillarId { get; init; }
    public string PillarName { get; init; } = string.Empty;

    private int _score = 5;
    public int Score
    {
        get => _score;
        set => SetProperty(ref _score, value);
    }
}

public partial class WeeklyReviewViewModel : ObservableObject
{
    private readonly ReviewService _reviewService;
    private readonly PillarService _pillarService;
    private readonly AIContextBuilder _contextBuilder;
    private readonly IAIService _aiService;
    private readonly IUserRepository _userRepo;
    private Guid _userId;
    private WeeklyReview? _currentReview;

    [ObservableProperty] private string _wins = string.Empty;
    [ObservableProperty] private string _frictions = string.Empty;
    [ObservableProperty] private string _avoidedThings = string.Empty;
    [ObservableProperty] private string _energyDrains = string.Empty;
    [ObservableProperty] private string _energyGains = string.Empty;
    [ObservableProperty] private string _mainInsight = string.Empty;
    [ObservableProperty] private string _nextWeekFocus = string.Empty;
    [ObservableProperty] private string _notes = string.Empty;
    [ObservableProperty] private string _aiSummary = string.Empty;
    [ObservableProperty] private ObservableCollection<PillarScore> _pillarScores = new();
    [ObservableProperty] private ObservableCollection<WeeklyReview> _pastReviews = new();
    [ObservableProperty] private bool _isSaved;
    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private bool _isGeneratingAI;
    [ObservableProperty] private string _weekLabel = string.Empty;

    public bool AIAvailable => _aiService.IsConfigured;

    public WeeklyReviewViewModel(
        ReviewService reviewService,
        PillarService pillarService,
        AIContextBuilder contextBuilder,
        IAIService aiService,
        IUserRepository userRepo)
    {
        _reviewService = reviewService;
        _pillarService = pillarService;
        _contextBuilder = contextBuilder;
        _aiService = aiService;
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

            var weekStart = ReviewService.GetCurrentWeekStart();
            WeekLabel = $"Week of {weekStart:dd MMM yyyy}";

            var pillars = await _pillarService.GetAllOrderedAsync(_userId);
            PillarScores = new ObservableCollection<PillarScore>(
                pillars.Select(p => new PillarScore { PillarId = p.Id, PillarName = p.Name, Score = p.Score ?? 5 }));

            _currentReview = await _reviewService.GetByWeekAsync(_userId, weekStart);
            if (_currentReview != null)
            {
                Wins = _currentReview.Wins ?? string.Empty;
                Frictions = _currentReview.Frictions ?? string.Empty;
                AvoidedThings = _currentReview.AvoidedThings ?? string.Empty;
                EnergyDrains = _currentReview.EnergyDrains ?? string.Empty;
                EnergyGains = _currentReview.EnergyGains ?? string.Empty;
                MainInsight = _currentReview.MainInsight ?? string.Empty;
                NextWeekFocus = _currentReview.NextWeekFocus ?? string.Empty;
                Notes = _currentReview.Notes ?? string.Empty;
                AiSummary = _currentReview.AISummary ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(_currentReview.PillarScoresJson))
                {
                    try
                    {
                        var scores = JsonSerializer.Deserialize<Dictionary<string, int>>(_currentReview.PillarScoresJson);
                        if (scores != null)
                            foreach (var ps in PillarScores)
                                if (scores.TryGetValue(ps.PillarId.ToString(), out var s))
                                    ps.Score = s;
                    }
                    catch { }
                }
            }

            var pastReviews = await _reviewService.GetRecentAsync(_userId, 8);
            PastReviews = new ObservableCollection<WeeklyReview>(pastReviews);
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var pillarScoresDict = PillarScores.ToDictionary(ps => ps.PillarId.ToString(), ps => ps.Score);
        var pillarScoresJson = JsonSerializer.Serialize(pillarScoresDict);

        var review = new WeeklyReview
        {
            UserId = _userId,
            WeekStart = ReviewService.GetCurrentWeekStart(),
            Wins = string.IsNullOrWhiteSpace(Wins) ? null : Wins.Trim(),
            Frictions = string.IsNullOrWhiteSpace(Frictions) ? null : Frictions.Trim(),
            AvoidedThings = string.IsNullOrWhiteSpace(AvoidedThings) ? null : AvoidedThings.Trim(),
            EnergyDrains = string.IsNullOrWhiteSpace(EnergyDrains) ? null : EnergyDrains.Trim(),
            EnergyGains = string.IsNullOrWhiteSpace(EnergyGains) ? null : EnergyGains.Trim(),
            MainInsight = string.IsNullOrWhiteSpace(MainInsight) ? null : MainInsight.Trim(),
            NextWeekFocus = string.IsNullOrWhiteSpace(NextWeekFocus) ? null : NextWeekFocus.Trim(),
            PillarScoresJson = pillarScoresJson,
            Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim(),
            AISummary = string.IsNullOrWhiteSpace(AiSummary) ? null : AiSummary.Trim(),
        };

        await _reviewService.SaveAsync(review);
        IsSaved = true;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task GenerateAISummaryAsync()
    {
        if (!_aiService.IsConfigured) return;
        IsGeneratingAI = true;
        try
        {
            // Save first so context is fresh
            var reviewForContext = new WeeklyReview
            {
                UserId = _userId,
                WeekStart = ReviewService.GetCurrentWeekStart(),
                Wins = Wins, Frictions = Frictions, AvoidedThings = AvoidedThings,
                EnergyDrains = EnergyDrains, EnergyGains = EnergyGains,
                MainInsight = MainInsight, NextWeekFocus = NextWeekFocus,
            };

            var context = await _contextBuilder.BuildWeeklyReviewContextAsync(_userId, reviewForContext);
            var systemPrompt = "You are a personal life coach assistant. Analyze the user's weekly review data and provide a concise, insightful summary (2-4 paragraphs). Identify patterns in their energy, mood, and progress. Highlight what's working and what deserves attention. Offer one concrete suggestion. Be personal and direct. Do not use bullet points.";
            var result = await _aiService.CompleteAsync(systemPrompt, context, 1200);
            if (result != null) AiSummary = result;
        }
        catch (Exception ex)
        {
            AiSummary = $"Error: {ex.Message}";
        }
        finally { IsGeneratingAI = false; }
    }
}
