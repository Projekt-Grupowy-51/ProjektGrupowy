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

        // Konfiguracja związku wiele do wielu między projektami a wideo.
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Videos)
            .WithMany(v => v.Projects)
            .UsingEntity<Dictionary<string, object>>(
                "ProjectVideo",
                j => j.HasOne<Video>().WithMany().HasForeignKey("VideoId"),
                j => j.HasOne<Project>().WithMany().HasForeignKey("ProjectId"));

        ConfigureProjects(modelBuilder);
    }

    private void ConfigureProjects(ModelBuilder modelBuilder)
    {

    }
}