using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using ModernApi.Abstractions.Caching;

namespace ModernApi.Infrastructure.Caching;

public sealed class RedisAppCache : IAppCache
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IDistributedCache _distributedCache;

    public RedisAppCache(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var data = await _distributedCache.GetStringAsync(key, ct);
        return data is null ? default : JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };
        
        var data = JsonSerializer.Serialize(value);
        await _distributedCache.SetStringAsync(key, data, options, ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await _distributedCache.RemoveAsync(key, ct);
    }
}