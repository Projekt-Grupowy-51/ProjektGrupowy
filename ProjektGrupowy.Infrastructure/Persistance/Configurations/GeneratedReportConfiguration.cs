using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Infrastructure.Persistance.Configurations;

public class GeneratedReportConfiguration : IEntityTypeConfiguration<GeneratedReport>
{
    public void Configure(EntityTypeBuilder<GeneratedReport> builder)
    {
        builder.HasKey(gr => gr.Id);
        builder.Ignore(b => b.DomainEvents);

        builder.Property(gr => gr.Name)
            .IsRequired();

        builder.Property(gr => gr.Path)
            .IsRequired();

        builder.Property(gr => gr.CreatedAtUtc)
            .IsRequired();

        builder.Property(gr => gr.CreatedById)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(gr => gr.Project)
            .WithMany(p => p.GeneratedReports)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(gr => gr.CreatedBy)
            .WithMany()
            .HasForeignKey(gr => gr.CreatedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(gr => gr.DelDate == null);
    }
}
