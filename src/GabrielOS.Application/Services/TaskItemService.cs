using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Application.Services;

public class TaskItemService
{
    private readonly ITaskItemRepository _taskRepo;

    public TaskItemService(ITaskItemRepository taskRepo)
    {
        _taskRepo = taskRepo;
    }

    public Task<IReadOnlyList<TaskItem>> GetByUserAsync(Guid userId)
        => _taskRepo.GetByUserAsync(userId);

    public Task<IReadOnlyList<TaskItem>> GetByGoalAsync(Guid goalId)
        => _taskRepo.GetByGoalAsync(goalId);

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        task.CreatedAt = task.UpdatedAt = DateTime.UtcNow;
        return await _taskRepo.AddAsync(task);
    }

    public async Task UpdateAsync(TaskItem task)
    {
        task.UpdatedAt = DateTime.UtcNow;
        await _taskRepo.UpdateAsync(task);
    }

    public Task DeleteAsync(Guid id)
        => _taskRepo.DeleteAsync(id);
}
