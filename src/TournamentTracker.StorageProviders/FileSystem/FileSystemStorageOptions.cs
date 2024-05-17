namespace TournamentTracker.StorageProviders.FileSystem;

public class FileSystemStorageOptions
{
    internal FileSystemStorageOptions(string storageFolder)
    {
        StorageFolder = storageFolder;
    }

    public string StorageFolder { get; }

    public string CreatePath(string path)
    {
        return Path.Combine(StorageFolder, path);
    }
}