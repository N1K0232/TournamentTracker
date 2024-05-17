using TournamentTracker.StorageProviders.Caching;

namespace TournamentTracker.StorageProviders.FileSystem;

public class FileSystemStorageProvider : IStorageProvider
{
    private readonly FileSystemStorageClient client;
    private readonly IStorageProviderCache cache;

    public FileSystemStorageProvider(FileSystemStorageClient client, IStorageProviderCache cache)
    {
        this.client = client;
        this.cache = cache;
    }

    public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        await client.DeleteAsync(path, cancellationToken);
        await cache.DeleteAsync(path, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        var cacheExists = await cache.ExistsAsync(path, cancellationToken);
        if (!cacheExists)
        {
            var exists = await client.ExistsAsync(path, cancellationToken);
            return exists;
        }

        return cacheExists;
    }

    public async Task<Stream?> ReadAsStreamAsync(string path, CancellationToken cancellationToken = default)
    {
        var cachedStream = await cache.ReadAsync(path, cancellationToken);
        if (cachedStream is not null)
        {
            return cachedStream;
        }

        var stream = await client.OpenReadAsync(path, cancellationToken);
        return stream;
    }

    public async Task SaveAsync(Stream stream, string path, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        if (!overwrite)
        {
            var exists = await client.ExistsAsync(path, cancellationToken);
            if (exists)
            {
                throw new IOException($"The file {path} already exists");
            }
        }

        await client.CreateDirectoryAsync(path, cancellationToken);
        await client.SaveAsync(stream, path, cancellationToken);
        await cache.SetAsync(path, stream, cancellationToken);
    }
}