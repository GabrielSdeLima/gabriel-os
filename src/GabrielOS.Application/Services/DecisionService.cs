using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Application.Services;

public class DecisionService
{
    private readonly IDecisionRepository _decisionRepo;

    public DecisionService(IDecisionRepository decisionRepo)
    {
        _decisionRepo = decisionRepo;
    }

    public Task<IReadOnlyList<Decision>> GetByUserAsync(Guid userId)
        => _decisionRepo.GetByUserAsync(userId);

    public Task<Decision?> GetByIdAsync(Guid id)
        => _decisionRepo.GetByIdAsync(id);

    public async Task<Decision> CreateAsync(Decision decision)
    {
        decision.CreatedAt = decision.UpdatedAt = DateTime.UtcNow;
        return await _decisionRepo.AddAsync(decision);
    }

    public async Task UpdateAsync(Decision decision)
    {
        decision.UpdatedAt = DateTime.UtcNow;
        await _decisionRepo.UpdateAsync(decision);
    }

    public Task DeleteAsync(Guid id)
        => _decisionRepo.DeleteAsync(id);

    public Task<IReadOnlyList<Decision>> SearchAsync(Guid userId, string query)
        => _decisionRepo.SearchAsync(userId, query);
}
