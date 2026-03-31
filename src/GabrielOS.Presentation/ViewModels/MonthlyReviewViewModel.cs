using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.Interfaces;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;
using GabrielOS.Presentation.Navigation;

namespace GabrielOS.Presentation.ViewModels;

public partial class MonthlyReviewViewModel : ObservableObject, IUnsavedChangesAware
{
    private readonly MonthlyReviewService _reviewService;
    private readonly PillarService _pillarService;
    private readonly AIContextBuilder _contextBuilder;
    private readonly IAIService _aiService;
    private readonly IUserRepository _userRepo;
    private Guid _userId;
    private bool _isInitialized;

    public bool HasUnsavedChanges { get; private set; }

    [ObservableProperty] private int _year = DateTime.Today.Year;
    [ObservableProperty] private int _month = DateTime.Today.Month;
    [ObservableProperty] private string _highlights = string.Empty;
    [ObservableProperty] private string _lowlights = string.Empty;
    [ObservableProperty] private string _keyLearnings = string.Empty;
    [ObservableProperty] private string _nextMonthIntentions = string.Empty;
    [ObservableProperty] private string _aiSummary = string.Empty;
    [ObservableProperty] private ObservableCollection<PillarScore> _pillarScores = new();
    [ObservableProperty] private ObservableCollection<MonthlyReview> _pastReviews = new();
    [ObservableProperty] private bool _isSaved;
    [ObservableProperty] private bool _isGeneratingAI;
    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private string _monthLabel = string.Empty;

    public bool AIAvailable => _aiService.IsConfigured;

    public IReadOnlyList<int> MonthNumbers { get; } = Enumerable.Range(1, 12).ToList();
    public IReadOnlyList<int> YearNumbers { get; } = Enumerable.Range(DateTime.Today.Year - 3, 5).ToList();

    partial void OnHighlightsChanged(string value) { if (_isInitialized) HasUnsavedChanges = true; }
    partial void OnLowlightsChanged(string value) { if (_isInitialized) HasUnsavedChanges = true; }
    partial void OnKeyLearningsChanged(string value) { if (_isInitialized) HasUnsavedChanges = true; }
    partial void OnNextMonthIntentionsChanged(string value) { if (_isInitialized) HasUnsavedChanges = true; }

    public MonthlyReviewViewModel(
        MonthlyReviewService reviewService,
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

            MonthLabel = new DateTime(Year, Month, 1).ToString("MMMM yyyy");

            var pillars = await _pillarService.GetAllOrderedAsync(_userId);
            PillarScores = new ObservableCollection<PillarScore>(
                pillars.Select(p => new PillarScore { PillarId = p.Id, PillarName = p.Name, Score = p.Score ?? 5 }));

            var existing = await _reviewService.GetByMonthAsync(_userId, Year, Month);
            if (existing != null)
            {
                Highlights = existing.Highlights ?? string.Empty;
                Lowlights = existing.Lowlights ?? string.Empty;
                KeyLearnings = existing.KeyLearnings ?? string.Empty;
                NextMonthIntentions = existing.NextMonthIntentions ?? string.Empty;
                AiSummary = existing.AISummary ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(existing.PillarScoresJson))
                {
                    try
                    {
                        var scores = JsonSerializer.Deserialize<Dictionary<string, int>>(existing.PillarScoresJson);
                        if (scores != null)
                            foreach (var ps in PillarScores)
                                if (scores.TryGetValue(ps.PillarId.ToString(), out var s))
                                    ps.Score = s;
                    }
                    catch { }
                }
            }

            var past = await _reviewService.GetRecentAsync(_userId, 12);
            PastReviews = new ObservableCollection<MonthlyReview>(past);
        }
        finally
        {
            HasUnsavedChanges = false;
            _isInitialized = true;
            IsLoading = false;
        }
    }

    partial void OnYearChanged(int value) { _isInitialized = false; _ = LoadAsync(); }
    partial void OnMonthChanged(int value) { _isInitialized = false; _ = LoadAsync(); }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var pillarScoresDict = PillarScores.ToDictionary(ps => ps.PillarId.ToString(), ps => ps.Score);
        var review = new MonthlyReview
        {
            UserId = _userId,
            Year = Year,
            Month = Month,
            Highlights = string.IsNullOrWhiteSpace(Highlights) ? null : Highlights.Trim(),
            Lowlights = string.IsNullOrWhiteSpace(Lowlights) ? null : Lowlights.Trim(),
            KeyLearnings = string.IsNullOrWhiteSpace(KeyLearnings) ? null : KeyLearnings.Trim(),
            NextMonthIntentions = string.IsNullOrWhiteSpace(NextMonthIntentions) ? null : NextMonthIntentions.Trim(),
            PillarScoresJson = JsonSerializer.Serialize(pillarScoresDict),
            AISummary = string.IsNullOrWhiteSpace(AiSummary) ? null : AiSummary.Trim(),
        };
        await _reviewService.SaveAsync(review);
        HasUnsavedChanges = false;
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
            var context = await _contextBuilder.BuildMonthlyContextAsync(_userId, Year, Month);
            var systemPrompt = "You are a personal life coach assistant. Analyze the user's monthly data and generate a concise, insightful summary (3-5 paragraphs). Focus on patterns, what's working, what needs attention, and actionable advice for the next month. Be direct and personal. Do not use bullet points — write in flowing prose.";
            var result = await _aiService.CompleteAsync(systemPrompt, context, 1500);
            if (result != null) AiSummary = result;
        }
        catch (Exception ex)
        {
            AiSummary = $"Error generating AI summary: {ex.Message}";
        }
        finally { IsGeneratingAI = false; }
    }

}
