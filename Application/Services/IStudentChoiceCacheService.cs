using System;
using System.Threading.Tasks;

namespace OlimpBack.Application.Services;

public interface IStudentChoiceCacheService
{
    Task<int[]> GetLimitsAsync(Guid studentId);
    Task UpdateLimitAsync(Guid studentId, bool isSpring, int delta);
    Task ClearCacheAsync(Guid studentId);
    Task ClearCacheForStudentsInPeriodAsync(DisciplineChoicePeriod period);
}
