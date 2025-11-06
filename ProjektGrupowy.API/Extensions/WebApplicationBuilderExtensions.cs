using ProjektGrupowy.Application.IoC;
using ProjektGrupowy.Infrastructure.IoC;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

namespace ProjektGrupowy.API.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        // Logging
        var lokiUrl = configuration["Loki:Url"] ?? "http://loki:3100";
        var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var labels = new[]
        {
            new LokiLabel { Key = "app", Value = "projektgrupowy" },
            new LokiLabel { Key = "env", Value = envName },
            new LokiLabel { Key = "service", Value = "api" }
        };

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("app", "ProjektGrupowy.API")
            .Enrich.WithProperty("env", envName)
            .WriteTo.Console()
            .WriteTo.File("Logs/projekt_grupowy.log", rollingInterval: RollingInterval.Day)
            .WriteTo.GrafanaLoki(lokiUrl, labels: labels)
            .CreateLogger();

        builder.Services.AddLogging(lb => lb.ClearProviders().AddSerilog());

        builder.Host.UseSerilog();

        // Infrastructure layer (Database, Repositories, UnitOfWork, Hangfire Storage & Server, SignalR)
        builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);

        // Application layer (MediatR, AutoMapper, Services, Hangfire Client & Jobs)
        builder.Services.AddApplicationServices(builder.Configuration);

        // API layer configuration
        builder.Services.AddHttpContextConfiguration();
        builder.Services.AddJsonConfiguration();
        builder.Services.AddHealthChecksConfiguration();
        builder.Services.AddKestrelConfiguration(builder.Configuration);
        builder.Services.AddFilters();

        // CORS
        builder.Services.AddCorsConfiguration(builder.Configuration);

        // Authentication & Authorization
        builder.Services.AddAuthenticationConfiguration(builder.Configuration, builder.Environment);
        builder.Services.AddAuthorizationConfiguration();

        // Swagger
        builder.Services.AddSwaggerConfiguration();

        return builder;
    }
}
