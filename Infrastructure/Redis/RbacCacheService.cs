using StackExchange.Redis;

namespace OlimpBack.Infrastructure.Redis;

public class RbacCacheService : IRbacCacheService
{
    private readonly IDatabase _database;

    public RbacCacheService(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task<long?> GetLongAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await _database.StringGetAsync(key);
        if (!value.HasValue)
            return null;

        return long.TryParse(value.ToString(), out var parsed) ? parsed : null;
    }

    public async Task SetLongAsync(string key, long value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        await _database.StringSetAsync(key, value.ToString(), ttl);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _database.KeyDeleteAsync(key);
    }
}
