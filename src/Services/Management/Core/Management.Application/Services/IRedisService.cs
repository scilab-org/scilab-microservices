namespace Management.Application.Services;

public interface IRedisService
{
    #region Methods

    Task<T?> GetOrSetCacheAsync<T>(string cacheKey,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken ct = default);

    Task RemoveCacheAsync(string cacheKey, CancellationToken ct = default);

    #endregion
}
