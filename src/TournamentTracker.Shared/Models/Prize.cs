using TournamentTracker.Shared.Models.Common;

namespace TournamentTracker.Shared.Models;

public class Prize : BaseObject
{
    public string Name { get; set; } = null!;

    public decimal Value { get; set; }
}