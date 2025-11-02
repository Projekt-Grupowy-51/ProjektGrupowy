using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Events;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Infrastructure.Persistance.Repositories;

namespace ProjektGrupowy.Infrastructure.Persistance;

public class AppDbContext : DbContext, IApplicationDbContext, IReadWriteContext
{
    private readonly ICurrentUserService? _currentUserService;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
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
    public DbSet<User> Users { get; set; }
    public DbSet<DomainEvent> DomainEvents { get; set; }

    public override int SaveChanges()
    {
        ApplySoftDelete();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplySoftDelete();
        await DispatchDomainEventsAsync();
        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    private async Task DispatchDomainEventsAsync()
    {
        var entities = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ForEach(e => e.ClearDomainEvents());

        await DomainEvents.AddRangeAsync(domainEvents);
    }

    private void ApplySoftDelete()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Deleted && entry.Properties.Any(p => p.Metadata.Name == nameof(IOwnedEntity.DelDate)))
            {
                entry.State = EntityState.Modified;
                entry.Property(nameof(IOwnedEntity.DelDate)).CurrentValue = DateTime.UtcNow;

                // Add domain event for soft delete
                if (entry.Entity is BaseEntity baseEntity)
                {
                    var entityName = entry.Entity.GetType().Name;
                    var userId = _currentUserService?.UserId ?? "system";
                    baseEntity.AddDomainEvent($"{GetPolishEntityName(entityName)} został usunięty!", userId);
                }
            }
        }
    }

    private string GetPolishEntityName(string entityName)
    {
        return entityName switch
        {
            nameof(Project) => "Projekt",
            nameof(Subject) => "Temat",
            nameof(Video) => "Wideo",
            nameof(VideoGroup) => "Grupa wideo",
            nameof(Label) => "Etykieta",
            nameof(AssignedLabel) => "Przypisana etykieta",
            nameof(SubjectVideoGroupAssignment) => "Przypisanie",
            nameof(ProjectAccessCode) => "Kod dostępu",
            _ => "Obiekt"
        };
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

        // Configure User as read-only mapping to Keycloak table (exclude from migrations)
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user_entity", t => t.ExcludeFromMigrations());
            entity.HasKey(u => u.Id);
        });

        modelBuilder.Entity<Project>()
            .HasOne(p => p.CreatedBy)
            .WithMany(u => u.OwnedProjects) // Musi istnieć w User.cs
            .HasForeignKey(p => p.CreatedById)
            .OnDelete(DeleteBehavior.SetNull);

        // === Relacja N:N - Labelerzy w projekcie ===
        modelBuilder.Entity<Project>()
            .HasMany(p => p.ProjectLabelers)
            .WithMany(u => u.LabeledProjects) // Musi istnieć w User.cs
            .UsingEntity(j => j.ToTable("project_labelers"));

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
            .UsingEntity(j => j.ToTable("labelers_assignments"));

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

        // === DomainEvent Configuration ===
        modelBuilder.Entity<DomainEvent>()
            .HasIndex(de => de.IsPublished);
    }
}
