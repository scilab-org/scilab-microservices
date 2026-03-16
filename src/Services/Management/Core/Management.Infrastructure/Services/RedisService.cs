#region using

using System.Text.Json;
using Management.Application.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

#endregion

namespace Management.Infrastructure.Services;

public class RedisService : IRedisService
{
    #region Fields

    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisService> _logger;
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    #endregion

    #region Ctors

    public RedisService(IDistributedCache cache, ILogger<RedisService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    #endregion

    #region Implementations

    public async Task<T?> GetOrSetCacheAsync<T>(string cacheKey,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken ct = default)
    {
        // Try to get from cache first
        var cached = await GetAsync<T>(cacheKey, ct);
        if (cached is not null)
            return cached;

        // Cache miss — invoke factory
        var result = await factory(ct);

        // Store in cache
        await SetAsync(cacheKey, result, expiration ?? DefaultExpiration, ct);

        return result;
    }

    public async Task RemoveCacheAsync(string cacheKey, CancellationToken ct = default)
    {
        try
        {
            await _cache.RemoveAsync(cacheKey, ct);
            _logger.LogDebug("Cache removed: {CacheKey}", cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to remove cache key: {CacheKey}", cacheKey);
        }
    }

    #endregion

    #region Private Methods

    private async Task<T?> GetAsync<T>(string cacheKey, CancellationToken ct = default)
    {
        try
        {
            var cachedBytes = await _cache.GetAsync(cacheKey, ct);
            if (cachedBytes is null || cachedBytes.Length == 0)
                return default;

            var result = JsonSerializer.Deserialize<T>(cachedBytes);
            _logger.LogDebug("Cache hit: {CacheKey}", cacheKey);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get cache key: {CacheKey}. Falling through to source.", cacheKey);
            return default;
        }
    }

    private async Task SetAsync<T>(string cacheKey, T value, TimeSpan expiration, CancellationToken ct = default)
    {
        try
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            await _cache.SetAsync(cacheKey, bytes, options, ct);
            _logger.LogDebug("Cache set: {CacheKey}, TTL: {Expiration}", cacheKey, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to set cache key: {CacheKey}. Continuing without cache.", cacheKey);
        }
    }

    #endregion
}
