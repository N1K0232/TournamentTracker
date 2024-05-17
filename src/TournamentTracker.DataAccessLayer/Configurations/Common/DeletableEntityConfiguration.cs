using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.DataAccessLayer.Configurations.Common;

public abstract class DeletableEntityConfiguration<T> : BaseEntityConfiguration<T> where T : DeletableEntity
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(x => x.IsDeleted).ValueGeneratedOnAdd().HasDefaultValueSql("(0)");
        builder.Property(x => x.DeletedDate).IsRequired(false);

        base.Configure(builder);
    }
}