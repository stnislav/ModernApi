namespace ModernApi.Abstractions.Caching;

public interface IListVersionStore
{
    Task<long> GetAsync(CancellationToken ct = default);
    Task<long> BumpAsync(CancellationToken ct = default);
}
