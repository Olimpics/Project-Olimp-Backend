using System;
using System.Threading;
using System.Threading.Tasks;
using OlimpBack.Models;
using StackExchange.Redis;

namespace OlimpBack.Application.Services;

/// <summary>
/// Caches StudentId -> active PeriodId in Redis so discipline selection does not
/// re-resolve the period on every request. Only open/confirmed periods are stored.
/// </summary>
public class PeriodCacheService : IPeriodCacheService
{
    private readonly IDatabase _database;
    private readonly IPeriodResolverService _periodResolver;
    private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
    private const string CacheKeyPrefix = "active_period:";
    private static readonly TimeSpan Ttl = TimeSpan.FromHours(24);

    public PeriodCacheService(IConnectionMultiplexer redis, IPeriodResolverService periodResolver)
    {
        _database = redis.GetDatabase();
        _periodResolver = periodResolver;
    }

    public async Task<Guid?> GetActivePeriodIdAsync(Guid studentId)
    {
        string cacheKey = $"{CacheKeyPrefix}{studentId}";
        var cached = await _database.StringGetAsync(cacheKey);
        if (cached.HasValue && Guid.TryParse(cached!, out var cachedId))
            return cachedId;

        await _lock.WaitAsync();
        try
        {
            cached = await _database.StringGetAsync(cacheKey);
            if (cached.HasValue && Guid.TryParse(cached!, out cachedId))
                return cachedId;

            var resolution = await _periodResolver.ResolveActivePeriodAsync(studentId);

            // Do not cache conflicts or empty results — they must keep re-resolving
            // until the configuration is fixed or a period opens.
            if (resolution.HasConflict || resolution.Period == null)
                return null;

            var periodId = resolution.Period.IdDisciplineChoicePeriod;
            await _database.StringSetAsync(cacheKey, periodId.ToString(), Ttl);
            return periodId;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task InvalidateAsync(Guid studentId)
    {
        await _database.KeyDeleteAsync($"{CacheKeyPrefix}{studentId}");
    }

    public async Task InvalidateForPeriodAsync(DisciplineChoicePeriod period)
    {
        var students = await _periodResolver.GetStudentsForPeriodAsync(period);
        foreach (var student in students)
            await InvalidateAsync(student.IdStudent);
    }
}
