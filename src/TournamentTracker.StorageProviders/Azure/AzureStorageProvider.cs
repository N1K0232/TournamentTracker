using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MimeMapping;
using TournamentTracker.StorageProviders.Caching;

namespace TournamentTracker.StorageProviders.Azure;

public class AzureStorageProvider : IStorageProvider
{
    private readonly BlobServiceClient client;
    private readonly AzureStorageOptions options;
    private readonly IStorageProviderCache cache;

    public AzureStorageProvider(BlobServiceClient client, AzureStorageOptions options, IStorageProviderCache cache)
    {
        this.client = client;
        this.options = options;
        this.cache = cache;
    }

    public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        await client.DeleteBlobIfExistsAsync(path, cancellationToken: cancellationToken);
        await cache.DeleteAsync(path, cancellationToken);
    }

    public async Task<Stream?> ReadAsStreamAsync(string path, CancellationToken cancellationToken = default)
    {
        var cachedStream = await cache.ReadAsync(path, cancellationToken);
        if (cachedStream is not null)
        {
            return cachedStream;
        }

        var blobClient = await GetBlobClientAsync(path, cancellationToken: cancellationToken);
        var exists = await blobClient.ExistsAsync(cancellationToken);

        if (!exists)
        {
            return null;
        }

        var stream = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
        await cache.SetAsync(path, stream, cancellationToken);

        return stream;
    }

    public async Task SaveAsync(Stream stream, string path, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        var blobClient = await GetBlobClientAsync(path, true, cancellationToken);

        if (!overwrite)
        {
            var exists = await blobClient.ExistsAsync(cancellationToken);
            if (exists)
            {
                throw new IOException($"The file {path} already exists");
            }
        }

        stream.Position = 0;
        var headers = new BlobHttpHeaders
        {
            ContentType = MimeUtility.GetMimeMapping(path)
        };

        await blobClient.UploadAsync(stream, headers, cancellationToken: cancellationToken);
        await cache.SetAsync(path, stream, cancellationToken);
    }

    private async Task<BlobClient> GetBlobClientAsync(string path, bool createIfNotExists = false, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = client.GetBlobContainerClient(options.ContainerName);

        if (createIfNotExists)
        {
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
        }

        return blobContainerClient.GetBlobClient(path);
    }
}