using System.Text.Json.Serialization;
using ProjektGrupowy.API.Filters;
using Serilog;

namespace ProjektGrupowy.API.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddJsonConfiguration(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        return services;
    }

    public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .WriteTo.Console()
            .WriteTo.File("Logs/internal_api.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        return services;
    }

    public static IServiceCollection AddHealthChecksConfiguration(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }

    public static IServiceCollection AddKestrelConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var maxBodySize = int.TryParse(configuration["Limits:MaxBodySizeMb"], out var parsedMaxBodySize)
            ? parsedMaxBodySize * 1024 * 1024
            : 500 * 1024 * 1024; // fallback

        services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = maxBodySize;
        });

        services.AddResponseCompression(options => { options.EnableForHttps = true; });

        return services;
    }

    public static IServiceCollection AddFilters(this IServiceCollection services)
    {
        services.AddScoped<ValidateModelStateFilter>();
        services.AddScoped<NonSuccessGetFilter>();

        return services;
    }

    public static IServiceCollection AddHttpContextConfiguration(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        return services;
    }
}
