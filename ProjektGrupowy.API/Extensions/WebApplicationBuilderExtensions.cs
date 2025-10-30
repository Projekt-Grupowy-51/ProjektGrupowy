using ProjektGrupowy.Application.IoC;
using ProjektGrupowy.Infrastructure.IoC;
using Serilog;

namespace ProjektGrupowy.API.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        // Logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .WriteTo.Console()
            .WriteTo.File("Logs/internal_api.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

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
