using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.DB;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Mapper;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Repositories.Impl;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Services.Impl;
using ProjektGrupowy.API.Utils.Enums;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Log Configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("Logs/internal_api.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Add services to the container.
AddServices(builder);

builder.Host.UseSerilog();

var app = builder.Build();

// Seed the database and create roles
await SeedDatabase(app.Services);
await CreateRoles(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static void AddServices(WebApplicationBuilder builder)
{
    builder.Services.AddControllers()
        .AddJsonOptions(options => {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            //options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("FrontendPolicy", policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Adres Reacta
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });



    // Project
    builder.Services.AddScoped<IProjectService, ProjectService>();
    builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

    // Video
    builder.Services.AddScoped<IVideoService, VideoService>();
    builder.Services.AddScoped<IVideoRepository, VideoRepository>();

    // Mapper
    builder.Services.AddAutoMapper(typeof(ProjectMap));
    builder.Services.AddAutoMapper(typeof(VideoMap));

    builder.Services.AddScoped<ValidateModelStateFilter>();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                ValidAudience = builder.Configuration["JWT:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
            };
        });
}

static async Task SeedDatabase(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(context);
}

static async Task CreateRoles(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roleNames = Enum.GetValues(typeof(RoleEnum)).Cast<RoleEnum>();

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName.ToString());
        if (!roleExist)
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName.ToString()));
            if (roleResult.Succeeded)
            {
                Log.Information($"Rola {roleName} zosta³a pomyœlnie utworzona.");
            }
            else
            {
                Log.Error(
                    $"B³¹d przy tworzeniu roli {roleName}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            Log.Information($"Rola {roleName} ju¿ istnieje.");
        }
    }
}