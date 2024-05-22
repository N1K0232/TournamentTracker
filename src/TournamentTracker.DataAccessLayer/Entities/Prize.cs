using TournamentTracker.DataAccessLayer.Entities.Common;

namespace TournamentTracker.DataAccessLayer.Entities;

public class Prize : BaseEntity
{
    public Guid TournamentId { get; set; }

    public string Name { get; set; }

    public decimal Value { get; set; }

    public virtual Tournament Tournament { get; set; }
}