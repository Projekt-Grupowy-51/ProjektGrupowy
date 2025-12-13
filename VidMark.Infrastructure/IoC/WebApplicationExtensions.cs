using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VidMark.Infrastructure.Persistance;

namespace VidMark.Infrastructure.IoC;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Applies pending database migrations automatically on application startup.
    /// </summary>
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        // Skip migrations in Testing environment
        if (app.Environment.EnvironmentName == "Testing")
        {
            return;
        }

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }
}
