using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Infrastructure.Persistance.Configurations;

public class AssignedLabelConfiguration : IEntityTypeConfiguration<AssignedLabel>
{
    public void Configure(EntityTypeBuilder<AssignedLabel> builder)
    {
        builder.HasKey(al => al.Id);
        builder.Ignore(b => b.DomainEvents);

        builder.Property(al => al.Start)
            .IsRequired();

        builder.Property(al => al.End)
            .IsRequired();

        builder.Property(al => al.CreatedById)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(al => al.InsDate)
            .IsRequired();

        builder.HasOne(al => al.Label)
            .WithMany(l => l.AssignedLabels)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(al => al.Video)
            .WithMany(v => v.AssignedLabels)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(al => al.CreatedBy)
            .WithMany()
            .HasForeignKey(al => al.CreatedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(al => al.DelDate == null);
    }
}
