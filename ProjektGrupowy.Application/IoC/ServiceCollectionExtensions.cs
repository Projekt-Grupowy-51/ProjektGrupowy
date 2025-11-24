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
        // Configuration
        _ = services.Configure<Configuration.OutboxSettings>(configuration.GetSection(Configuration.OutboxSettings.SectionName));

        // MediatR
        _ = services.AddMediatR(cfg =>
        {
            _ = cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

            // Register pipeline behaviors
            _ = cfg.AddOpenBehavior(typeof(Pipelines.OutboxProcessingBehavior<,>));
        });

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
    /// <param name="configuration">Configuration to read OutboxSettings</param>
    public static void RegisterHangfireJobs(IConfiguration configuration)
    {
        var outboxSettings = configuration.GetSection("Outbox").Get<Configuration.OutboxSettings>()
            ?? new Configuration.OutboxSettings();

        if (outboxSettings.IsCronMode)
        {
            // Process outbox via Hangfire cron job
            RecurringJob.AddOrUpdate<IDomainEventPublisher>(
                "publish-domain-events",
                publisher => publisher.PublishPendingEventsAsync(),
                "*/10 * * * * *" // Every 10 seconds
            );
        }
        // Otherwise don't register any job - processing is handled by MediatR pipeline
    }
}
