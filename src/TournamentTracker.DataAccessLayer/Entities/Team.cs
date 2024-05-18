using TournamentTracker.DataAccessLayer.Entities.Common;

namespace TournamentTracker.DataAccessLayer.Entities;

public class Team : BaseEntity
{
    public Guid TournamentId { get; set; }

    public string Name { get; set; }

    public virtual Tournament Tournament { get; set; }

    public virtual ICollection<Person> People { get; set; }
}