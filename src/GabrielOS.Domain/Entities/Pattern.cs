using GabrielOS.Domain.Enums;

namespace GabrielOS.Domain.Entities;

public class Pattern : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Trigger { get; set; }
    public PatternStatus Status { get; set; } = PatternStatus.Emerging;

    // Navigation
    public User User { get; set; } = null!;
}
