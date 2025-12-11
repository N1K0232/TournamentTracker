using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentTracker.DataAccessLayer.Configurations.Common;
using TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.DataAccessLayer.Configurations;

internal class PersonConfiguration : BaseEntityConfiguration<Person>
{
    public override void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.Property(p => p.FirstName).HasMaxLength(255).IsRequired();
        builder.Property(p => p.LastName).HasMaxLength(255).IsRequired();
        builder.Property(p => p.City).HasMaxLength(100).IsRequired();

        builder.Property(p => p.CellphoneNumber).HasMaxLength(100).IsRequired();
        builder.Property(p => p.EmailAddress).HasMaxLength(255).IsRequired();

        builder.HasOne(p => p.Team)
            .WithMany(t => t.People)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_People_Teams")
            .IsRequired();

        builder.HasIndex(p => p.CellphoneNumber, "IX_People_CellphoneNumber")
            .IsClustered(false)
            .IsUnique();

        builder.HasIndex(p => p.EmailAddress, "IX_People_EmailAddress")
            .IsClustered(false)
            .IsUnique();

        builder.ToTable("People");
        base.Configure(builder);
    }
}