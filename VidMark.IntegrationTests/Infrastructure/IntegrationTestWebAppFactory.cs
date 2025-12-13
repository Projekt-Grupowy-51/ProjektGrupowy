using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VidMark.Application.Services;
using VidMark.Infrastructure.Persistance;
using Testcontainers.PostgreSql;

namespace VidMark.IntegrationTests.Infrastructure;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;

    public IntegrationTestWebAppFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:17")
            .WithDatabase("test_db")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .WithCleanUp(true)
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.RemoveAll(typeof(AppDbContext));

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString())
                    .UseSnakeCaseNamingConvention();

                var currentUserService = sp.GetService<ICurrentUserService>();
                if (currentUserService != null)
                {
                    // DbContext constructor requires ICurrentUserService
                }
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthenticationHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthenticationHandler.SchemeName;
            })
            .AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>(
                TestAuthenticationHandler.SchemeName,
                options => { });

            services.RemoveAll(typeof(ICurrentUserService));
            services.AddScoped<ICurrentUserService, TestCurrentUserService>();

            services.RemoveAll(typeof(VidMark.Application.Interfaces.SignalR.IMessageService));
            services.AddScoped<VidMark.Application.Interfaces.SignalR.IMessageService, TestMessageService>();

            services.RemoveAll(typeof(Hangfire.IBackgroundJobClient));
            services.RemoveAll(typeof(Hangfire.IRecurringJobManager));
        });

        builder.UseEnvironment("Testing");
    }

    public async Task<AppDbContext> GetDbContextAsync()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.EnsureDeletedAsync();

        var dbName = dbContext.Database.GetDbConnection().Database;

        var connectionStringBuilder = new Npgsql.NpgsqlConnectionStringBuilder(_dbContainer.GetConnectionString())
        {
            Database = "postgres"
        };

        using (var adminConnection = new Npgsql.NpgsqlConnection(connectionStringBuilder.ToString()))
        {
            await adminConnection.OpenAsync();

            using (var command = adminConnection.CreateCommand())
            {
                command.CommandText = $"CREATE DATABASE {dbName}";
                await command.ExecuteNonQueryAsync();
            }
        }

        using (var testDbConnection = new Npgsql.NpgsqlConnection(_dbContainer.GetConnectionString()))
        {
            await testDbConnection.OpenAsync();

            using (var command = testDbConnection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE user_entity (
                        id TEXT PRIMARY KEY,
                        username TEXT NOT NULL,
                        email TEXT,
                        first_name TEXT,
                        last_name TEXT,
                        enabled BOOLEAN NOT NULL DEFAULT true,
                        created_timestamp BIGINT NOT NULL DEFAULT 0
                    );";
                await command.ExecuteNonQueryAsync();
            }
        }

        var script = dbContext.Database.GenerateCreateScript();
        await dbContext.Database.ExecuteSqlRawAsync(script);
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}
