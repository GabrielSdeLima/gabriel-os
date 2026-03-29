using GabrielOS.Domain.Enums;

namespace GabrielOS.Domain.Entities;

public class Decision : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? PillarId { get; set; }
    public Guid? GoalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string? ProblemStatement { get; set; }
    public string? OptionsJson { get; set; }
    public string? ChosenOption { get; set; }
    public string? Rationale { get; set; }
    public string? TradeOffs { get; set; }
    public string? RisksAccepted { get; set; }
    public DecisionStatus Status { get; set; } = DecisionStatus.Active;
    public DateTime? ReviewDate { get; set; }
    public string? OutcomeNotes { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Pillar? Pillar { get; set; }
    public Goal? Goal { get; set; }
    public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
}
