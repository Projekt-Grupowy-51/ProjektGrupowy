using ProjektGrupowy.Notifications.Extensions;

var builder = WebApplication.CreateBuilder(args)
    .RegisterHttpServices()
    .ConfigureJson()
    .ConfigureLogging()
    .ConfigureHealthChecks()
    .RegisterCors()
    .ApplySwaggerConfig()
    .ApplyKestrelConfig()
    .RegisterScopedServices()
    .RegisterSignalR()
    .RegisterFilters()
    .RegisterAuth();

var app = builder.Build()
    .RegisterHealth()
    .ConfigureSwagger()
    .ConfigureSignalR()
    .ConfigureBasicSettings()
    .ConfigureExceptionHandling();

app.Run();