namespace GabrielOS.Domain.Entities;

public class MonthlyReview : BaseEntity
{
    public Guid UserId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string? Highlights { get; set; }
    public string? Lowlights { get; set; }
    public string? KeyLearnings { get; set; }
    public string? NextMonthIntentions { get; set; }
    public string? PillarScoresJson { get; set; }
    public string? AISummary { get; set; }

    // Navigation
    public User User { get; set; } = null!;

    public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
}
