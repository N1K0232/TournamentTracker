using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TournamentTracker.StorageProviders.Azure;
using TournamentTracker.StorageProviders.FileSystem;

namespace TournamentTracker.StorageProviders.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureStorage(this IServiceCollection services, Action<AzureStorageOptions> configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        services.TryAddScoped(_ =>
        {
            var options = new AzureStorageOptions();
            configuration.Invoke(options);

            return options;
        });

        services.TryAddScoped(services =>
        {
            var options = services.GetRequiredService<AzureStorageOptions>();
            return new BlobServiceClient(options.ConnectionString);
        });

        services.AddScoped<IStorageProvider, AzureStorageProvider>();
        return services;
    }

    public static IServiceCollection AddFileSystemStorage(this IServiceCollection services, Action<FileSystemStorageOptions> configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        var options = new FileSystemStorageOptions();
        configuration.Invoke(options);

        services.TryAddSingleton(options);
        services.AddScoped<IStorageProvider, FileSystemStorageProvider>();

        return services;
    }
}