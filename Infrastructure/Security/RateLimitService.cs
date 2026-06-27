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
        
        if (!_cache.TryGetValue(cacheKey, out RateLimitCounter? counter) || counter == null || counter.Expiry <= DateTime.UtcNow)
        {
            counter = new RateLimitCounter
            {
                Count = 0,
                Expiry = DateTime.UtcNow.Add(window)
            };
        }

        if (counter.Count >= limit)
        {
            return Task.FromResult(false);
        }

        counter.Count++;
        
        var remaining = counter.Expiry - DateTime.UtcNow;
        if (remaining > TimeSpan.Zero)
        {
            _cache.Set(cacheKey, counter, remaining);
        }
        
        return Task.FromResult(true);
    }

    private class RateLimitCounter
    {
        public int Count { get; set; }
        public DateTime Expiry { get; set; }
    }
}
