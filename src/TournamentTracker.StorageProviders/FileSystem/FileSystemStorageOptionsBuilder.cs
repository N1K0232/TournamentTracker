namespace TournamentTracker.StorageProviders.FileSystem;

public class FileSystemStorageOptionsBuilder
{
    private string storageFolder = null!;

    public FileSystemStorageOptionsBuilder UseStorageFolder(string storageFolder)
    {
        this.storageFolder = storageFolder;
        return this;
    }

    public FileSystemStorageOptions Build() => new(storageFolder);
}