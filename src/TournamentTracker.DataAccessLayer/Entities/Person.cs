using TournamentTracker.DataAccessLayer.Entities.Common;

namespace TournamentTracker.DataAccessLayer.Entities;

public class Person : BaseEntity
{
    public Guid TeamId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string City { get; set; } = null!;

    public string CellphoneNumber { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;

    public virtual Team Team { get; set; } = null!;
}