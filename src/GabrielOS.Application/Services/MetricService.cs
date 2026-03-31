using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Application.Services;

public class MetricService
{
    private readonly IMetricRepository _metricRepo;

    public MetricService(IMetricRepository metricRepo)
    {
        _metricRepo = metricRepo;
    }

    public Task<IReadOnlyList<Metric>> GetByUserAsync(Guid userId)
        => _metricRepo.GetByUserAsync(userId);

    public Task<IReadOnlyList<Metric>> GetByPillarAsync(Guid userId, Guid pillarId)
        => _metricRepo.GetByPillarAsync(userId, pillarId);

    public async Task<Metric> CreateAsync(Metric metric)
    {
        metric.CreatedAt = metric.UpdatedAt = DateTime.UtcNow;
        return await _metricRepo.AddAsync(metric);
    }

    public Task DeleteAsync(Guid id)
        => _metricRepo.DeleteAsync(id);
}
