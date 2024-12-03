namespace TournamentTracker.StorageProviders.FileSystem;

public class FileSystemStorageOptions
{
    public string StorageFolder { get; set; }

    internal string CreatePath(string path)
    {
        return Path.Combine(StorageFolder, path);
    }
}