using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VidMark.Domain.Models;

namespace VidMark.Infrastructure.Persistance.Configurations;

public class VideoGroupConfiguration : IEntityTypeConfiguration<VideoGroup>
{
    public void Configure(EntityTypeBuilder<VideoGroup> builder)
    {
        builder.HasKey(vg => vg.Id);
        builder.Ignore(b => b.DomainEvents);

        builder.Property(vg => vg.Name)
            .IsRequired();

        builder.Property(vg => vg.Description)
            .IsRequired();

        builder.Property(vg => vg.CreatedById)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(vg => vg.Project)
            .WithMany(p => p.VideoGroups)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vg => vg.CreatedBy)
            .WithMany()
            .HasForeignKey(vg => vg.CreatedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(vg => vg.Videos)
            .WithOne(v => v.VideoGroup)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(vg => vg.SubjectVideoGroupAssignments)
            .WithOne(svga => svga.VideoGroup)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(vg => vg.DelDate == null);
    }
}
