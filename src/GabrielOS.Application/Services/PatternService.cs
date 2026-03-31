using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Application.Services;

public class PatternService
{
    private readonly IPatternRepository _patternRepo;

    public PatternService(IPatternRepository patternRepo)
    {
        _patternRepo = patternRepo;
    }

    public Task<IReadOnlyList<Pattern>> GetByUserAsync(Guid userId)
        => _patternRepo.GetByUserAsync(userId);

    public async Task<Pattern> CreateAsync(Pattern pattern)
    {
        pattern.CreatedAt = pattern.UpdatedAt = DateTime.UtcNow;
        return await _patternRepo.AddAsync(pattern);
    }

    public async Task UpdateAsync(Pattern pattern)
    {
        pattern.UpdatedAt = DateTime.UtcNow;
        await _patternRepo.UpdateAsync(pattern);
    }

    public Task DeleteAsync(Guid id)
        => _patternRepo.DeleteAsync(id);
}
