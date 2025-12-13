using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VidMark.Domain.Models;

namespace VidMark.Infrastructure.Persistance.Configurations;

public class ProjectAccessCodeConfiguration : IEntityTypeConfiguration<ProjectAccessCode>
{
    public void Configure(EntityTypeBuilder<ProjectAccessCode> builder)
    {
        builder.HasKey(pac => pac.Id);
        builder.Ignore(b => b.DomainEvents);

        builder.Property(pac => pac.Code)
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(pac => pac.CreatedAtUtc)
            .IsRequired();

        builder.Property(pac => pac.ExpiresAtUtc)
            .IsRequired();

        builder.Property(pac => pac.CreatedById)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(pac => pac.Project)
            .WithMany(p => p.AccessCodes)
            .HasForeignKey(pac => pac.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pac => pac.CreatedBy)
            .WithMany()
            .HasForeignKey(pac => pac.CreatedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(pac => pac.Code)
            .IsUnique();

        builder.Ignore(pac => pac.IsExpired);
        builder.Ignore(pac => pac.IsValid);

        builder.HasQueryFilter(pac => pac.DelDate == null);
    }
}
