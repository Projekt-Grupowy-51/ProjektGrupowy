using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ProjektGrupowy.Application.Services;
using System.Security.Claims;

namespace ProjektGrupowy.Infrastructure.Persistance;

/// <summary>
/// Factory for creating AppDbContext instances at design-time (e.g., during migrations).
/// This is required because AppDbContext has dependencies (ICurrentUserService) that can't be resolved during EF Tools operations.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Read connection string from environment variable
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string not found. Please set the 'ConnectionStrings__DefaultConnection' environment variable before running migrations.");
        }

        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.UseLazyLoadingProxies();

        // Create a design-time instance of ICurrentUserService
        var designTimeUserService = new DesignTimeCurrentUserService();

        return new AppDbContext(optionsBuilder.Options, designTimeUserService);
    }

    /// <summary>
    /// Design-time implementation of ICurrentUserService.
    /// This is a simple mock that returns default values since we don't have a real user during migrations.
    /// </summary>
    private class DesignTimeCurrentUserService : ICurrentUserService
    {
        public string UserId => "system";
        public string? UserName => "System";
        public IEnumerable<string> Roles => Array.Empty<string>();
        public bool IsAdmin => false;
        public ClaimsPrincipal User => new ClaimsPrincipal();
        public bool IsAuthenticated => false;
    }
}