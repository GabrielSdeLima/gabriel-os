using GabrielOS.Domain.Enums;

namespace GabrielOS.Domain.Entities;

public class Goal : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid PillarId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? WhyItMatters { get; set; }
    public HorizonType HorizonType { get; set; } = HorizonType.Quarterly;
    public GoalStatus Status { get; set; } = GoalStatus.Idea;
    public GoalPriority Priority { get; set; } = GoalPriority.P3;
    public string? NextAction { get; set; }
    public string? SuccessCriteria { get; set; }
    public string? MainRisk { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? TargetDate { get; set; }
    public DateTime? ReviewDate { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Pillar Pillar { get; set; } = null!;
    public ICollection<Initiative> Initiatives { get; set; } = new List<Initiative>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
    public ICollection<CycleFocusGoal> CycleFocusGoals { get; set; } = new List<CycleFocusGoal>();
}
