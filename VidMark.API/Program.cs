using VidMark.API.Extensions;
using VidMark.Infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);

// Configure all services
builder.ConfigureServices(builder.Configuration);

var app = builder.Build();

// Configure middleware pipeline
app.UseExceptionHandlingConfiguration()
    .UseSwaggerConfiguration()
    .UseHangfireDashboardConfiguration()
    .UseHealthChecksConfiguration()
    .UseBasicConfiguration();

// Apply migrations
await app.ApplyMigrationsAsync();

app.MapPrometheusScrapingEndpoint("/metrics");

app.Run();

// Make Program class accessible to integration tests
public partial class Program { }
