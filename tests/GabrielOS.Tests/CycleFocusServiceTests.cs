using GabrielOS.Application.Services;
using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;
using Moq;

namespace GabrielOS.Tests;

public class CycleFocusServiceTests
{
    private readonly Mock<ICycleFocusRepository> _cycleRepoMock;
    private readonly Mock<IGoalRepository> _goalRepoMock;
    private readonly CycleFocusService _service;
    private readonly Guid _userId = Guid.NewGuid();

    public CycleFocusServiceTests()
    {
        _cycleRepoMock = new Mock<ICycleFocusRepository>();
        _goalRepoMock = new Mock<IGoalRepository>();
        _service = new CycleFocusService(_cycleRepoMock.Object, _goalRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_Fails_WhenMoreThan3GoalsLinked()
    {
        var focus = new CycleFocus { UserId = _userId, Title = "Test", IsActive = true };
        var goalIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        var (success, error) = await _service.CreateAsync(focus, goalIds);

        Assert.False(success);
        Assert.NotNull(error);
        _cycleRepoMock.Verify(r => r.AddAsync(It.IsAny<CycleFocus>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_DeactivatesExistingCycle_WhenNewCycleIsActive()
    {
        var existingActive = new CycleFocus { Id = Guid.NewGuid(), UserId = _userId, IsActive = true };
        _cycleRepoMock.Setup(r => r.GetActiveAsync(_userId)).ReturnsAsync(existingActive);
        _cycleRepoMock.Setup(r => r.UpdateAsync(It.IsAny<CycleFocus>())).Returns(Task.CompletedTask);
        _cycleRepoMock.Setup(r => r.AddAsync(It.IsAny<CycleFocus>())).ReturnsAsync(new CycleFocus { Id = Guid.NewGuid() });

        var newFocus = new CycleFocus { UserId = _userId, Title = "New", IsActive = true };
        var (success, _) = await _service.CreateAsync(newFocus, new List<Guid>());

        Assert.True(success);
        _cycleRepoMock.Verify(r => r.UpdateAsync(It.Is<CycleFocus>(c => c.Id == existingActive.Id && !c.IsActive)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Succeeds_WithUpTo3Goals()
    {
        _cycleRepoMock.Setup(r => r.GetActiveAsync(_userId)).ReturnsAsync((CycleFocus?)null);
        _cycleRepoMock.Setup(r => r.AddAsync(It.IsAny<CycleFocus>())).ReturnsAsync(new CycleFocus { Id = Guid.NewGuid() });

        var focus = new CycleFocus { UserId = _userId, Title = "Test", IsActive = true };
        var goalIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        var (success, error) = await _service.CreateAsync(focus, goalIds);

        Assert.True(success);
        Assert.Null(error);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepository()
    {
        var id = Guid.NewGuid();
        _cycleRepoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

        await _service.DeleteAsync(id);

        _cycleRepoMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }
}
