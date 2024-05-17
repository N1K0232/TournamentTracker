namespace TournamentTracker.StorageProviders;

public interface IStorageProvider
{
    Task DeleteAsync(string path, CancellationToken cancellationToken = default);

    Task<Stream?> ReadAsStreamAsync(string path, CancellationToken cancellationToken = default);

    async Task<string?> ReadAsStringAsync(string path, CancellationToken cancellationToken = default)
    {
        var stream = await ReadAsStreamAsync(path, cancellationToken);
        if (stream is null)
        {
            return null;
        }

        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync(cancellationToken);

        await stream.DisposeAsync();
        return content;
    }

    async Task<byte[]?> ReadAsByteArrayAsync(string path, CancellationToken cancellationToken = default)
    {
        var stream = await ReadAsStreamAsync(path, cancellationToken);
        if (stream is null)
        {
            return null;
        }

        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);

        await stream.DisposeAsync();
        return memoryStream.ToArray();
    }

    Task SaveAsync(Stream stream, string path, bool overwrite = false, CancellationToken cancellationToken = default);

    async Task SaveAsync(byte[] content, string path, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        var stream = new MemoryStream(content);
        await SaveAsync(stream, path, overwrite, cancellationToken);
        await stream.DisposeAsync();
    }
}