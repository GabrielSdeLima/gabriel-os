using GabrielOS.Domain.Entities;

namespace GabrielOS.Domain.Interfaces;

public interface IPillarRepository : IRepository<Pillar>
{
    Task<IReadOnlyList<Pillar>> GetByUserOrderedAsync(Guid userId);
}

public interface ICheckInRepository : IRepository<CheckIn>
{
    Task<CheckIn?> GetByDateAsync(Guid userId, DateTime date);
    Task<IReadOnlyList<CheckIn>> GetRecentAsync(Guid userId, int count);
    Task<CheckIn> UpsertAsync(CheckIn checkIn);
}

public interface IJournalEntryRepository : IRepository<JournalEntry>
{
    Task<IReadOnlyList<JournalEntry>> GetByUserAsync(Guid userId, int skip = 0, int take = 50);
    Task<IReadOnlyList<JournalEntry>> SearchAsync(Guid userId, string query);
    Task<int> GetCountThisWeekAsync(Guid userId);
}

public interface IGoalRepository : IRepository<Goal>
{
    Task<IReadOnlyList<Goal>> GetByUserAsync(Guid userId);
    Task<int> CountActiveP1Async(Guid userId);
    Task<IReadOnlyList<Goal>> SearchAsync(Guid userId, string query);
}

public interface IDecisionRepository : IRepository<Decision>
{
    Task<IReadOnlyList<Decision>> GetByUserAsync(Guid userId);
    Task<IReadOnlyList<Decision>> SearchAsync(Guid userId, string query);
}

public interface IWeeklyReviewRepository : IRepository<WeeklyReview>
{
    Task<WeeklyReview?> GetByWeekAsync(Guid userId, DateTime weekStart);
    Task<IReadOnlyList<WeeklyReview>> GetRecentAsync(Guid userId, int count);
}

public interface ICycleFocusRepository : IRepository<CycleFocus>
{
    Task<CycleFocus?> GetActiveAsync(Guid userId);
    Task<IReadOnlyList<CycleFocus>> GetByUserAsync(Guid userId);
}

public interface IPatternRepository : IRepository<Pattern>
{
    Task<IReadOnlyList<Pattern>> GetByUserAsync(Guid userId);
    Task<IReadOnlyList<Pattern>> SearchAsync(Guid userId, string query);
}

public interface IMetricRepository : IRepository<Metric>
{
    Task<IReadOnlyList<Metric>> GetByUserAsync(Guid userId);
    Task<IReadOnlyList<Metric>> GetByPillarAsync(Guid userId, Guid pillarId);
}

public interface ITaskItemRepository : IRepository<TaskItem>
{
    Task<IReadOnlyList<TaskItem>> GetByUserAsync(Guid userId);
    Task<IReadOnlyList<TaskItem>> GetByGoalAsync(Guid goalId);
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetDefaultUserAsync();
}

public interface IMonthlyReviewRepository : IRepository<MonthlyReview>
{
    Task<MonthlyReview?> GetByMonthAsync(Guid userId, int year, int month);
    Task<IReadOnlyList<MonthlyReview>> GetRecentAsync(Guid userId, int count);
}
