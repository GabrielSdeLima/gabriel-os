using GabrielOS.Domain.Enums;

namespace GabrielOS.Domain.Entities;

public class JournalEntry : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? PillarId { get; set; }
    public Guid? GoalId { get; set; }
    public Guid? DecisionId { get; set; }
    public EntryType EntryType { get; set; } = EntryType.Reflection;
    public string? Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? Mood { get; set; }
    public int? Energy { get; set; }
    public int? Intensity { get; set; }
    public string? TagsJson { get; set; }
    public bool IsSensitive { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Pillar? Pillar { get; set; }
    public Goal? Goal { get; set; }
    public Decision? Decision { get; set; }
}
