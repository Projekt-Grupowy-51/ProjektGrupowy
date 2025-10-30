using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjektGrupowy.Application.Interfaces.Persistence;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.SignalR;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;
using ProjektGrupowy.Infrastructure.Persistance;
using ProjektGrupowy.Infrastructure.Persistance.Repositories;
using ProjektGrupowy.Infrastructure.SignalR;

namespace ProjektGrupowy.Infrastructure.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        // Database
        _ = services.AddDbContext<AppDbContext>(options =>
            options
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseLazyLoadingProxies()
        );

        // Register AppDbContext as IReadWriteContext
        _ = services.AddScoped<IReadWriteContext>(provider => provider.GetRequiredService<AppDbContext>());

        // Unit of Work
        _ = services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        _ = services.AddScoped<IKeycloakUserRepository, KeycloakUserRepository>();
        _ = services.AddScoped<IAssignedLabelRepository, AssignedLabelRepository>();
        _ = services.AddScoped<ILabelRepository, LabelRepository>();
        _ = services.AddScoped<IProjectRepository, ProjectRepository>();
        _ = services.AddScoped<ISubjectRepository, SubjectRepository>();
        _ = services.AddScoped<ISubjectVideoGroupAssignmentRepository, SubjectVideoGroupAssignmentRepository>();
        _ = services.AddScoped<IVideoGroupRepository, VideoGroupRepository>();
        _ = services.AddScoped<IVideoRepository, VideoRepository>();
        _ = services.AddScoped<IProjectAccessCodeRepository, ProjectAccessCodeRepository>();
        _ = services.AddScoped<IProjectReportRepository, ProjectReportRepository>();
        _ = services.AddScoped<IDomainEventRepository, DomainEventRepository>();

        // SignalR
        _ = services.AddSignalR();
        _ = services.AddScoped<IMessageService, SignalRMessageService>();

        // Hangfire Storage and Server
        _ = services.AddHangfireStorage(configuration, environment);
        services.AddHangfireServer();

        return services;
    }

    private static IServiceCollection AddHangfireStorage(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddHangfire(config =>
        {
            config.UseSerilogLogProvider();
            config.UseSimpleAssemblyNameTypeSerializer();
            config.UseRecommendedSerializerSettings();

            if (environment.IsDevelopment())
            {
                config.UseMemoryStorage();
            }
            else
            {
                var hangfireConnectionString = configuration.GetConnectionString("HangfireConnection");
                config.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(hangfireConnectionString));
            }
        });

        return services;
    }
}
