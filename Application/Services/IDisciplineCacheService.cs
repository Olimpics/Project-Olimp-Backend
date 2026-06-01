using System;
using System.Threading.Tasks;

namespace OlimpBack.Application.Services;

public interface IDisciplineCacheService
{
    Task<int> GetOccupancyAsync(Guid disciplineId);
    Task SetOccupancyAsync(Guid disciplineId, int count);
    Task<int> IncrementOccupancyAsync(Guid disciplineId);
    Task ClearCacheAsync(Guid disciplineId);
}
