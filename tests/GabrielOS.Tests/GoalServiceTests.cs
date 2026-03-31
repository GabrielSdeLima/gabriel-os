using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Enums;
using GabrielOS.Domain.Interfaces;
using Moq;

namespace GabrielOS.Tests;

public class GoalServiceTests
{
    private readonly Mock<IGoalRepository> _goalRepoMock;
    private readonly GoalService _service;
    private readonly Guid _userId = Guid.NewGuid();

    public GoalServiceTests()
    {
        _goalRepoMock = new Mock<IGoalRepository>();
        _service = new GoalService(_goalRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_AllowsCreation_WhenFewer3ActiveP1Goals()
    {
        _goalRepoMock.Setup(r => r.CountActiveP1Async(_userId)).ReturnsAsync(2);
        _goalRepoMock.Setup(r => r.AddAsync(It.IsAny<Goal>())).ReturnsAsync((Goal g) => g);

        var goal = new Goal { UserId = _userId, Status = GoalStatus.Active, Priority = GoalPriority.P1, Title = "Test" };
        var (success, error) = await _service.CreateAsync(goal);

        Assert.True(success);
        Assert.Null(error);
        _goalRepoMock.Verify(r => r.AddAsync(It.IsAny<Goal>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Blocks_WhenAlready3ActiveP1Goals()
    {
        _goalRepoMock.Setup(r => r.CountActiveP1Async(_userId)).ReturnsAsync(3);

        var goal = new Goal { UserId = _userId, Status = GoalStatus.Active, Priority = GoalPriority.P1, Title = "Test" };
        var (success, error) = await _service.CreateAsync(goal);

        Assert.False(success);
        Assert.NotNull(error);
        _goalRepoMock.Verify(r => r.AddAsync(It.IsAny<Goal>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_SkipsP1Check_WhenNotP1()
    {
        _goalRepoMock.Setup(r => r.AddAsync(It.IsAny<Goal>())).ReturnsAsync((Goal g) => g);

        var goal = new Goal { UserId = _userId, Status = GoalStatus.Active, Priority = GoalPriority.P2, Title = "Test" };
        var (success, error) = await _service.CreateAsync(goal);

        Assert.True(success);
        _goalRepoMock.Verify(r => r.CountActiveP1Async(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_SetsCompletedAt_WhenStatusChangesToCompleted()
    {
        var existing = new Goal { Id = Guid.NewGuid(), UserId = _userId, Status = GoalStatus.Active, Priority = GoalPriority.P2 };
        _goalRepoMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _goalRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Goal>())).Returns(Task.CompletedTask);

        var updated = new Goal { Id = existing.Id, UserId = _userId, Status = GoalStatus.Completed, Priority = GoalPriority.P2 };
        await _service.UpdateAsync(updated);

        Assert.NotNull(updated.CompletedAt);
    }

    [Fact]
    public async Task UpdateAsync_BlocksP1Promotion_When3AlreadyActive()
    {
        var existing = new Goal { Id = Guid.NewGuid(), UserId = _userId, Status = GoalStatus.Active, Priority = GoalPriority.P2 };
        _goalRepoMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _goalRepoMock.Setup(r => r.CountActiveP1Async(_userId)).ReturnsAsync(3);

        var updated = new Goal { Id = existing.Id, UserId = _userId, Status = GoalStatus.Active, Priority = GoalPriority.P1 };
        var (success, error) = await _service.UpdateAsync(updated);

        Assert.False(success);
        Assert.NotNull(error);
        _goalRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Goal>()), Times.Never);
    }
}
