using GabrielOS.Application.DTOs;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Application.Services;

public class SearchService
{
    private readonly IGoalRepository _goalRepo;
    private readonly IDecisionRepository _decisionRepo;
    private readonly IJournalEntryRepository _journalRepo;
    private readonly IPatternRepository _patternRepo;

    public SearchService(
        IGoalRepository goalRepo,
        IDecisionRepository decisionRepo,
        IJournalEntryRepository journalRepo,
        IPatternRepository patternRepo)
    {
        _goalRepo = goalRepo;
        _decisionRepo = decisionRepo;
        _journalRepo = journalRepo;
        _patternRepo = patternRepo;
    }

    public async Task<List<SearchResult>> SearchAsync(Guid userId, string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return new List<SearchResult>();

        var results = new List<SearchResult>();

        var goals = await _goalRepo.SearchAsync(userId, query);
        results.AddRange(goals.Select(g => new SearchResult("Goal", g.Id, g.Title,
            g.Description ?? g.NextAction ?? "", g.UpdatedAt)));

        var decisions = await _decisionRepo.SearchAsync(userId, query);
        results.AddRange(decisions.Select(d => new SearchResult("Decision", d.Id, d.Title,
            d.Context, d.UpdatedAt)));

        var journal = await _journalRepo.SearchAsync(userId, query);
        results.AddRange(journal.Select(j => new SearchResult("Journal", j.Id,
            j.Title ?? "(no title)",
            j.Content.Length > 80 ? j.Content[..80] + "…" : j.Content,
            j.CreatedAt)));

        var patterns = await _patternRepo.SearchAsync(userId, query);
        results.AddRange(patterns.Select(p => new SearchResult("Pattern", p.Id, p.Name,
            p.Description ?? "", p.UpdatedAt)));

        return results.OrderByDescending(r => r.Date).ToList();
    }
}
