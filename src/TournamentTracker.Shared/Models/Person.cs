using TournamentTracker.Shared.Models.Common;

namespace TournamentTracker.Shared.Models;

public class Person : BaseObject
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string CellphoneNumber { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;
}