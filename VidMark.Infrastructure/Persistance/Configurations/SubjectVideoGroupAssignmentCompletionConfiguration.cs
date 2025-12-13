using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VidMark.Domain.Models;

namespace VidMark.Infrastructure.Persistance.Configurations;

public class SubjectVideoGroupAssignmentCompletionConfiguration : IEntityTypeConfiguration<SubjectVideoGroupAssignmentCompletion>
{
    public void Configure(EntityTypeBuilder<SubjectVideoGroupAssignmentCompletion> builder)
    {
        builder.ToTable("assignment_completions");

        builder.HasKey(ac => ac.Id);

        builder.Ignore(ac => ac.DomainEvents);

        builder.Property(ac => ac.SubjectVideoGroupAssignmentId)
            .IsRequired();

        builder.Property(ac => ac.LabelerId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ac => ac.IsCompleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ac => ac.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(ac => ac.Assignment)
            .WithMany()
            .HasForeignKey(ac => ac.SubjectVideoGroupAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ac => ac.Labeler)
            .WithMany()
            .HasForeignKey(ac => ac.LabelerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint - one completion record per assignment per labeler
        builder.HasIndex(ac => new { ac.SubjectVideoGroupAssignmentId, ac.LabelerId })
            .IsUnique();

        // Index for querying by labeler
        builder.HasIndex(ac => ac.LabelerId);
    }
}
