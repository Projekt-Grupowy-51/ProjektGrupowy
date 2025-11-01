using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Application.Services.Background;
using ProjektGrupowy.Application.Services.Background.Impl;
using ProjektGrupowy.Application.Services.Impl;

namespace ProjektGrupowy.Application.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // MediatR
        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        // AutoMapper
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ServiceCollectionExtensions).Assembly));

        // Services
        _ = services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Background Services
        _ = services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

        // Authorization
        _ = services.AddScoped<IAuthorizationHandler, CustomAuthorizationHandler>();

        _ = services.AddOpenTelemetry()
            .WithMetrics(b => b
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddMeter("Npgsql")
                .AddPrometheusExporter())
            .WithTracing(t => t
                .AddAspNetCoreInstrumentation(o => o.RecordException = true)
                .AddNpgsql());

        return services;
    }

    /// <summary>
    /// Registers Hangfire recurring jobs. Should be called after the application has started and JobStorage is configured.
    /// </summary>
    public static void RegisterHangfireJobs()
    {
        // Domain events are now published immediately via MediatR pipeline (see DomainEventSavedNotificationHandler)
        // This Hangfire job is kept as a fallback mechanism for unpublished events
        RecurringJob.AddOrUpdate<IDomainEventPublisher>(
            "publish-domain-events-fallback",
            publisher => publisher.PublishPendingEventsAsync(),
            // "*/30 * * * * *" // Every 30 seconds (fallback only)
            Cron.Never()
        );
    }
}
