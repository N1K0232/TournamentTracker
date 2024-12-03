namespace TournamentTracker.StorageProviders.FileSystem;

public class FileSystemStorageProvider : IStorageProvider
{
    private readonly FileSystemStorageOptions options;

    public FileSystemStorageProvider(FileSystemStorageOptions options)
    {
        this.options = options;
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = options.CreatePath(path);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = options.CreatePath(path);
        var exists = File.Exists(fullPath);

        return Task.FromResult(exists);
    }

    public Task<Stream> ReadAsStreamAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = options.CreatePath(path);
        if (!File.Exists(fullPath))
        {
            return Task.FromResult<Stream>(null);
        }

        var stream = File.OpenRead(fullPath);
        return Task.FromResult<Stream>(stream);
    }

    public async Task SaveAsync(Stream stream, string path, CancellationToken cancellationToken = default)
    {
        var fullPath = options.CreatePath(path);
        var directoryName = Path.GetDirectoryName(fullPath);

        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        using var fileStream = File.OpenWrite(fullPath);
        await stream.CopyToAsync(fileStream, cancellationToken);
    }
}