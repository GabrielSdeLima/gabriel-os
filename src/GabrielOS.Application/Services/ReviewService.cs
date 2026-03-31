using System.Text.Json;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Application.Services;

public class ReviewService
{
    private readonly IWeeklyReviewRepository _reviewRepo;
    private readonly IPillarRepository _pillarRepo;

    public ReviewService(IWeeklyReviewRepository reviewRepo, IPillarRepository pillarRepo)
    {
        _reviewRepo = reviewRepo;
        _pillarRepo = pillarRepo;
    }

    public Task<IReadOnlyList<WeeklyReview>> GetRecentAsync(Guid userId, int count = 10)
        => _reviewRepo.GetRecentAsync(userId, count);

    public Task<WeeklyReview?> GetByWeekAsync(Guid userId, DateTime weekStart)
        => _reviewRepo.GetByWeekAsync(userId, weekStart);

    public static DateTime GetCurrentWeekStart()
    {
        var today = DateTime.UtcNow.Date;
        var daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
        return today.AddDays(-daysFromMonday);
    }

    public async Task<WeeklyReview> SaveAsync(WeeklyReview review)
    {
        var existing = await _reviewRepo.GetByWeekAsync(review.UserId, review.WeekStart);
        if (existing != null)
        {
            existing.Wins = review.Wins;
            existing.Frictions = review.Frictions;
            existing.AvoidedThings = review.AvoidedThings;
            existing.EnergyDrains = review.EnergyDrains;
            existing.EnergyGains = review.EnergyGains;
            existing.MainInsight = review.MainInsight;
            existing.NextWeekFocus = review.NextWeekFocus;
            existing.PillarScoresJson = review.PillarScoresJson;
            existing.Notes = review.Notes;
            existing.UpdatedAt = DateTime.UtcNow;
            await _reviewRepo.UpdateAsync(existing);

            if (!string.IsNullOrWhiteSpace(review.PillarScoresJson))
                await UpdatePillarScoresAsync(review.UserId, review.PillarScoresJson);

            return existing;
        }

        review.CreatedAt = review.UpdatedAt = DateTime.UtcNow;
        var saved = await _reviewRepo.AddAsync(review);

        if (!string.IsNullOrWhiteSpace(review.PillarScoresJson))
            await UpdatePillarScoresAsync(review.UserId, review.PillarScoresJson);

        return saved;
    }

    private async Task UpdatePillarScoresAsync(Guid userId, string pillarScoresJson)
    {
        try
        {
            var scores = JsonSerializer.Deserialize<Dictionary<string, int>>(pillarScoresJson);
            if (scores == null) return;

            var pillars = await _pillarRepo.GetByUserOrderedAsync(userId);
            foreach (var pillar in pillars)
            {
                if (scores.TryGetValue(pillar.Id.ToString(), out var score))
                {
                    pillar.Score = score;
                    pillar.LastReviewedAt = DateTime.UtcNow;
                    pillar.UpdatedAt = DateTime.UtcNow;
                    await _pillarRepo.UpdateAsync(pillar);
                }
            }
        }
        catch { /* ignore malformed JSON */ }
    }
}
