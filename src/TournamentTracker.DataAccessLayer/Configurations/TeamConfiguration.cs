using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentTracker.DataAccessLayer.Configurations.Common;
using TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.DataAccessLayer.Configurations;

internal class TeamConfiguration : BaseEntityConfiguration<Team>
{
    public override void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(255).IsRequired();

        builder.HasOne(t => t.Tournament)
            .WithMany(t => t.Teams)
            .HasForeignKey(t => t.TournamentId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Teams_Tournaments")
            .IsRequired();

        builder.HasIndex(t => t.Name, "IX_Team_Name")
            .IsClustered(false)
            .IsUnique();

        builder.ToTable("Teams");
        base.Configure(builder);
    }
}