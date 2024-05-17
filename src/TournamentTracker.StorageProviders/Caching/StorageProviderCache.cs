using Microsoft.Extensions.Caching.Memory;

namespace TournamentTracker.StorageProviders.Caching;

internal class StorageProviderCache : IStorageProviderCache
{
    private readonly IMemoryCache cache;

    public StorageProviderCache(IMemoryCache cache)
    {
        this.cache = cache;
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        cache.Remove(path);
        return Task.CompletedTask;
    }

    public Task<Stream?> ReadAsync(string path, CancellationToken cancellationToken = default)
    {
        var stream = cache.Get<Stream>(path);
        return Task.FromResult<Stream?>(stream);
    }

    public Task SetAsync(string path, Stream stream, CancellationToken cancellationToken = default)
    {
        cache.Set(path, stream);
        return Task.CompletedTask;
    }
}