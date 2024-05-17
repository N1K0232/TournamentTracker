namespace TournamentTracker.StorageProviders.FileSystem;

public class FileSystemStorageClient(FileSystemStorageOptions options)
{
    public Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = options.CreatePath(path);
        var directoryName = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrWhiteSpace(directoryName) && !Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        return Task.CompletedTask;
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

    public Task<Stream?> OpenReadAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = options.CreatePath(path);
        if (!File.Exists(fullPath))
        {
            return Task.FromResult<Stream?>(null);
        }

        var stream = File.OpenRead(fullPath);
        return Task.FromResult<Stream?>(stream);
    }

    public async Task SaveAsync(Stream stream, string path, CancellationToken cancellationToken = default)
    {
        var fullPath = options.CreatePath(path);
        var fileStream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write);

        await stream.CopyToAsync(fileStream, cancellationToken);
        await fileStream.FlushAsync(cancellationToken);

        await stream.DisposeAsync();
        await fileStream.DisposeAsync();
    }
}