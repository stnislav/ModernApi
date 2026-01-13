using StackExchange.Redis;
using ModernApi.Abstractions.Caching;

namespace ModernApi.Infrastructure.Caching;

public sealed class RedisListVersionStore : IListVersionStore
{
    private readonly IDatabase _db;
    private const string VersionKey = "modernapi:items:list:ver";

    public RedisListVersionStore(IConnectionMultiplexer mux)
        => _db = mux.GetDatabase();

    public async Task<long> GetAsync(CancellationToken ct = default)
    {
        // StackExchange.Redis doesn't use CancellationToken directly; that's OK.
        RedisValue v = await _db.StringGetAsync(VersionKey);
        return v.HasValue ? (long)v : 1;
    }

    public async Task<long> BumpAsync(CancellationToken ct = default)
    {
        var v = await _db.StringIncrementAsync(VersionKey);
        // if it was missing, INCR returns 1 automatically
        return (long)v;
    }
}
