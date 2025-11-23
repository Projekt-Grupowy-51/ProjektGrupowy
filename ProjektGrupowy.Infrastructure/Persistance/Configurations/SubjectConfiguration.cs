using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Infrastructure.Persistance.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Ignore(b => b.DomainEvents);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(s => s.Description)
            .IsRequired();

        builder.Property(s => s.CreatedById)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(s => s.Project)
            .WithMany(p => p.Subjects)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.CreatedBy)
            .WithMany()
            .HasForeignKey(s => s.CreatedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(s => s.Labels)
            .WithOne(l => l.Subject)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.SubjectVideoGroupAssignments)
            .WithOne(svga => svga.Subject)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(s => s.DelDate == null);
    }
}
