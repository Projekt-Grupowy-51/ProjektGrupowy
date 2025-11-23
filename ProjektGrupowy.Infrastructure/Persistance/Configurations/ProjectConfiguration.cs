using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Infrastructure.Persistance.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Ignore(p => p.DomainEvents);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Description)
            .IsRequired();

        builder.Property(p => p.CreatedById)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(p => p.CreatedBy)
            .WithMany(u => u.OwnedProjects)
            .HasForeignKey(p => p.CreatedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(p => p.Subjects)
            .WithOne(s => s.Project)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.VideoGroups)
            .WithOne(vg => vg.Project)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.AccessCodes)
            .WithOne(ac => ac.Project)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.GeneratedReports)
            .WithOne(gr => gr.Project)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(p => p.DelDate == null);
    }
}
