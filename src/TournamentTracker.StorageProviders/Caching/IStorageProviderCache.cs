
namespace TournamentTracker.StorageProviders.Caching;

public interface IStorageProviderCache
{
    Task DeleteAsync(string path, CancellationToken cancellationToken = default);

    Task<Stream?> ReadAsync(string path, CancellationToken cancellationToken = default);

    Task SetAsync(string path, Stream stream, CancellationToken cancellationToken = default);
}