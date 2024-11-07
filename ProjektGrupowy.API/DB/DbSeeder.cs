using ProjektGrupowy.API.Models;

namespace ProjektGrupowy.API.Data
{
    public static class DbSeeder
    {
        public static async Task Seed(AppDbContext context)
        {
            if (!context.Projects.Any())
            {
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
    }
}
