using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;
using GabrielOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GabrielOS.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
        => await _dbSet.FindAsync(id);

    public async Task<IReadOnlyList<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }
    public async Task<User?> GetDefaultUserAsync()
        => await _dbSet.FirstOrDefaultAsync();
}

public class PillarRepository : Repository<Pillar>, IPillarRepository
{
    public PillarRepository(AppDbContext context) : base(context) { }
    public async Task<IReadOnlyList<Pillar>> GetByUserOrderedAsync(Guid userId)
        => await _dbSet
            .Where(p => p.UserId == userId)
            .OrderBy(p => p.SortOrder)
            .ToListAsync();
}

public class CheckInRepository : Repository<CheckIn>, ICheckInRepository
{
    public CheckInRepository(AppDbContext context) : base(context) { }

    public async Task<CheckIn?> GetByDateAsync(Guid userId, DateTime date)
        => await _dbSet.FirstOrDefaultAsync(c => c.UserId == userId && c.Date == date.Date);

    public async Task<IReadOnlyList<CheckIn>> GetRecentAsync(Guid userId, int count)
        => await _dbSet
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.Date)
            .Take(count)
            .ToListAsync();

    public async Task<CheckIn> UpsertAsync(CheckIn checkIn)
    {
        var existing = await GetByDateAsync(checkIn.UserId, checkIn.Date);
        if (existing != null)
        {
            existing.Energy = checkIn.Energy;
            existing.Mood = checkIn.Mood;
            existing.Clarity = checkIn.Clarity;
            existing.Tension = checkIn.Tension;
            existing.PhysicalState = checkIn.PhysicalState;
            existing.TopConcern = checkIn.TopConcern;
            existing.TopPriority = checkIn.TopPriority;
            existing.FreeText = checkIn.FreeText;
            existing.SuggestedMode = checkIn.SuggestedMode;
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existing;
        }
        checkIn.Date = checkIn.Date.Date;
        return await AddAsync(checkIn);
    }
}

public class JournalEntryRepository : Repository<JournalEntry>, IJournalEntryRepository
{
    public JournalEntryRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<JournalEntry>> GetByUserAsync(Guid userId, int skip = 0, int take = 50)
        => await _dbSet
            .Where(j => j.UserId == userId)
            .OrderByDescending(j => j.CreatedAt)
            .Skip(skip).Take(take)
            .ToListAsync();

    public async Task<IReadOnlyList<JournalEntry>> SearchAsync(Guid userId, string query)
        => await _dbSet
            .Where(j => j.UserId == userId &&
                (j.Content.Contains(query) || (j.Title != null && j.Title.Contains(query))))
            .OrderByDescending(j => j.CreatedAt)
            .Take(50)
            .ToListAsync();

    public async Task<int> GetCountThisWeekAsync(Guid userId)
    {
        var today = DateTime.UtcNow.Date;
        var daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
        var weekStart = today.AddDays(-daysFromMonday);
        return await _dbSet.CountAsync(j => j.UserId == userId && j.CreatedAt >= weekStart);
    }
}

public class GoalRepository : Repository<Goal>, IGoalRepository
{
    public GoalRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Goal>> GetByUserAsync(Guid userId)
        => await _dbSet
            .Where(g => g.UserId == userId)
            .Include(g => g.Pillar)
            .OrderBy(g => g.Priority)
            .ThenByDescending(g => g.UpdatedAt)
            .ToListAsync();

    public async Task<int> CountActiveP1Async(Guid userId)
        => await _dbSet.CountAsync(g =>
            g.UserId == userId &&
            g.Status == Domain.Enums.GoalStatus.Active &&
            g.Priority == Domain.Enums.GoalPriority.P1);

    public async Task<IReadOnlyList<Goal>> SearchAsync(Guid userId, string query)
        => await _dbSet
            .Where(g => g.UserId == userId &&
                (g.Title.Contains(query) ||
                 (g.Description != null && g.Description.Contains(query)) ||
                 (g.NextAction != null && g.NextAction.Contains(query))))
            .Include(g => g.Pillar)
            .OrderByDescending(g => g.UpdatedAt)
            .Take(20)
            .ToListAsync();
}

public class DecisionRepository : Repository<Decision>, IDecisionRepository
{
    public DecisionRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Decision>> GetByUserAsync(Guid userId)
        => await _dbSet
            .Where(d => d.UserId == userId)
            .Include(d => d.Pillar)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();

    public async Task<IReadOnlyList<Decision>> SearchAsync(Guid userId, string query)
        => await _dbSet
            .Where(d => d.UserId == userId &&
                (d.Title.Contains(query) ||
                 d.Context.Contains(query) ||
                 (d.ChosenOption != null && d.ChosenOption.Contains(query))))
            .Include(d => d.Pillar)
            .OrderByDescending(d => d.CreatedAt)
            .Take(20)
            .ToListAsync();
}

public class WeeklyReviewRepository : Repository<WeeklyReview>, IWeeklyReviewRepository
{
    public WeeklyReviewRepository(AppDbContext context) : base(context) { }

    public async Task<WeeklyReview?> GetByWeekAsync(Guid userId, DateTime weekStart)
        => await _dbSet.FirstOrDefaultAsync(r => r.UserId == userId && r.WeekStart == weekStart.Date);

    public async Task<IReadOnlyList<WeeklyReview>> GetRecentAsync(Guid userId, int count)
        => await _dbSet
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.WeekStart)
            .Take(count)
            .ToListAsync();
}

public class CycleFocusRepository : Repository<CycleFocus>, ICycleFocusRepository
{
    public CycleFocusRepository(AppDbContext context) : base(context) { }

    public async Task<CycleFocus?> GetActiveAsync(Guid userId)
        => await _dbSet
            .Include(cf => cf.CycleFocusGoals)
                .ThenInclude(cfg => cfg.Goal)
                    .ThenInclude(g => g.Pillar)
            .FirstOrDefaultAsync(cf => cf.UserId == userId && cf.IsActive);

    public async Task<IReadOnlyList<CycleFocus>> GetByUserAsync(Guid userId)
        => await _dbSet
            .Where(cf => cf.UserId == userId)
            .Include(cf => cf.CycleFocusGoals)
                .ThenInclude(cfg => cfg.Goal)
            .OrderByDescending(cf => cf.StartDate)
            .ToListAsync();
}

public class PatternRepository : Repository<Pattern>, IPatternRepository
{
    public PatternRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Pattern>> GetByUserAsync(Guid userId)
        => await _dbSet
            .Where(p => p.UserId == userId)
            .OrderBy(p => p.Status)
            .ThenByDescending(p => p.UpdatedAt)
            .ToListAsync();

    public async Task<IReadOnlyList<Pattern>> SearchAsync(Guid userId, string query)
        => await _dbSet
            .Where(p => p.UserId == userId &&
                (p.Name.Contains(query) || (p.Description != null && p.Description.Contains(query))))
            .ToListAsync();
}

public class MetricRepository : Repository<Metric>, IMetricRepository
{
    public MetricRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Metric>> GetByUserAsync(Guid userId)
        => await _dbSet
            .Where(m => m.UserId == userId)
            .Include(m => m.Pillar)
            .OrderByDescending(m => m.Date)
            .ToListAsync();

    public async Task<IReadOnlyList<Metric>> GetByPillarAsync(Guid userId, Guid pillarId)
        => await _dbSet
            .Where(m => m.UserId == userId && m.PillarId == pillarId)
            .OrderByDescending(m => m.Date)
            .ToListAsync();
}

public class TaskItemRepository : Repository<TaskItem>, ITaskItemRepository
{
    public TaskItemRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<TaskItem>> GetByUserAsync(Guid userId)
        => await _dbSet
            .Where(t => t.UserId == userId)
            .Include(t => t.Goal)
            .OrderBy(t => t.Status)
            .ThenBy(t => t.Priority)
            .ToListAsync();

    public async Task<IReadOnlyList<TaskItem>> GetByGoalAsync(Guid goalId)
        => await _dbSet
            .Where(t => t.GoalId == goalId)
            .OrderBy(t => t.Status)
            .ToListAsync();
}

public class MonthlyReviewRepository : Repository<MonthlyReview>, IMonthlyReviewRepository
{
    public MonthlyReviewRepository(AppDbContext context) : base(context) { }

    public async Task<MonthlyReview?> GetByMonthAsync(Guid userId, int year, int month)
        => await _dbSet.FirstOrDefaultAsync(r => r.UserId == userId && r.Year == year && r.Month == month);

    public async Task<IReadOnlyList<MonthlyReview>> GetRecentAsync(Guid userId, int count)
        => await _dbSet
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.Year).ThenByDescending(r => r.Month)
            .Take(count)
            .ToListAsync();
}

public class CalendarEventRepository : Repository<CalendarEvent>, ICalendarEventRepository
{
    public CalendarEventRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<CalendarEvent>> GetByUserAsync(Guid userId)
    {
        var list = await _dbSet
            .Where(e => e.UserId == userId)
            .Include(e => e.Goal)
            .Include(e => e.Pillar)
            .OrderBy(e => e.Date)
            .ToListAsync();
        return list.OrderBy(e => e.Date).ThenBy(e => e.StartTime).ToList();
    }

    public async Task<IReadOnlyList<CalendarEvent>> GetByDateRangeAsync(Guid userId, DateTime start, DateTime end)
    {
        var list = await _dbSet
            .Where(e => e.UserId == userId && e.Date >= start.Date && e.Date <= end.Date)
            .Include(e => e.Goal)
            .Include(e => e.Pillar)
            .OrderBy(e => e.Date)
            .ToListAsync();
        return list.OrderBy(e => e.Date).ThenBy(e => e.StartTime).ToList();
    }

    public async Task<IReadOnlyList<CalendarEvent>> GetUpcomingAsync(Guid userId, int days = 7)
    {
        var today = DateTime.UtcNow.Date;
        var end = today.AddDays(days);
        var list = await _dbSet
            .Where(e => e.UserId == userId && !e.IsCompleted && e.Date >= today && e.Date <= end)
            .Include(e => e.Goal)
            .Include(e => e.Pillar)
            .OrderBy(e => e.Date)
            .ToListAsync();
        return list.OrderBy(e => e.Date).ThenBy(e => e.StartTime).ToList();
    }
}
