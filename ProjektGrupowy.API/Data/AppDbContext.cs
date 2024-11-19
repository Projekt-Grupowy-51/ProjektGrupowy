using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Models;

namespace ProjektGrupowy.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Video> Videos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Związek jeden do wielu między projektami a wideo.
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Videos)
            .WithOne(v => v.Project)
            .HasForeignKey(v => v.ProjectId);

        ConfigureProjects(modelBuilder);
    }

    private void ConfigureProjects(ModelBuilder modelBuilder)
    {

    }
}