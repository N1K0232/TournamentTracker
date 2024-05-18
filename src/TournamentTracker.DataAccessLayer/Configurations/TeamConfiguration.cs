using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentTracker.DataAccessLayer.Configurations.Common;
using TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.DataAccessLayer.Configurations;

public class TeamConfiguration : BaseEntityConfiguration<Team>
{
    public override void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();

        builder.HasOne(t => t.Tournament)
            .WithMany(t => t.Teams)
            .HasForeignKey(t => t.TournamentId)
            .IsRequired();

        builder.ToTable("Teams");
        base.Configure(builder);
    }
}