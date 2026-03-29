using GabrielOS.Domain.Enums;

namespace GabrielOS.Domain.Entities;

public class CheckIn : BaseEntity
{
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public int Energy { get; set; }
    public int Mood { get; set; }
    public int Clarity { get; set; }
    public int Tension { get; set; }
    public string? PhysicalState { get; set; }
    public string? TopConcern { get; set; }
    public string? TopPriority { get; set; }
    public string? FreeText { get; set; }
    public SuggestedMode? SuggestedMode { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}
