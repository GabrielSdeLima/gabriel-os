using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Application.Services;

public class PillarService
{
    private readonly IPillarRepository _pillarRepo;

    public PillarService(IPillarRepository pillarRepo)
    {
        _pillarRepo = pillarRepo;
    }

    public Task<IReadOnlyList<Pillar>> GetAllOrderedAsync(Guid userId)
        => _pillarRepo.GetByUserOrderedAsync(userId);

    public Task<Pillar?> GetByIdAsync(Guid id)
        => _pillarRepo.GetByIdAsync(id);

    public async Task<Pillar> CreateAsync(Pillar pillar)
    {
        pillar.CreatedAt = DateTime.UtcNow;
        pillar.UpdatedAt = DateTime.UtcNow;
        return await _pillarRepo.AddAsync(pillar);
    }

    public async Task UpdateAsync(Pillar pillar)
    {
        pillar.UpdatedAt = DateTime.UtcNow;
        await _pillarRepo.UpdateAsync(pillar);
    }

    public Task DeleteAsync(Guid id)
        => _pillarRepo.DeleteAsync(id);
}
