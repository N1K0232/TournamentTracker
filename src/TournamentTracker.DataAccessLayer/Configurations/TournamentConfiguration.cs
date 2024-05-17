using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentTracker.DataAccessLayer.Configurations.Common;
using TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.DataAccessLayer.Configurations;

public class TournamentConfiguration : DeletableEntityConfiguration<Tournament>
{
    public override void Configure(EntityTypeBuilder<Tournament> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
        builder.Property(t => t.EntryFee).HasPrecision(8, 2).IsRequired();
        builder.Property(t => t.StartDate).IsRequired();

        builder.HasIndex(t => new { t.Name, t.EntryFee })
            .HasDatabaseName("IX_Tournament")
            .IsUnique();

        builder.ToTable("Tournaments");
        base.Configure(builder);
    }
}