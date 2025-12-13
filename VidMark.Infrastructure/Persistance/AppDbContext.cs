using Microsoft.EntityFrameworkCore;
using VidMark.Application.Services;
using VidMark.Domain.Events;
using VidMark.Domain.Models;
using VidMark.Infrastructure.Persistance.Repositories;

namespace VidMark.Infrastructure.Persistance;

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
    public DbSet<SubjectVideoGroupAssignmentCompletion> AssignmentCompletions { get; set; }

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // === Additional configurations for N:N relationships ===

        // N:N - Labelers in project
        modelBuilder.Entity<Project>()
            .HasMany(p => p.ProjectLabelers)
            .WithMany(u => u.LabeledProjects)
            .UsingEntity(j => j.ToTable("project_labelers"));

        // N:N - Labelers assigned to SubjectVideoGroupAssignments
        // Note: This is already configured in SubjectVideoGroupAssignmentConfiguration
        // but we override the table name here for consistency with existing database
        modelBuilder.Entity<SubjectVideoGroupAssignment>()
            .HasMany(svga => svga.Labelers)
            .WithMany(u => u.LabeledAssignments)
            .UsingEntity(j => j.ToTable("labelers_assignments"));

        // Override CreatedBy relationship DeleteBehavior for SubjectVideoGroupAssignment
        modelBuilder.Entity<SubjectVideoGroupAssignment>()
            .HasOne(svga => svga.CreatedBy)
            .WithMany(u => u.OwnedAssignments)
            .HasForeignKey(svga => svga.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // === Additional indexes ===

        modelBuilder.Entity<Video>()
            .HasIndex(v => new { v.VideoGroupId, v.PositionInQueue });

        modelBuilder.Entity<GeneratedReport>()
            .HasIndex(r => r.Path)
            .IsUnique();

        modelBuilder.Entity<DomainEvent>()
            .HasIndex(de => de.IsPublished);
    }
}
