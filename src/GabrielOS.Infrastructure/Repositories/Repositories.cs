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
            .Skip(skip)
            .Take(take)
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
}

public class DecisionRepository : Repository<Decision>, IDecisionRepository
{
    public DecisionRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Decision>> GetByUserAsync(Guid userId)
        => await _dbSet
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.CreatedAt)
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
            .FirstOrDefaultAsync(cf => cf.UserId == userId && cf.IsActive);
}
