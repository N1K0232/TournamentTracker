namespace TournamentTracker.StorageProviders.Azure;

public class AzureStorageOptions
{
    internal AzureStorageOptions(string connectionString, string containerName)
    {
        ConnectionString = connectionString;
        ContainerName = containerName;
    }

    public string ConnectionString { get; }

    public string ContainerName { get; }
}