using ProjektGrupowy.Background.Extensions;

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
    .RegisterAuth()
    .RegisterHangfireServer();

var app = builder.Build()
    .RegisterHangfireDashboard()
    .RegisterHealth()
    .ConfigureSwagger()
    .ConfigureBasicSettings()
    .ConfigureExceptionHandling();

app.Run();
