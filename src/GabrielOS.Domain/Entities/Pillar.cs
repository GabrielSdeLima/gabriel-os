using GabrielOS.Domain.Enums;

namespace GabrielOS.Domain.Entities;

public class Pillar : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Vision { get; set; }
    public string? CurrentState { get; set; }
    public int? Score { get; set; }
    public PillarPriority Priority { get; set; } = PillarPriority.Medium;
    public Trend Trend { get; set; } = Trend.Unknown;
    public int SortOrder { get; set; }
    public DateTime? LastReviewedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<Goal> Goals { get; set; } = new List<Goal>();
    public ICollection<Metric> Metrics { get; set; } = new List<Metric>();
    public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
}
