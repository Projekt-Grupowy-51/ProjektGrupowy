using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var loggerConfig = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console();
    
Log.Logger = loggerConfig.CreateLogger();

builder.Host.UseSerilog();

var jsonFile = builder.Configuration["Ocelot:JsonFile"]!;

builder.Configuration.AddJsonFile(jsonFile, optional: false, reloadOnChange: true);

builder.Services.AddOcelot();

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        var allowedDevOrigin = builder.Configuration["Cors:AllowedDevOrigin"];
        var allowedProdOrigin = builder.Configuration["Cors:AllowedProductionOrigin"];

        policy.WithOrigins(allowedDevOrigin!, allowedProdOrigin!)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("FrontendPolicy");

app.UseHttpsRedirection();

app.UseWebSockets();
await app.UseOcelot();
app.Run();