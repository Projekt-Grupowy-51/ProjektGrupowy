using ProjektGrupowy.API.Extensions;

var builder = WebApplication.CreateBuilder(args)
    .RegisterDataSources()
    .RegisterHttpServices()
    .ConfigureJson()
    .ConfigureLogging()
    .ConfigureHealthChecks()
    .RegisterBasicHangfire()
    .RegisterCors()
    .ApplySwaggerConfig()
    .ApplyKestrelConfig()
    .RegisterScopedRepositories()
    .RegisterScopedServices()
    .RegisterMapper()
    .RegisterFilters()
    .RegisterAuth();

var app = builder.Build()
    .RegisterHangfireDashboard()
    .RegisterHealth()
    .ConfigureSwagger()
    .ConfigureBasicSettings()
    .ConfigureExceptionHandling();

await app.ApplyMigrationsAsync();

app.Run();
