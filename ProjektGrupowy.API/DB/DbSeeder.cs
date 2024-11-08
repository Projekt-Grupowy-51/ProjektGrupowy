using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;

namespace ProjektGrupowy.API.DB;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (context.Projects.Any())
            return;

        await context.Projects.AddRangeAsync(
            new Project
            {
                Name = "Projekt 1",
                Description = "Opis projektu 1"
            },
            new Project
            {
                Name = "Projekt 2",
                Description = "Opis projektu 2"
            },
            new Project
            {
                Name = "Projekt 3",
                Description = "Opis projektu 3"
            }
        );

        await context.SaveChangesAsync();
    }
}