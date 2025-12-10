using TournamentTracker.DataAccessLayer.Entities.Common;

namespace TournamentTracker.DataAccessLayer.Entities;

public class Tournament : BaseEntity
{
    public string Name { get; set; } = null!;

    public decimal EntryFee { get; set; }

    public DateTimeOffset StartsAt { get; set; }

    public DateTimeOffset EndsAt { get; set; }
}