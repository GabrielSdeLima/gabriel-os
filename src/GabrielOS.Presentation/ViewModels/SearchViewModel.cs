using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GabrielOS.Application.DTOs;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Presentation.ViewModels;

public partial class SearchViewModel : ObservableObject
{
    private readonly SearchService _searchService;
    private readonly IUserRepository _userRepo;
    private Guid _userId;

    [ObservableProperty] private string _query = string.Empty;
    [ObservableProperty] private ObservableCollection<SearchResult> _results = new();
    [ObservableProperty] private bool _isSearching;
    [ObservableProperty] private bool _hasSearched;
    [ObservableProperty] private string _resultSummary = string.Empty;

    public SearchViewModel(SearchService searchService, IUserRepository userRepo)
    {
        _searchService = searchService;
        _userRepo = userRepo;
        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        var user = await _userRepo.GetDefaultUserAsync();
        if (user != null) _userId = user.Id;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(Query)) return;
        IsSearching = true;
        HasSearched = false;
        try
        {
            var results = await _searchService.SearchAsync(_userId, Query.Trim());
            Results = new ObservableCollection<SearchResult>(results);
            HasSearched = true;
            ResultSummary = results.Count == 0
                ? "No results found."
                : $"{results.Count} result{(results.Count == 1 ? "" : "s")} across Goals, Decisions, Journal, and Patterns.";
        }
        finally { IsSearching = false; }
    }

    [RelayCommand]
    private void ClearSearch()
    {
        Query = string.Empty;
        Results.Clear();
        HasSearched = false;
        ResultSummary = string.Empty;
    }
}
