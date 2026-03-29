namespace GabrielOS.Domain.Entities;

public class Metric : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid PillarId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? Unit { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Pillar Pillar { get; set; } = null!;
}
