using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Application.Services;

public class MonthlyReviewService
{
    private readonly IMonthlyReviewRepository _repo;

    public MonthlyReviewService(IMonthlyReviewRepository repo)
    {
        _repo = repo;
    }

    public async Task<MonthlyReview?> GetByMonthAsync(Guid userId, int year, int month)
        => await _repo.GetByMonthAsync(userId, year, month);

    public async Task<IReadOnlyList<MonthlyReview>> GetRecentAsync(Guid userId, int count = 6)
        => await _repo.GetRecentAsync(userId, count);

    public async Task<MonthlyReview> SaveAsync(MonthlyReview review)
    {
        var existing = await _repo.GetByMonthAsync(review.UserId, review.Year, review.Month);
        if (existing != null)
        {
            existing.Highlights = review.Highlights;
            existing.Lowlights = review.Lowlights;
            existing.KeyLearnings = review.KeyLearnings;
            existing.NextMonthIntentions = review.NextMonthIntentions;
            existing.PillarScoresJson = review.PillarScoresJson;
            if (review.AISummary != null) existing.AISummary = review.AISummary;
            existing.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(existing);
            return existing;
        }
        review.CreatedAt = review.UpdatedAt = DateTime.UtcNow;
        return await _repo.AddAsync(review);
    }

    public async Task UpdateAISummaryAsync(Guid userId, int year, int month, string summary)
    {
        var review = await _repo.GetByMonthAsync(userId, year, month);
        if (review == null) return;
        review.AISummary = summary;
        review.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(review);
    }
}
