using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;
using GabrielOS.Application.Rules;

namespace GabrielOS.Application.Services;

public class CheckInService
{
    private readonly ICheckInRepository _checkInRepo;

    public CheckInService(ICheckInRepository checkInRepo)
    {
        _checkInRepo = checkInRepo;
    }

    public Task<CheckIn?> GetTodayAsync(Guid userId)
        => _checkInRepo.GetByDateAsync(userId, DateTime.UtcNow.Date);

    public Task<CheckIn?> GetByDateAsync(Guid userId, DateTime date)
        => _checkInRepo.GetByDateAsync(userId, date.Date);

    public Task<IReadOnlyList<CheckIn>> GetRecentAsync(Guid userId, int count = 7)
        => _checkInRepo.GetRecentAsync(userId, count);

    public async Task<CheckIn> SaveAsync(CheckIn checkIn)
    {
        checkIn.SuggestedMode = ModeCalculator.Calculate(
            checkIn.Energy, checkIn.Mood, checkIn.Clarity, checkIn.Tension);

        return await _checkInRepo.UpsertAsync(checkIn);
    }
}
