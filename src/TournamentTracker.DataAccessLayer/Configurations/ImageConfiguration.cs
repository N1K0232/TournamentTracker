using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentTracker.DataAccessLayer.Configurations.Common;
using TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.DataAccessLayer.Configurations;

public class ImageConfiguration : BaseEntityConfiguration<Image>
{
    public override void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.Property(i => i.Path).HasMaxLength(512).IsRequired();
        builder.Property(i => i.Length).IsRequired();
        builder.Property(i => i.ContentType).HasMaxLength(50).IsRequired();

        builder.HasIndex(i => i.Path)
            .HasDatabaseName("IX_UniquePath")
            .IsUnique();

        builder.ToTable("Images");
        base.Configure(builder);
    }
}