using System.Text.Json;
using CommerceOS.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace CommerceOS.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public RedisCacheService(
        IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        var cachedValue = await _cache.GetStringAsync(
            key,
            cancellationToken);

        if (string.IsNullOrWhiteSpace(cachedValue))
            return default;

        return JsonSerializer.Deserialize<T>(
            cachedValue,
            JsonOptions);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        var serializedValue =
            JsonSerializer.Serialize(
                value,
                JsonOptions);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        await _cache.SetStringAsync(
            key,
            serializedValue,
            options,
            cancellationToken);
    }

    public async Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(
            key,
            cancellationToken);
    }
}