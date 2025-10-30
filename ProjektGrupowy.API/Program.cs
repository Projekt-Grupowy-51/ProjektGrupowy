using ProjektGrupowy.API.Extensions;
using ProjektGrupowy.Infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);

// Configure all services
builder.ConfigureServices();

var app = builder.Build();

// Configure middleware pipeline
app.UseExceptionHandlingConfiguration()
    .UseSwaggerConfiguration()
    .UseHangfireDashboardConfiguration()
    .UseHealthChecksConfiguration()
    .UseBasicConfiguration();

// Apply migrations
await app.ApplyMigrationsAsync();

app.Run();
