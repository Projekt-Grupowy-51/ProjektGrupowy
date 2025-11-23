using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Infrastructure.Persistance.Configurations;

public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Ignore(b => b.DomainEvents);

        builder.Property(v => v.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(v => v.Path)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(v => v.ContentType)
            .IsRequired();

        builder.Property(v => v.PositionInQueue)
            .IsRequired();

        builder.Property(v => v.VideoGroupId)
            .IsRequired();

        builder.Property(v => v.CreatedById)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(v => v.OriginalQuality)
            .IsRequired();

        builder.HasOne(v => v.VideoGroup)
            .WithMany(vg => vg.Videos)
            .HasForeignKey(v => v.VideoGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.CreatedBy)
            .WithMany()
            .HasForeignKey(v => v.CreatedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(v => v.AssignedLabels)
            .WithOne(al => al.Video)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(v => v.DelDate == null);
    }
}
