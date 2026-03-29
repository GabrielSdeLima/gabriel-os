using GabrielOS.Domain.Enums;

namespace GabrielOS.Domain.Entities;

public class TaskItem : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? GoalId { get; set; }
    public Guid? InitiativeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Backlog;
    public GoalPriority? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsNextAction { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Goal? Goal { get; set; }
    public Initiative? Initiative { get; set; }
}
