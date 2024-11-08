using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.DB;
using ProjektGrupowy.API.Mapper;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Repositories.Impl;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Services.Impl;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console() // Ensure this line now works
    .WriteTo.File("Logs/internal_api.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Add services to the container.
AddServices(builder);

builder.Host.UseSerilog();

var app = builder.Build();

//await SeedDatabase(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
return;

static void AddServices(WebApplicationBuilder builder)
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddScoped<IProjectService, ProjectService>();
    builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

    builder.Services.AddAutoMapper(typeof(ProjectMap));

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}

static async Task SeedDatabase(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(context);
}
