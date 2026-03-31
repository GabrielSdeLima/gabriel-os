using GabrielOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GabrielOS.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Pillar> Pillars => Set<Pillar>();
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<Initiative> Initiatives => Set<Initiative>();
    public DbSet<Decision> Decisions => Set<Decision>();
    public DbSet<CheckIn> CheckIns => Set<CheckIn>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<WeeklyReview> WeeklyReviews => Set<WeeklyReview>();
    public DbSet<CycleFocus> CycleFocuses => Set<CycleFocus>();
    public DbSet<CycleFocusGoal> CycleFocusGoals => Set<CycleFocusGoal>();
    public DbSet<Pattern> Patterns => Set<Pattern>();
    public DbSet<Metric> Metrics => Set<Metric>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<MonthlyReview> MonthlyReviews => Set<MonthlyReview>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
