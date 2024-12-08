using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Models;

namespace ProjektGrupowy.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Video> Videos { get; set; }

    public DbSet<Scientist> Scientists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Scientist>()
            .HasMany(s => s.Projects)
            .WithOne(p => p.Scientist)
            .HasForeignKey(p => p.ScientistId)
            .OnDelete(DeleteBehavior.Cascade);
            

        modelBuilder.Entity<Project>()
            .HasMany(p => p.Videos)
            .WithOne(v => v.Project)
            .HasForeignKey(v => v.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        ConfigureProjects(modelBuilder);
    }

    private void ConfigureProjects(ModelBuilder modelBuilder)
    {

    }
}