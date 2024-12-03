namespace TournamentTracker.StorageProviders;

public interface IStorageProvider
{
    Task DeleteAsync(string path, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);

    Task<Stream> ReadAsStreamAsync(string path, CancellationToken cancellationToken = default);

    async Task<string> ReadAsStringAsync(string path, CancellationToken cancellationToken = default)
    {
        using var stream = await ReadAsStreamAsync(path, cancellationToken);
        if (stream is null)
        {
            return null;
        }

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    async Task<byte[]> ReadAsByteArrayAsync(string path, CancellationToken cancellationToken = default)
    {
        using var stream = await ReadAsStreamAsync(path, cancellationToken);
        if (stream is null)
        {
            return null;
        }

        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);

        return memoryStream.ToArray();
    }

    Task SaveAsync(Stream stream, string path, CancellationToken cancellationToken = default);

    async Task SaveAsync(byte[] content, string path, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(content);
        await SaveAsync(stream, path, cancellationToken);
    }
}