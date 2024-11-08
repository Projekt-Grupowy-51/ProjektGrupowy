using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Models;
using System.Reflection.Emit;

namespace ProjektGrupowy.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

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
}
