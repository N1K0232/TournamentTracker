using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentTracker.DataAccessLayer.Configurations.Common;
using TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.DataAccessLayer.Configurations;

internal class TournamentConfiguration : BaseEntityConfiguration<Tournament>
{
    public override void Configure(EntityTypeBuilder<Tournament> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(255).IsRequired();
        builder.Property(t => t.EntryFee).HasPrecision(6, 2);

        builder.HasIndex(t => t.Name, "IX_Tournaments_Name")
            .IsClustered(false)
            .IsUnique();

        builder.ToTable("Tournaments");
        base.Configure(builder);
    }
}