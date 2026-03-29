using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Enums;

namespace GabrielOS.Application.Rules;

public record Alert(string Code, string Title, string Message, AlertSeverity Severity);

public enum AlertSeverity { Low, Medium, High }

public static class AlertRules
{
    public static List<Alert> Evaluate(
        IReadOnlyList<Goal> activeGoals,
        IReadOnlyList<Pillar> pillars,
        IReadOnlyList<CheckIn> recentCheckIns,
        WeeklyReview? lastReview)
    {
        var alerts = new List<Alert>();

        // A1: Too many P1 fronts
        var p1Count = activeGoals.Count(g => g.Priority == GoalPriority.P1 && g.Status == GoalStatus.Active);
        if (p1Count > 3)
        {
            alerts.Add(new Alert("A1", "Too many P1 fronts",
                $"You have {p1Count} P1 active goals. Consider demoting or pausing some.", AlertSeverity.High));
        }

        // A2: Goals without next action
        var noAction = activeGoals.Where(g => g.Status == GoalStatus.Active && string.IsNullOrWhiteSpace(g.NextAction)).ToList();
        foreach (var goal in noAction)
        {
            alerts.Add(new Alert("A2", "Goal without next action",
                $"\"{goal.Title}\" is active but has no next action defined.", AlertSeverity.Medium));
        }

        // A3: Stale goals (no update in 21+ days)
        var staleThreshold = DateTime.UtcNow.AddDays(-21);
        var stale = activeGoals.Where(g => g.Status == GoalStatus.Active && g.UpdatedAt < staleThreshold).ToList();
        foreach (var goal in stale)
        {
            alerts.Add(new Alert("A3", "Stale goal",
                $"\"{goal.Title}\" hasn't been updated in {(DateTime.UtcNow - goal.UpdatedAt).Days} days.", AlertSeverity.Medium));
        }

        // A4: Neglected pillars (not reviewed in 30+ days)
        var pillarThreshold = DateTime.UtcNow.AddDays(-30);
        var neglected = pillars.Where(p => p.LastReviewedAt == null || p.LastReviewedAt < pillarThreshold).ToList();
        foreach (var pillar in neglected)
        {
            alerts.Add(new Alert("A4", "Neglected pillar",
                $"\"{pillar.Name}\" hasn't been reviewed in over 30 days.", AlertSeverity.Medium));
        }

        // A5: Low energy streak (3+ consecutive check-ins with energy ≤ 4)
        if (recentCheckIns.Count >= 3)
        {
            var lastThree = recentCheckIns.OrderByDescending(c => c.Date).Take(3).ToList();
            if (lastThree.All(c => c.Energy <= 4))
            {
                alerts.Add(new Alert("A5", "Low energy streak",
                    "Your energy has been low for 3+ days. Consider reducing demands and prioritizing recovery.", AlertSeverity.High));
            }
        }

        // A6: Missing weekly review
        if (lastReview == null)
        {
            alerts.Add(new Alert("A6", "No weekly review yet",
                "You haven't completed a weekly review. Consider doing one to consolidate direction.", AlertSeverity.Low));
        }
        else
        {
            var daysSinceReview = (DateTime.UtcNow - lastReview.CreatedAt).Days;
            if (daysSinceReview > 9)
            {
                alerts.Add(new Alert("A6", "Weekly review overdue",
                    $"Last weekly review was {daysSinceReview} days ago.", AlertSeverity.Low));
            }
        }

        // A7: Decisions needing review
        // (handled separately when decisions are loaded)

        return alerts;
    }
}
