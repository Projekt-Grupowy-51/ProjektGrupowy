using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VidMark.Domain.Models;

namespace VidMark.Infrastructure.Persistance.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user_entity", t => t.ExcludeFromMigrations());

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id");

        builder.Property(u => u.UserName)
            .HasColumnName("username")
            .IsRequired();

        builder.Property(u => u.Email)
            .HasColumnName("email");

        builder.Property(u => u.FirstName)
            .HasColumnName("first_name");

        builder.Property(u => u.LastName)
            .HasColumnName("last_name");

        builder.Property(u => u.Enabled)
            .HasColumnName("enabled");

        builder.Property(u => u.CreatedTimestamp)
            .HasColumnName("created_timestamp");

        builder.Ignore(u => u.RegistrationDate);
    }
}
