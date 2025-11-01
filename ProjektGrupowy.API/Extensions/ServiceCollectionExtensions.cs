using System.Text.Json.Serialization;
using ProjektGrupowy.API.Filters;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

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

    public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var lokiUrl = configuration["Loki:Url"] ?? "http://loki:3100";
        var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var labels = new[]
        {
            new LokiLabel { Key = "app", Value = "projektgrupowy" },
            new LokiLabel { Key = "env", Value = envName },
            new LokiLabel { Key = "service", Value = "api" }
        };

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("app", "ProjektGrupowy.API")
            .Enrich.WithProperty("env", envName)
            .WriteTo.Console()
            .WriteTo.File("Logs/projekt_grupowy.log", rollingInterval: RollingInterval.Day)
            .WriteTo.GrafanaLoki(lokiUrl, labels: labels)
            .CreateLogger();

        services.AddLogging(lb => lb.ClearProviders().AddSerilog());
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