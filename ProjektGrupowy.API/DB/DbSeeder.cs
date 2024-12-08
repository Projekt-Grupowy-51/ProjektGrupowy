using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;

namespace ProjektGrupowy.API.DB;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Scientists.AnyAsync())
            return;

        var s = new Scientist
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Title = "Dr",
            Description = "Opis",
            Projects = new List<Project>
            {
                new Project
                {
                    Name = "Projekt 4",
                    Description = "Opis projektu 4",
                    Videos = new List<Video>
                    {
                        new Video
                        {
                            Title = "Video 1",
                            Description = "Opis video 1",
                            Path = "path/to/video1"
                        },
                        new Video
                        {
                            Title = "Video 2",
                            Description = "Opis video 2",
                            Path = "path/to/video2"
                        }
                    }
                },
                new Project
                {
                    Name = "Projekt 5",
                    Description = "Opis projektu 5",
                    Videos = new List<Video>
                    {
                        new Video
                        {
                            Title = "Video 3",
                            Description = "Opis video 3",
                            Path = "path/to/video3"
                        },
                        new Video
                        {
                            Title = "Video 4",
                            Description = "Opis video 4",
                            Path = "path/to/video4"
                        }
                    }

                }
            }
        };

        await context.Scientists.AddAsync(s);

        await context.SaveChangesAsync();
    }
}