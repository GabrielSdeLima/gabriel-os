using GabrielOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GabrielOS.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Timezone).IsRequired().HasMaxLength(100);
    }
}

public class PillarConfiguration : IEntityTypeConfiguration<Pillar>
{
    public void Configure(EntityTypeBuilder<Pillar> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Priority).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.Trend).HasConversion<string>().HasMaxLength(50);
        builder.HasOne(e => e.User).WithMany(u => u.Pillars).HasForeignKey(e => e.UserId);
        builder.HasIndex(e => new { e.UserId, e.SortOrder });
    }
}

public class GoalConfiguration : IEntityTypeConfiguration<Goal>
{
    public void Configure(EntityTypeBuilder<Goal> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(500);
        builder.Property(e => e.HorizonType).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.Priority).HasConversion<string>().HasMaxLength(10);
        builder.HasOne(e => e.User).WithMany(u => u.Goals).HasForeignKey(e => e.UserId);
        builder.HasOne(e => e.Pillar).WithMany(p => p.Goals).HasForeignKey(e => e.PillarId);
        builder.HasIndex(e => new { e.UserId, e.Status, e.Priority });
    }
}

public class InitiativeConfiguration : IEntityTypeConfiguration<Initiative>
{
    public void Configure(EntityTypeBuilder<Initiative> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasOne(e => e.Goal).WithMany(g => g.Initiatives).HasForeignKey(e => e.GoalId);
    }
}

public class DecisionConfiguration : IEntityTypeConfiguration<Decision>
{
    public void Configure(EntityTypeBuilder<Decision> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Context).IsRequired();
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasOne(e => e.User).WithMany(u => u.Decisions).HasForeignKey(e => e.UserId);
        builder.HasOne(e => e.Pillar).WithMany().HasForeignKey(e => e.PillarId).IsRequired(false);
        builder.HasOne(e => e.Goal).WithMany().HasForeignKey(e => e.GoalId).IsRequired(false);
    }
}

public class CheckInConfiguration : IEntityTypeConfiguration<CheckIn>
{
    public void Configure(EntityTypeBuilder<CheckIn> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.SuggestedMode).HasConversion<string>().HasMaxLength(50);
        builder.HasOne(e => e.User).WithMany(u => u.CheckIns).HasForeignKey(e => e.UserId);
        builder.HasIndex(e => new { e.UserId, e.Date }).IsUnique();
    }
}

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Content).IsRequired();
        builder.Property(e => e.EntryType).HasConversion<string>().HasMaxLength(50);
        builder.HasOne(e => e.User).WithMany(u => u.JournalEntries).HasForeignKey(e => e.UserId);
        builder.HasOne(e => e.Pillar).WithMany(p => p.JournalEntries).HasForeignKey(e => e.PillarId).IsRequired(false);
        builder.HasOne(e => e.Goal).WithMany(g => g.JournalEntries).HasForeignKey(e => e.GoalId).IsRequired(false);
        builder.HasOne(e => e.Decision).WithMany(d => d.JournalEntries).HasForeignKey(e => e.DecisionId).IsRequired(false);
        builder.HasIndex(e => new { e.UserId, e.CreatedAt });
    }
}

public class WeeklyReviewConfiguration : IEntityTypeConfiguration<WeeklyReview>
{
    public void Configure(EntityTypeBuilder<WeeklyReview> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.User).WithMany(u => u.WeeklyReviews).HasForeignKey(e => e.UserId);
        builder.HasIndex(e => new { e.UserId, e.WeekStart }).IsUnique();
    }
}

public class CycleFocusConfiguration : IEntityTypeConfiguration<CycleFocus>
{
    public void Configure(EntityTypeBuilder<CycleFocus> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(500);
        builder.HasOne(e => e.User).WithMany(u => u.CycleFocuses).HasForeignKey(e => e.UserId);
    }
}

public class CycleFocusGoalConfiguration : IEntityTypeConfiguration<CycleFocusGoal>
{
    public void Configure(EntityTypeBuilder<CycleFocusGoal> builder)
    {
        builder.HasKey(e => new { e.CycleFocusId, e.GoalId });
        builder.HasOne(e => e.CycleFocus).WithMany(cf => cf.CycleFocusGoals).HasForeignKey(e => e.CycleFocusId);
        builder.HasOne(e => e.Goal).WithMany(g => g.CycleFocusGoals).HasForeignKey(e => e.GoalId);
    }
}

public class PatternConfiguration : IEntityTypeConfiguration<Pattern>
{
    public void Configure(EntityTypeBuilder<Pattern> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasOne(e => e.User).WithMany(u => u.Patterns).HasForeignKey(e => e.UserId);
    }
}

public class MetricConfiguration : IEntityTypeConfiguration<Metric>
{
    public void Configure(EntityTypeBuilder<Metric> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Value).HasPrecision(18, 4);
        builder.HasOne(e => e.User).WithMany(u => u.Metrics).HasForeignKey(e => e.UserId);
        builder.HasOne(e => e.Pillar).WithMany(p => p.Metrics).HasForeignKey(e => e.PillarId);
        builder.HasIndex(e => new { e.UserId, e.PillarId, e.Date });
    }
}

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.Priority).HasConversion<string>().HasMaxLength(10);
        builder.HasOne(e => e.User).WithMany(u => u.Tasks).HasForeignKey(e => e.UserId);
        builder.HasOne(e => e.Goal).WithMany(g => g.Tasks).HasForeignKey(e => e.GoalId).IsRequired(false);
        builder.HasOne(e => e.Initiative).WithMany(i => i.Tasks).HasForeignKey(e => e.InitiativeId).IsRequired(false);
    }
}

public class MonthlyReviewConfiguration : IEntityTypeConfiguration<MonthlyReview>
{
    public void Configure(EntityTypeBuilder<MonthlyReview> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => new { e.UserId, e.Year, e.Month }).IsUnique();
        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
    }
}
