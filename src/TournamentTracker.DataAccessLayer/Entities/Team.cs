using TournamentTracker.DataAccessLayer.Entities.Common;

namespace TournamentTracker.DataAccessLayer.Entities;

public class Team : BaseEntity
{
    public Guid TournamentId { get; set; }

    public string Name { get; set; } = null!;

    public virtual Tournament Tournament { get; set; } = null!;

    public virtual ICollection<Person> People { get; set; } = [];
}