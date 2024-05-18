using TournamentTracker.DataAccessLayer.Entities.Common;

namespace TournamentTracker.DataAccessLayer.Entities;

public class Person : BaseEntity
{
    public Guid TeamId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateOnly BirthDate { get; set; }

    public string CellphoneNumber { get; set; }

    public string EmailAddress { get; set; }

    public virtual Team Team { get; set; }
}