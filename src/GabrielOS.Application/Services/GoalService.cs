using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Enums;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Application.Services;

public class GoalService
{
    private readonly IGoalRepository _goalRepo;

    public GoalService(IGoalRepository goalRepo)
    {
        _goalRepo = goalRepo;
    }

    public Task<IReadOnlyList<Goal>> GetByUserAsync(Guid userId)
        => _goalRepo.GetByUserAsync(userId);

    public Task<Goal?> GetByIdAsync(Guid id)
        => _goalRepo.GetByIdAsync(id);

    public async Task<(bool Success, string? Error)> CreateAsync(Goal goal)
    {
        if (goal.Status == GoalStatus.Active && goal.Priority == GoalPriority.P1)
        {
            var p1Count = await _goalRepo.CountActiveP1Async(goal.UserId);
            if (p1Count >= 3)
                return (false, "You already have 3 P1 active goals. Demote or pause one first.");
        }
        goal.CreatedAt = goal.UpdatedAt = DateTime.UtcNow;
        await _goalRepo.AddAsync(goal);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(Goal goal)
    {
        var existing = await _goalRepo.GetByIdAsync(goal.Id);
        bool wasP1Active = existing?.Status == GoalStatus.Active && existing.Priority == GoalPriority.P1;
        bool isNowP1Active = goal.Status == GoalStatus.Active && goal.Priority == GoalPriority.P1;

        if (isNowP1Active && !wasP1Active)
        {
            var p1Count = await _goalRepo.CountActiveP1Async(goal.UserId);
            if (p1Count >= 3)
                return (false, "You already have 3 P1 active goals. Demote or pause one first.");
        }

        if (goal.Status == GoalStatus.Active && goal.Priority != GoalPriority.P1 && string.IsNullOrWhiteSpace(goal.NextAction))
        {
            // Warn but don't block — handled by UI flag
        }

        goal.UpdatedAt = DateTime.UtcNow;
        if (goal.Status == GoalStatus.Completed && !goal.CompletedAt.HasValue)
            goal.CompletedAt = DateTime.UtcNow;

        await _goalRepo.UpdateAsync(goal);
        return (true, null);
    }

    public Task DeleteAsync(Guid id)
        => _goalRepo.DeleteAsync(id);

    public Task<IReadOnlyList<Goal>> SearchAsync(Guid userId, string query)
        => _goalRepo.SearchAsync(userId, query);
}
