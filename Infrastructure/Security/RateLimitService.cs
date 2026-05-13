using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace OlimpBack.Infrastructure.Security;

public class RateLimitService : IRateLimitService
{
    private readonly IMemoryCache _cache;

    public RateLimitService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<bool> IsAllowedAsync(string key, int limit, TimeSpan window)
    {
        var cacheKey = $"ratelimit:{key}";
        
        var count = _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = window;
            return 0;
        });

        if (count >= limit)
        {
            return Task.FromResult(false);
        }

        _cache.Set(cacheKey, count + 1, window);
        return Task.FromResult(true);
    }
}
