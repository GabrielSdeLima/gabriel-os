using GabrielOS.Domain.Enums;

namespace GabrielOS.Domain.Entities;

public class Initiative : BaseEntity
{
    public Guid GoalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GoalStatus Status { get; set; } = GoalStatus.Idea;
    public string? NextAction { get; set; }

    // Navigation
    public Goal Goal { get; set; } = null!;
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
