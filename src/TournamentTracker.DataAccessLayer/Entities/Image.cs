using TournamentTracker.DataAccessLayer.Entities.Common;

namespace TournamentTracker.DataAccessLayer.Entities;

public class Image : BaseEntity
{
    public string Path { get; set; }

    public long Length { get; set; }

    public string ContentType { get; set; }
}