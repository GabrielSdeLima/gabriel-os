namespace GabrielOS.Domain.Entities;

public class WeeklyReview : BaseEntity
{
    public Guid UserId { get; set; }
    public DateTime WeekStart { get; set; }
    public string? Wins { get; set; }
    public string? Frictions { get; set; }
    public string? AvoidedThings { get; set; }
    public string? EnergyDrains { get; set; }
    public string? EnergyGains { get; set; }
    public string? MainInsight { get; set; }
    public string? NextWeekFocus { get; set; }
    public string? PillarScoresJson { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}
