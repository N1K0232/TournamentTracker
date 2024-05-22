using TournamentTracker.Shared.Models.Collections;
using TournamentTracker.Shared.Models.Common;

namespace TournamentTracker.Shared.Models;

public class Tournament : BaseObject
{
    public string Name { get; set; } = null!;

    public decimal EntryFee { get; set; }

    public DateOnly StartDate { get; set; }

    public ListResult<Team>? EnteredTeams { get; set; }

    public IEnumerable<Prize>? Prizes { get; set; }
}