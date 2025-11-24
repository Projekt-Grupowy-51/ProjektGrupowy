using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VidMark.Domain.Models;

namespace VidMark.Infrastructure.Persistance.Configurations;

public class SubjectVideoGroupAssignmentConfiguration : IEntityTypeConfiguration<SubjectVideoGroupAssignment>
{
    public void Configure(EntityTypeBuilder<SubjectVideoGroupAssignment> builder)
    {
        builder.HasKey(svga => svga.Id);
        builder.Ignore(b => b.DomainEvents);

        builder.Property(svga => svga.CreationDate)
            .IsRequired();

        builder.Property(svga => svga.CreatedById)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(svga => svga.Subject)
            .WithMany(s => s.SubjectVideoGroupAssignments)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(svga => svga.VideoGroup)
            .WithMany(vg => vg.SubjectVideoGroupAssignments)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(svga => svga.CreatedBy)
            .WithMany(u => u.OwnedAssignments)
            .HasForeignKey(svga => svga.CreatedById)
            .OnDelete(DeleteBehavior.SetNull);

        // Note: Labelers many-to-many relationship is configured in AppDbContext.OnModelCreating
        // to use the table name "labelers_assignments" for consistency with existing database

        builder.HasQueryFilter(svga => svga.DelDate == null);
    }
}
