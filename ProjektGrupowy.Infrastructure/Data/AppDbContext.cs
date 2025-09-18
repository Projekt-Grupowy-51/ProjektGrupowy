using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Infrastructure.Services;

namespace ProjektGrupowy.Infrastructure.Data;

public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    ICurrentUserService currentUserService) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<AssignedLabel> AssignedLabels { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<SubjectVideoGroupAssignment> SubjectVideoGroupAssignments { get; set; }
    public DbSet<VideoGroup> VideoGroups { get; set; }
    public DbSet<ProjectAccessCode> ProjectAccessCodes { get; set; }
    public DbSet<GeneratedReport> GeneratedReports { get; set; }
    public DbSet<User> Users { get; set; }

    private string CurrentUserId => currentUserService.UserId;
    private bool CurrentIsAdmin => currentUserService.IsAdmin;
    private bool IsAuthenticated => currentUserService.IsAuthenticated;

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
            if (entry.State == EntityState.Deleted &&
                entry.Properties.Any(p => p.Metadata.Name == nameof(IOwnedEntity.DelDate)))
            {
                entry.State = EntityState.Modified;
                entry.Property(nameof(IOwnedEntity.DelDate)).CurrentValue = DateTime.UtcNow;
            }
        }
    }

    private static void ApplyQueryFilterIndexes(ModelBuilder modelBuilder)
    {
        foreach (var et in modelBuilder.Model.GetEntityTypes())
        {
            var clr = et.ClrType;
            if (et.IsKeyless || string.IsNullOrEmpty(et.GetTableName()) || !typeof(IOwnedEntity).IsAssignableFrom(clr)) 
                continue;
            
            if (et.FindProperty(nameof(IOwnedEntity.CreatedById)) == null) 
                continue;
            
            if (et.FindProperty(nameof(IOwnedEntity.DelDate)) == null) 
                continue;

            var eb = modelBuilder.Entity(clr);
            
            eb.HasIndex(nameof(IOwnedEntity.CreatedById))
                .HasDatabaseName($"ix_{et.GetTableName()}_owner_live")
                .HasFilter("\"DelDate\" IS NULL");
        }
    }


    private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
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

        // Configure User as read-only view from Keycloak
        modelBuilder.Entity<User>()
            .ToView("user_entity")
            .HasKey(u => u.Id);

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

        // 1:N - Project -> VideoGroups  
        modelBuilder.Entity<Project>()
            .HasMany(p => p.VideoGroups)
            .WithOne(vg => vg.Project)
            .OnDelete(DeleteBehavior.Cascade);

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

        // === GLOBAL QUERY FILTERS ===

        modelBuilder.Entity<Project>()
            .HasQueryFilter(p =>
                p.DelDate == null &&
                (CurrentIsAdmin ||
                 (IsAuthenticated &&
                  (p.CreatedById == CurrentUserId ||
                   p.ProjectLabelers.Any(u => u.Id == CurrentUserId)))));

        modelBuilder.Entity<VideoGroup>()
            .HasQueryFilter(vg =>
                vg.DelDate == null &&
                (CurrentIsAdmin ||
                 (IsAuthenticated &&
                  (vg.Project.CreatedById == CurrentUserId ||
                   vg.SubjectVideoGroupAssignments.Any(svga =>
                       svga.Labelers.Any(l => l.Id == CurrentUserId))))));

        modelBuilder.Entity<Video>()
            .HasQueryFilter(v =>
                v.DelDate == null &&
                (CurrentIsAdmin ||
                 (IsAuthenticated &&
                  (v.VideoGroup.Project.CreatedById == CurrentUserId ||
                   v.VideoGroup.SubjectVideoGroupAssignments.Any(svga =>
                       svga.Labelers.Any(l => l.Id == CurrentUserId))))));

        modelBuilder.Entity<Subject>()
            .HasQueryFilter(s =>
                s.DelDate == null &&
                (CurrentIsAdmin ||
                 (IsAuthenticated &&
                  (s.Project.CreatedById == CurrentUserId ||
                   s.SubjectVideoGroupAssignments.Any(svga =>
                       svga.Labelers.Any(l => l.Id == CurrentUserId))))));

        modelBuilder.Entity<Label>()
            .HasQueryFilter(l =>
                l.DelDate == null &&
                (CurrentIsAdmin ||
                 (IsAuthenticated &&
                  (l.Subject.Project.CreatedById == CurrentUserId ||
                   l.Subject.SubjectVideoGroupAssignments.Any(svga =>
                       svga.Labelers.Any(lab => lab.Id == CurrentUserId))))));

        modelBuilder.Entity<AssignedLabel>()
            .HasQueryFilter(al =>
                al.DelDate == null &&
                (CurrentIsAdmin ||
                 (IsAuthenticated &&
                  (al.CreatedById == CurrentUserId ||
                   al.Video.VideoGroup.Project.CreatedById == CurrentUserId))));

        modelBuilder.Entity<ProjectAccessCode>()
            .HasQueryFilter(pac =>
                pac.DelDate == null &&
                (CurrentIsAdmin ||
                 (IsAuthenticated && pac.CreatedById == CurrentUserId)));

        modelBuilder.Entity<SubjectVideoGroupAssignment>()
            .HasQueryFilter(svga =>
                svga.DelDate == null &&
                (CurrentIsAdmin ||
                 (IsAuthenticated &&
                  (svga.Subject.Project.CreatedById == CurrentUserId ||
                   svga.Labelers.Any(l => l.Id == CurrentUserId)))));

        modelBuilder.Entity<GeneratedReport>()
            .HasQueryFilter(r =>
                r.DelDate == null &&
                (CurrentIsAdmin ||
                 (IsAuthenticated && r.CreatedById == CurrentUserId)));
        
        ApplyQueryFilterIndexes(modelBuilder);
    }
}