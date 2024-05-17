using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TournamentTracker.StorageProviders.Azure;
using TournamentTracker.StorageProviders.Caching;
using TournamentTracker.StorageProviders.FileSystem;

namespace TournamentTracker.StorageProviders.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureStorage(this IServiceCollection services, Action<AzureStorageOptionsBuilder> configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        var builder = new AzureStorageOptionsBuilder();
        configuration.Invoke(builder);

        var options = builder.Build();
        services.TryAddScoped(_ => options);

        services.TryAddScoped(_ => new BlobServiceClient(options.ConnectionString));
        services.AddStorageProvider<AzureStorageProvider>();

        return services;
    }

    public static IServiceCollection AddFileSystemStorage(this IServiceCollection services, Action<FileSystemStorageOptionsBuilder> configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        var builder = new FileSystemStorageOptionsBuilder();
        configuration.Invoke(builder);

        services.TryAddScoped(_ => builder.Build());
        services.TryAddScoped<FileSystemStorageClient>();
        services.AddStorageProvider<FileSystemStorageProvider>();

        return services;
    }

    private static void AddStorageProvider<TStorageProvider>(this IServiceCollection services)
        where TStorageProvider : class, IStorageProvider
    {
        services.TryAddSingleton<IStorageProviderCache, StorageProviderCache>();
        services.TryAddScoped<IStorageProvider, TStorageProvider>();
    }
}