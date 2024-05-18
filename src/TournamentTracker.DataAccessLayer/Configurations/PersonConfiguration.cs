using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentTracker.DataAccessLayer.Configurations.Common;
using TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.DataAccessLayer.Configurations;

public class PersonConfiguration : BaseEntityConfiguration<Person>
{
    public override void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.Property(p => p.FirstName).HasMaxLength(256).IsRequired();
        builder.Property(p => p.LastName).HasMaxLength(256).IsRequired();

        builder.Property(p => p.CellphoneNumber).HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.Property(p => p.EmailAddress).HasMaxLength(512).IsUnicode(false).IsRequired();

        builder.HasIndex(p => p.CellphoneNumber)
            .HasDatabaseName("IX_CellphoneNumber")
            .IsUnique();

        builder.HasIndex(p => p.EmailAddress)
            .HasDatabaseName("IX_EmailAddress")
            .IsUnique();

        builder.HasIndex(p => new { p.TeamId, p.FirstName, p.LastName })
            .HasDatabaseName("IX_TeamMember")
            .IsUnique();

        builder.ToTable("People");
        base.Configure(builder);
    }
}