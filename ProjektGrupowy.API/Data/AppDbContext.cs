using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Models;

namespace ProjektGrupowy.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<Scientist> Scientists { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<AssignedLabel> AssignedLabels { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<SubjectVideoGroupAssignment> SubjectVideoGroupAssignments { get; set; }
    public DbSet<Labeler> Labelers { get; set; }
    public DbSet<VideoGroup> VideoGroups { get; set; }
    public DbSet<ProjectAccessCode> ProjectAccessCodes { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // === ProjectAccessCode === //

        modelBuilder.Entity<ProjectAccessCode>()
            .HasIndex(p => p.Code)
            .HasDatabaseName("IX_ProjectAccessCode_Code")
            .IsUnique();

        // ========================= //

        // === Video <-> VideoGroupId === //

        modelBuilder.Entity<Video>()
            .HasOne(v => v.VideoGroup)
            .WithMany(vg => vg.Videos)
            .HasForeignKey(v => v.VideoGroupId);

        // Index for cursor pagination
        modelBuilder.Entity<Video>()
            .HasIndex(v => new
            {
                v.VideoGroupId,
                v.PositionInQueue
            });

        // =========================== //

        base.OnModelCreating(modelBuilder);
    }
}