namespace GabrielOS.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Timezone { get; set; } = "America/Sao_Paulo";
    public string? CurrentPhase { get; set; }
    public string? PreferencesJson { get; set; }

    // Navigation
    public ICollection<Pillar> Pillars { get; set; } = new List<Pillar>();
    public ICollection<Goal> Goals { get; set; } = new List<Goal>();
    public ICollection<Decision> Decisions { get; set; } = new List<Decision>();
    public ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();
    public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
    public ICollection<WeeklyReview> WeeklyReviews { get; set; } = new List<WeeklyReview>();
    public ICollection<CycleFocus> CycleFocuses { get; set; } = new List<CycleFocus>();
    public ICollection<Pattern> Patterns { get; set; } = new List<Pattern>();
    public ICollection<Metric> Metrics { get; set; } = new List<Metric>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();
}
