using System;
using System.Threading.Tasks;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IPeriodCacheService
{
    /// <summary>
    /// Returns the active period id for a student, resolving and caching it on a miss.
    /// Returns null when no single active period applies (none or a configuration conflict).
    /// </summary>
    Task<Guid?> GetActivePeriodIdAsync(Guid studentId);

    /// <summary>Drops the cached active period for a single student.</summary>
    Task InvalidateAsync(Guid studentId);

    /// <summary>Drops cached active periods for every student covered by the period's filters.</summary>
    Task InvalidateForPeriodAsync(DisciplineChoicePeriod period);
}
