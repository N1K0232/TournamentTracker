using TournamentTracker.DataAccessLayer.Entities.Common;

namespace TournamentTracker.DataAccessLayer.Entities;

public class Tournament : DeletableEntity
{
    public string Name { get; set; }

    public decimal EntryFee { get; set; }

    public DateOnly StartDate { get; set; }
}