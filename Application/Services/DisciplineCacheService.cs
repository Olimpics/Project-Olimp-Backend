using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using StackExchange.Redis;

namespace OlimpBack.Application.Services;

public class DisciplineCacheService : IDisciplineCacheService
{
    private readonly IDatabase _database;
    private readonly AppDbContext _context;
    private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
    private const string CacheKeyPrefix = "discipline_occupancy:";

    public DisciplineCacheService(IConnectionMultiplexer redis, AppDbContext context)
    {
        _database = redis.GetDatabase();
        _context = context;
    }

    public async Task<int> GetOccupancyAsync(Guid disciplineId)
    {
        string cacheKey = $"{CacheKeyPrefix}{disciplineId}";
        var cached = await _database.StringGetAsync(cacheKey);

        if (cached.HasValue)
        {
            return (int)cached;
        }

        await _lock.WaitAsync();
        try
        {
            cached = await _database.StringGetAsync(cacheKey);
            if (cached.HasValue)
            {
                return (int)cached;
            }

            int count = await _context.BindSelectiveDisciplines
                .CountAsync(b => b.SelectiveDisciplineId == disciplineId && b.InProcess == true);

            await _database.StringSetAsync(cacheKey, count, TimeSpan.FromHours(24));
            return count;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task SetOccupancyAsync(Guid disciplineId, int count)
    {
        string cacheKey = $"{CacheKeyPrefix}{disciplineId}";
        await _database.StringSetAsync(cacheKey, count, TimeSpan.FromHours(24));
    }

    public async Task<int> IncrementOccupancyAsync(Guid disciplineId)
    {
        string cacheKey = $"{CacheKeyPrefix}{disciplineId}";
        
        // Check if exists first to avoid initializing with 1 if it should be higher
        var exists = await _database.KeyExistsAsync(cacheKey);
        if (!exists)
        {
            await GetOccupancyAsync(disciplineId);
        }

        return (int)await _database.StringIncrementAsync(cacheKey);
    }

    public async Task ClearCacheAsync(Guid disciplineId)
    {
        string cacheKey = $"{CacheKeyPrefix}{disciplineId}";
        await _database.KeyDeleteAsync(cacheKey);
    }
}
