using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<AssignedLabel> AssignedLabels { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<SubjectVideoGroupAssignment> SubjectVideoGroupAssignments { get; set; }
    public DbSet<VideoGroup> VideoGroups { get; set; }
    public DbSet<ProjectAccessCode> ProjectAccessCodes { get; set; }
    public DbSet<GeneratedReport> GeneratedReports { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public override int SaveChanges()
    {
        ApplySoftDelete();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplySoftDelete();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplySoftDelete()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Deleted && entry.Properties.Any(p => p.Metadata.Name == nameof(IOwnedEntity.DelDate)))
            {
                entry.State = EntityState.Modified;
                entry.Property(nameof(IOwnedEntity.DelDate)).CurrentValue = DateTime.UtcNow;
            }
        }
    }

    private void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>().HasQueryFilter(p => p.DelDate == null);
        modelBuilder.Entity<VideoGroup>().HasQueryFilter(vg => vg.DelDate == null);
        modelBuilder.Entity<Video>().HasQueryFilter(v => v.DelDate == null);
        modelBuilder.Entity<Subject>().HasQueryFilter(s => s.DelDate == null);
        modelBuilder.Entity<Label>().HasQueryFilter(l => l.DelDate == null);
        modelBuilder.Entity<SubjectVideoGroupAssignment>().HasQueryFilter(svga => svga.DelDate == null);
        modelBuilder.Entity<AssignedLabel>().HasQueryFilter(al => al.DelDate == null);
        modelBuilder.Entity<ProjectAccessCode>().HasQueryFilter(pac => pac.DelDate == null);
        modelBuilder.Entity<GeneratedReport>().HasQueryFilter(re => re.DelDate == null);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // === Relacja 1:N - Właściciel projektu ===
        modelBuilder.Entity<Project>()
            .HasOne(p => p.CreatedBy)
            .WithMany(u => u.OwnedProjects) // Musi istnieć w User.cs
            .HasForeignKey(p => p.CreatedById)
            .OnDelete(DeleteBehavior.SetNull);

        // === Relacja N:N - Labelerzy w projekcie ===
        modelBuilder.Entity<Project>()
            .HasMany(p => p.ProjectLabelers)
            .WithMany(u => u.LabeledProjects) // Musi istnieć w User.cs
            .UsingEntity(j => j.ToTable("ProjectLabelers"));

        // 1:N - Właściciel przypisania
        modelBuilder.Entity<User>()
            .HasMany(u => u.OwnedAssignments)
            .WithOne(svga => svga.CreatedBy)
            .HasForeignKey(svga => svga.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // N:N - Labelerzy przypisani do zadań
        modelBuilder.Entity<SubjectVideoGroupAssignment>()
            .HasMany(svga => svga.Labelers)
            .WithMany(u => u.LabeledAssignments)
            .UsingEntity(j => j.ToTable("LabelersAssignments"));

        // === Soft Delete Filters ===
        ApplySoftDeleteFilter(modelBuilder);

        // === Pozostałe konfiguracje ===

        modelBuilder.Entity<ProjectAccessCode>()
            .HasIndex(p => p.Code)
            .HasDatabaseName("IX_ProjectAccessCode_Code")
            .IsUnique();

        modelBuilder.Entity<Video>()
            .HasOne(v => v.VideoGroup)
            .WithMany(vg => vg.Videos)
            .HasForeignKey(v => v.VideoGroupId);

        modelBuilder.Entity<Video>()
            .HasIndex(v => new { v.VideoGroupId, v.PositionInQueue });

        modelBuilder.Entity<GeneratedReport>()
            .HasIndex(r => r.Path)
            .IsUnique();
    }
}
