using GabrielOS.Application.Rules;
using GabrielOS.Domain.Enums;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Application.Services;

public class AlertService
{
    private readonly IGoalRepository _goalRepo;
    private readonly IPillarRepository _pillarRepo;
    private readonly ICheckInRepository _checkInRepo;
    private readonly IWeeklyReviewRepository _reviewRepo;
    private readonly IDecisionRepository _decisionRepo;

    public AlertService(
        IGoalRepository goalRepo,
        IPillarRepository pillarRepo,
        ICheckInRepository checkInRepo,
        IWeeklyReviewRepository reviewRepo,
        IDecisionRepository decisionRepo)
    {
        _goalRepo = goalRepo;
        _pillarRepo = pillarRepo;
        _checkInRepo = checkInRepo;
        _reviewRepo = reviewRepo;
        _decisionRepo = decisionRepo;
    }

    public async Task<List<Alert>> GetAlertsAsync(Guid userId)
    {
        var goals = await _goalRepo.GetByUserAsync(userId);
        var pillars = await _pillarRepo.GetByUserOrderedAsync(userId);
        var recentCheckIns = await _checkInRepo.GetRecentAsync(userId, 7);
        var recentReviews = await _reviewRepo.GetRecentAsync(userId, 1);
        var lastReview = recentReviews.FirstOrDefault();

        var alerts = AlertRules.Evaluate(goals, pillars, recentCheckIns, lastReview);

        // A7: Decisions with overdue review date
        var decisions = await _decisionRepo.GetByUserAsync(userId);
        foreach (var d in decisions.Where(d =>
            d.ReviewDate.HasValue &&
            d.ReviewDate < DateTime.UtcNow &&
            d.Status == DecisionStatus.Active))
        {
            alerts.Add(new Alert("A7", "Decision needs review",
                $"\"{d.Title}\" review date has passed.", AlertSeverity.Medium));
        }

        return alerts.OrderByDescending(a => a.Severity).ToList();
    }
}
