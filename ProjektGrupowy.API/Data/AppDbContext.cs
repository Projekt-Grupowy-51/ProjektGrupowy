using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Models;

namespace ProjektGrupowy.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureProjects(modelBuilder);
    }

    private void ConfigureProjects(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>().HasKey(p => p.Id);
        modelBuilder.Entity<Project>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
    }
}