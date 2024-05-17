using TournamentTracker.DataAccessLayer.Entities.Common;

namespace TournamentTracker.DataAccessLayer.Entities;

public abstract class DeletableEntity : BaseEntity
{
    public bool IsDeleted { get; set; }

    public DateTime? DeletedDate { get; set; }
}