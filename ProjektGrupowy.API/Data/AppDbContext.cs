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


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}