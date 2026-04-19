namespace OlimpBack.Infrastructure.Redis;

public interface IRbacCacheService
{
    Task<long?> GetLongAsync(string key, CancellationToken cancellationToken = default);
    Task SetLongAsync(string key, long value, TimeSpan ttl, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
