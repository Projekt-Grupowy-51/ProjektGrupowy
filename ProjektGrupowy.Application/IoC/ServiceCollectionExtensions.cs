using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        return services;
    }

    /// <summary>
    /// Registers Hangfire recurring jobs. Should be called after the application has started and JobStorage is configured.
    /// </summary>
    public static void RegisterHangfireJobs()
    {
        // Register recurring job for publishing domain events (every 10 seconds)
        RecurringJob.AddOrUpdate<IDomainEventPublisher>(
            "publish-domain-events",
            publisher => publisher.PublishPendingEventsAsync(),
            "*/10 * * * * *" // Every 10 seconds
        );
    }
}
