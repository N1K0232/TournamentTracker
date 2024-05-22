using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentTracker.DataAccessLayer.Configurations.Common;
using TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.DataAccessLayer.Configurations;

public class PrizeConfiguration : BaseEntityConfiguration<Prize>
{
    public override void Configure(EntityTypeBuilder<Prize> builder)
    {
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Value).HasPrecision(8, 2).IsRequired();

        builder.HasOne(p => p.Tournament)
            .WithMany(t => t.Prizes)
            .HasForeignKey(p => p.TournamentId)
            .IsRequired();

        builder.HasIndex(p => new { p.TournamentId, p.Name })
            .HasDatabaseName("IX_TournamentPrize")
            .IsUnique();

        builder.ToTable("Prizes");
        base.Configure(builder);
    }
}