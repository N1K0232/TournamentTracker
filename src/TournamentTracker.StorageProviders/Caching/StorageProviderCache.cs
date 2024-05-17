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

    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        var exists = cache.TryGetValue<Stream>(path, out _);
        return Task.FromResult(exists);
    }

    public Task<Stream?> ReadAsync(string path, CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue<Stream>(path, out var stream))
        {
            return Task.FromResult<Stream?>(stream);
        }

        return Task.FromResult<Stream?>(null);
    }

    public Task SetAsync(string path, Stream stream, CancellationToken cancellationToken = default)
    {
        cache.Set(path, stream, TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }
}