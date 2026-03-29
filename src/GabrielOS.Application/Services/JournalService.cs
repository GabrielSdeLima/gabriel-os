using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Application.Services;

public class JournalService
{
    private readonly IJournalEntryRepository _journalRepo;

    public JournalService(IJournalEntryRepository journalRepo)
    {
        _journalRepo = journalRepo;
    }

    public Task<IReadOnlyList<JournalEntry>> GetEntriesAsync(Guid userId, int skip = 0, int take = 50)
        => _journalRepo.GetByUserAsync(userId, skip, take);

    public Task<JournalEntry?> GetByIdAsync(Guid id)
        => _journalRepo.GetByIdAsync(id);

    public async Task<JournalEntry> CreateAsync(JournalEntry entry)
    {
        entry.CreatedAt = DateTime.UtcNow;
        entry.UpdatedAt = DateTime.UtcNow;
        return await _journalRepo.AddAsync(entry);
    }

    public async Task UpdateAsync(JournalEntry entry)
    {
        entry.UpdatedAt = DateTime.UtcNow;
        await _journalRepo.UpdateAsync(entry);
    }

    public Task DeleteAsync(Guid id)
        => _journalRepo.DeleteAsync(id);

    public Task<IReadOnlyList<JournalEntry>> SearchAsync(Guid userId, string query)
        => _journalRepo.SearchAsync(userId, query);

    public Task<int> GetWeeklyCountAsync(Guid userId)
        => _journalRepo.GetCountThisWeekAsync(userId);
}
