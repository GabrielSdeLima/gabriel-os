namespace GabrielOS.Domain.Entities;

public class CycleFocus : BaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Thesis { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<CycleFocusGoal> CycleFocusGoals { get; set; } = new List<CycleFocusGoal>();
}

public class CycleFocusGoal
{
    public Guid CycleFocusId { get; set; }
    public Guid GoalId { get; set; }

    // Navigation
    public CycleFocus CycleFocus { get; set; } = null!;
    public Goal Goal { get; set; } = null!;
}
