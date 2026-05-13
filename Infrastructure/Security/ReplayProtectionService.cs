using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace OlimpBack.Infrastructure.Security;

public class ReplayProtectionService : IReplayProtectionService
{
    private readonly IMemoryCache _cache;
    private const int MaxDriftSeconds = 300; // 5 minutes window

    public ReplayProtectionService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<bool> IsReplayAsync(Guid conversationId, string nonce, long timestamp)
    {
        // 1. Validate timestamp drift
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (Math.Abs(now - timestamp) > MaxDriftSeconds * 1000)
        {
            return Task.FromResult(true); // Too much drift, potential replay
        }

        // 2. Check nonce in cache
        var key = $"replay:{conversationId}:{nonce}";
        if (_cache.TryGetValue(key, out _))
        {
            return Task.FromResult(true); // Nonce already seen
        }

        // 3. Store nonce for the duration of the window
        _cache.Set(key, true, TimeSpan.FromSeconds(MaxDriftSeconds));
        
        return Task.FromResult(false);
    }
}
