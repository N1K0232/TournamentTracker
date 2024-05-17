using TournamentTracker.Shared.Models.Common;

namespace TournamentTracker.Shared.Models;

public class Image : BaseObject
{
    public string Path { get; set; } = null!;

    public long Length { get; set; }

    public string ContentType { get; set; } = null!;
}