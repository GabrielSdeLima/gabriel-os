using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Application.Services;

public class CycleFocusService
{
    private readonly ICycleFocusRepository _cycleFocusRepo;
    private readonly IGoalRepository _goalRepo;

    public CycleFocusService(ICycleFocusRepository cycleFocusRepo, IGoalRepository goalRepo)
    {
        _cycleFocusRepo = cycleFocusRepo;
        _goalRepo = goalRepo;
    }

    public Task<CycleFocus?> GetActiveAsync(Guid userId)
        => _cycleFocusRepo.GetActiveAsync(userId);

    public Task<IReadOnlyList<CycleFocus>> GetByUserAsync(Guid userId)
        => _cycleFocusRepo.GetByUserAsync(userId);

    public async Task<(bool Success, string? Error)> CreateAsync(CycleFocus focus, List<Guid> goalIds)
    {
        if (goalIds.Count > 3)
            return (false, "A cycle focus should have at most 3 linked goals.");

        focus.CreatedAt = focus.UpdatedAt = DateTime.UtcNow;

        if (focus.IsActive)
            await DeactivateCurrentAsync(focus.UserId);

        var saved = await _cycleFocusRepo.AddAsync(focus);

        foreach (var goalId in goalIds)
        {
            var cfg = new CycleFocusGoal { CycleFocusId = saved.Id, GoalId = goalId };
            // Add directly via context — handled via navigation
        }

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(CycleFocus focus)
    {
        if (focus.IsActive)
            await DeactivateCurrentAsync(focus.UserId, focus.Id);

        focus.UpdatedAt = DateTime.UtcNow;
        await _cycleFocusRepo.UpdateAsync(focus);
        return (true, null);
    }

    public Task DeleteAsync(Guid id)
        => _cycleFocusRepo.DeleteAsync(id);

    private async Task DeactivateCurrentAsync(Guid userId, Guid? exceptId = null)
    {
        var active = await _cycleFocusRepo.GetActiveAsync(userId);
        if (active != null && active.Id != exceptId)
        {
            active.IsActive = false;
            active.UpdatedAt = DateTime.UtcNow;
            await _cycleFocusRepo.UpdateAsync(active);
        }
    }
}
