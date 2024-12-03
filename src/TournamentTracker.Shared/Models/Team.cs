using TournamentTracker.Shared.Models.Common;

namespace TournamentTracker.Shared.Models;

public class Team : BaseObject
{
    public string Name { get; set; } = null!;

    public IEnumerable<Person> Members { get; set; }
}