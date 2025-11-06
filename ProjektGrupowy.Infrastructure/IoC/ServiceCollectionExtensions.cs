using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using ProjektGrupowy.Application.Interfaces.Persistence;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Application.Interfaces.SignalR;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;
using ProjektGrupowy.Infrastructure.Persistance;
using ProjektGrupowy.Infrastructure.Persistance.Repositories;
using ProjektGrupowy.Infrastructure.SignalR;

namespace ProjektGrupowy.Infrastructure.IoC;

public sealed class HangfireNpgsqlConnectionFactory(NpgsqlDataSource ds) : IConnectionFactory
{
    public NpgsqlConnection GetOrCreateConnection() => ds.CreateConnection();
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString)
        {
            Name = $"{nameof(AppDbContext)}"
        };

        dataSourceBuilder.ConfigureTracing(o =>
        {
            o.ConfigureCommandFilter(cmd => !cmd.CommandText.StartsWith("COMMIT", StringComparison.OrdinalIgnoreCase) &&
                                            !cmd.CommandText.StartsWith("ROLLBACK", StringComparison.OrdinalIgnoreCase));
        });
        
        var dataSource = dataSourceBuilder.Build();

        _ = services.AddDbContext<AppDbContext>(options =>
            options
                .UseNpgsql(dataSource)
                .UseLazyLoadingProxies()
                .UseSnakeCaseNamingConvention()
        );

        services.AddSingleton(dataSource);

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
        _ = services.AddSignalR(options =>
        {
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            options.EnableDetailedErrors = environment.IsDevelopment();
        });

        // Configure SignalR to use the 'sub' claim as the user identifier
        // _ = services.AddSingleton<Microsoft.AspNetCore.SignalR.IUserIdProvider, SubClaimUserIdProvider>();

        _ = services.AddScoped<IMessageService, SignalRMessageService>();

        // Hangfire Storage and Server
        _ = services.AddHangfireStorage(environment);
        services.AddHangfireServer();

        return services;
    }

    private static IServiceCollection AddHangfireStorage(
        this IServiceCollection services,
        IHostEnvironment environment)
    {
        services.AddSingleton<IConnectionFactory, HangfireNpgsqlConnectionFactory>();
        services.AddHangfire((sp, config) =>
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
                var connectionFactory = sp.GetRequiredService<IConnectionFactory>();
                config.UsePostgreSqlStorage(c => c.UseConnectionFactory(connectionFactory));
            }
        });

        return services;
    }
}