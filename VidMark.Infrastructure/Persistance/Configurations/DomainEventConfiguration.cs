using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VidMark.Domain.Events;

namespace VidMark.Infrastructure.Persistance.Configurations;

public class DomainEventConfiguration : IEntityTypeConfiguration<DomainEvent>
{
    public void Configure(EntityTypeBuilder<DomainEvent> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Message)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.UserId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.EventType)
            .HasMaxLength(255);

        builder.Property(e => e.OccurredAt)
            .IsRequired();

        builder.Property(e => e.IsPublished)
            .IsRequired();
    }
}
