using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Infrastructure.Persistance.Configurations;

public class LabelConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Ignore(b => b.DomainEvents);

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(l => l.ColorHex)
            .IsRequired();

        builder.Property(l => l.Type)
            .IsRequired();

        builder.Property(l => l.CreatedById)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(l => l.Subject)
            .WithMany(s => s.Labels)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.CreatedBy)
            .WithMany()
            .HasForeignKey(l => l.CreatedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(l => l.AssignedLabels)
            .WithOne(al => al.Label)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(l => l.DelDate == null);
    }
}
