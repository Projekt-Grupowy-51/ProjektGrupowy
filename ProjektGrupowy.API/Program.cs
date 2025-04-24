using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Repositories.Impl;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Services.Impl;
using ProjektGrupowy.API.Utils.Constants;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;
using ProjektGrupowy.API.SignalR;
using Azure.Core;
using Microsoft.AspNetCore.SignalR;
using ProjektGrupowy.API.Utils.Extensions;

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

app.MapHealthChecks("/health");

// Seed the database and create roles
await MigrateDatabase(app.Services);
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

// Global exception handling
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        
        using var scope = app.Services.CreateScope();
        var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error != null)
        {
            Log.Error("An error occurred: {Error}", exceptionHandlerPathFeature.Error);

            try
            {
                var identity = context.User.GetUserId();
                var errorMessage = exceptionHandlerPathFeature.Error.Message;
                await messageService.SendErrorAsync(identity, $"Something went wrong: {errorMessage}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred: {Error}", ex.Message);
            }

            await context.Response.WriteAsync(new
            {
                StatusCode = context.Response.StatusCode,
                Message = "An internal server error occurred."
            }.ToString());
        }
    });
});

app.MapHub<AppHub>("/hub/app");


app.Run();

static void AddServices(WebApplicationBuilder builder)
{
    builder.Services
        .AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

    builder.Services.AddHealthChecks();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ProjektGrupowy API",
            Version = "v1",
            Description = "API for ProjektGrupowy application",
            Contact = new OpenApiContact
            {
                Name = "Support",
                Email = "support@projektgrupowy.com"
            }
        });
    });

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("FrontendPolicy", policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddTransient<ICurrentUserService, CurrentUserService>();

    // Repositories
    builder.Services.AddScoped<IAssignedLabelRepository, AssignedLabelRepository>();
    builder.Services.AddScoped<ILabelRepository, LabelRepository>();
    builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
    builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
    builder.Services.AddScoped<ISubjectVideoGroupAssignmentRepository, SubjectVideoGroupAssignmentRepository>();
    builder.Services.AddScoped<IVideoGroupRepository, VideoGroupRepository>();
    builder.Services.AddScoped<IVideoRepository, VideoRepository>();
    builder.Services.AddScoped<IProjectAccessCodeRepository, ProjectAccessCodeRepository>();

    // Services
    builder.Services.AddScoped<IAssignedLabelService, AssignedLabelService>();
    builder.Services.AddScoped<ILabelService, LabelService>();
    builder.Services.AddScoped<IProjectService, ProjectService>();
    builder.Services.AddScoped<ISubjectService, SubjectService>();
    builder.Services.AddScoped<ISubjectVideoGroupAssignmentService, SubjectVideoGroupAssignmentService>();
    builder.Services.AddScoped<IVideoGroupService, VideoGroupService>();
    builder.Services.AddScoped<IVideoService, VideoService>();
    builder.Services.AddScoped<IProjectAccessCodeService, ProjectAccessCodeService>();

    builder.Services.AddSingleton<IConnectedClientManager, ConnectedClientManager>();
    builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
    builder.Services.AddSignalR(options =>
    {
        options.EnableDetailedErrors = true;
        options.KeepAliveInterval = TimeSpan.FromSeconds(15); // Default is 15 seconds
        options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    });
    builder.Services.AddSingleton<IMessageService, MessageService>();

    // AutoMapper
    builder.Services.AddAutoMapper(typeof(Program).Assembly);

    // Filters
    builder.Services.AddScoped<ValidateModelStateFilter>();
    builder.Services.AddScoped<NonSuccessGetFilter>();

    // Database Context
    builder.Services.AddDbContext<AppDbContext>(options =>
        options
            .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            .UseLazyLoadingProxies()
    );

    // Identity
    builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

    // JWT Authentication
    var jwtSecret = builder.Configuration["JWT:Secret"];
    var jwtIssuer = builder.Configuration["JWT:ValidIssuer"];
    var jwtAudience = builder.Configuration["JWT:ValidAudience"];
    var jwtCookieName = builder.Configuration["JWT:JwtCookieName"];

    if (string.IsNullOrEmpty(jwtSecret)) throw new ArgumentNullException("JWT:Secret is missing in configuration.");
    if (string.IsNullOrEmpty(jwtIssuer)) throw new ArgumentNullException("JWT:ValidIssuer is missing in configuration.");
    if (string.IsNullOrEmpty(jwtAudience)) throw new ArgumentNullException("JWT:ValidAudience is missing in configuration.");

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
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var token = context.Request.Cookies[jwtCookieName];
                    
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(token) && path.ToString().Contains("/hub/app"))
                    {
                        context.Token = token;
                    }

                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Token = token;
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    Log.Error("Authentication failed: {Exception}", context.Exception);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Log.Information("Token validated for user: {Username}", context.Principal.Identity.Name);
                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireRole(RoleConstants.Admin, RoleConstants.Scientist, RoleConstants.Labeler)
            .Build();

        options.AddPolicy(PolicyConstants.RequireAdminOrScientist, policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole(RoleConstants.Admin) ||
                context.User.IsInRole(RoleConstants.Scientist)));
    });

    // Response Compression
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
    });
}

static async Task MigrateDatabase(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
}

static async Task CreateRoles(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var roleName in RoleConstants.AllRoles)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (roleResult.Succeeded)
            {
                Log.Information($"Rola {roleName} została pomyślnie utworzona.");
            }
            else
            {
                Log.Error($"Błąd przy tworzeniu roli {roleName}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            Log.Information($"Rola {roleName} już istnieje.");
        }
    }
}