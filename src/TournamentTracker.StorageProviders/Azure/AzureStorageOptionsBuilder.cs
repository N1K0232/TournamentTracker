namespace TournamentTracker.StorageProviders.Azure;

public class AzureStorageOptionsBuilder
{
    private string connectionString = null!;
    private string containerName = null!;

    public AzureStorageOptionsBuilder UseConnectionString(string connectionString)
    {
        this.connectionString = connectionString;
        return this;
    }

    public AzureStorageOptionsBuilder UseContainerName(string containerName)
    {
        this.containerName = containerName;
        return this;
    }

    public AzureStorageOptions Build() => new(connectionString, containerName);
}