using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjektGrupowy.Infrastructure.Persistance;

namespace ProjektGrupowy.Infrastructure.IoC;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Applies pending database migrations automatically on application startup.
    /// </summary>
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }
}
