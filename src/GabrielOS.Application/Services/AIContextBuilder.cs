using System.Text;
using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Enums;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Application.Services;

public class AIContextBuilder
{
    private readonly IUserRepository _userRepo;
    private readonly IPillarRepository _pillarRepo;
    private readonly ICheckInRepository _checkInRepo;
    private readonly IJournalEntryRepository _journalRepo;
    private readonly IGoalRepository _goalRepo;
    private readonly IWeeklyReviewRepository _reviewRepo;

    public AIContextBuilder(
        IUserRepository userRepo,
        IPillarRepository pillarRepo,
        ICheckInRepository checkInRepo,
        IJournalEntryRepository journalRepo,
        IGoalRepository goalRepo,
        IWeeklyReviewRepository reviewRepo)
    {
        _userRepo = userRepo;
        _pillarRepo = pillarRepo;
        _checkInRepo = checkInRepo;
        _journalRepo = journalRepo;
        _goalRepo = goalRepo;
        _reviewRepo = reviewRepo;
    }

    public async Task<string> BuildWeeklyReviewContextAsync(Guid userId, WeeklyReview review)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== WEEKLY REVIEW DATA ===");
        sb.AppendLine($"Week: {review.WeekStart:dd MMM yyyy}");

        var pillars = await _pillarRepo.GetByUserOrderedAsync(userId);
        if (pillars.Any())
        {
            sb.AppendLine("\n--- Pillars ---");
            foreach (var p in pillars)
                sb.AppendLine($"- {p.Name}: score {p.Score?.ToString() ?? "unset"}/10 ({p.Priority})");
        }

        var recentCheckIns = await _checkInRepo.GetRecentAsync(userId, 7);
        if (recentCheckIns.Any())
        {
            sb.AppendLine("\n--- Recent Check-ins (last 7 days) ---");
            foreach (var c in recentCheckIns)
                sb.AppendLine($"- {c.Date:dd MMM}: Energy={c.Energy}, Mood={c.Mood}, Clarity={c.Clarity}, Tension={c.Tension}" +
                    (c.TopPriority != null ? $", Priority: {c.TopPriority}" : "") +
                    (c.TopConcern != null ? $", Concern: {c.TopConcern}" : ""));
        }

        var goals = await _goalRepo.GetByUserAsync(userId);
        var activeGoals = goals.Where(g => g.Status == GoalStatus.Active).Take(5).ToList();
        if (activeGoals.Any())
        {
            sb.AppendLine("\n--- Active Goals ---");
            foreach (var g in activeGoals)
                sb.AppendLine($"- [{g.Priority}] {g.Title}" + (g.NextAction != null ? $" → {g.NextAction}" : ""));
        }

        if (!string.IsNullOrWhiteSpace(review.Wins))
            sb.AppendLine($"\n--- Wins ---\n{review.Wins}");
        if (!string.IsNullOrWhiteSpace(review.Frictions))
            sb.AppendLine($"\n--- Frictions ---\n{review.Frictions}");
        if (!string.IsNullOrWhiteSpace(review.AvoidedThings))
            sb.AppendLine($"\n--- Avoided ---\n{review.AvoidedThings}");
        if (!string.IsNullOrWhiteSpace(review.EnergyDrains))
            sb.AppendLine($"\n--- Energy Drains ---\n{review.EnergyDrains}");
        if (!string.IsNullOrWhiteSpace(review.EnergyGains))
            sb.AppendLine($"\n--- Energy Gains ---\n{review.EnergyGains}");
        if (!string.IsNullOrWhiteSpace(review.MainInsight))
            sb.AppendLine($"\n--- Main Insight ---\n{review.MainInsight}");
        if (!string.IsNullOrWhiteSpace(review.NextWeekFocus))
            sb.AppendLine($"\n--- Next Week Focus ---\n{review.NextWeekFocus}");

        return sb.ToString();
    }

    public async Task<string> BuildMonthlyContextAsync(Guid userId, int year, int month)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"=== MONTHLY REVIEW DATA: {new DateTime(year, month, 1):MMMM yyyy} ===");

        var pillars = await _pillarRepo.GetByUserOrderedAsync(userId);
        if (pillars.Any())
        {
            sb.AppendLine("\n--- Pillars ---");
            foreach (var p in pillars)
                sb.AppendLine($"- {p.Name}: {p.Score?.ToString() ?? "—"}/10");
        }

        var weekStart = new DateTime(year, month, 1);
        var weekEnd = weekStart.AddMonths(1);
        var reviews = await _reviewRepo.GetRecentAsync(userId, 6);
        var monthReviews = reviews.Where(r => r.WeekStart >= weekStart && r.WeekStart < weekEnd).ToList();
        if (monthReviews.Any())
        {
            sb.AppendLine("\n--- Weekly Reviews This Month ---");
            foreach (var r in monthReviews)
            {
                sb.AppendLine($"\nWeek of {r.WeekStart:dd MMM}:");
                if (!string.IsNullOrWhiteSpace(r.Wins)) sb.AppendLine($"  Wins: {r.Wins}");
                if (!string.IsNullOrWhiteSpace(r.Frictions)) sb.AppendLine($"  Frictions: {r.Frictions}");
                if (!string.IsNullOrWhiteSpace(r.MainInsight)) sb.AppendLine($"  Insight: {r.MainInsight}");
            }
        }

        var goals = await _goalRepo.GetByUserAsync(userId);
        var active = goals.Where(g => g.Status == GoalStatus.Active).Take(5).ToList();
        if (active.Any())
        {
            sb.AppendLine("\n--- Active Goals ---");
            foreach (var g in active)
                sb.AppendLine($"- [{g.Priority}] {g.Title}");
        }

        return sb.ToString();
    }

    public async Task<string> BuildDailyInsightContextAsync(Guid userId, CheckIn checkIn)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== DAILY CHECK-IN ===");
        sb.AppendLine($"Date: {checkIn.Date:dd MMM yyyy}");
        sb.AppendLine($"Energy: {checkIn.Energy}/10, Mood: {checkIn.Mood}/10, Clarity: {checkIn.Clarity}/10, Tension: {checkIn.Tension}/10");
        if (checkIn.TopConcern != null) sb.AppendLine($"Concern: {checkIn.TopConcern}");
        if (checkIn.TopPriority != null) sb.AppendLine($"Priority: {checkIn.TopPriority}");
        if (checkIn.FreeText != null) sb.AppendLine($"Notes: {checkIn.FreeText}");

        var goals = await _goalRepo.GetByUserAsync(userId);
        var p1Goals = goals.Where(g => g.Status == GoalStatus.Active && g.Priority == GoalPriority.P1).Take(3).ToList();
        if (p1Goals.Any())
        {
            sb.AppendLine("\n--- P1 Goals ---");
            foreach (var g in p1Goals)
                sb.AppendLine($"- {g.Title}" + (g.NextAction != null ? $" → {g.NextAction}" : ""));
        }

        return sb.ToString();
    }
}
