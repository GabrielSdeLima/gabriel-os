using GabrielOS.Domain.Enums;

namespace GabrielOS.Domain.Entities;

public class CalendarEvent : BaseEntity
{
    /// <summary>Default event type suggestions (user can create custom types).</summary>
    public static readonly IReadOnlyList<string> DefaultTypes = new[]
    {
        "Prova", "Trabalho", "Compromisso", "Prazo", "Reunião", "Lembrete", "Pessoal"
    };

    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string EventType { get; set; } = "Compromisso";
    public DateTime Date { get; set; }
    public DateTime? EndDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public bool IsAllDay { get; set; } = true;
    public string? Location { get; set; }
    public GoalPriority? Priority { get; set; }
    public bool IsCompleted { get; set; }
    public string? Notes { get; set; }

    // Optional links to existing entities
    public Guid? GoalId { get; set; }
    public Guid? PillarId { get; set; }

    // Simple recurrence
    public CalendarRecurrence Recurrence { get; set; } = CalendarRecurrence.None;
    public DateTime? RecurrenceEndDate { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Goal? Goal { get; set; }
    public Pillar? Pillar { get; set; }
}
