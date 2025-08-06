using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using ProjektGrupowy.API.Authentication;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Application.Services.Impl;
using ProjektGrupowy.Application.SignalR;
using ProjektGrupowy.Domain.Utils.Constants;
using Serilog;
using System.Text.Json.Serialization;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Log Configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.Console()
    .WriteTo.File("Logs/internal_api.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Add services to the container.
AddServices(builder);

builder.Host.UseSerilog();

var app = builder.Build();


app.MapHealthChecks("/health");

// Seed the database

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
        context.Response.ContentType = "application/json";

        using var scope = app.Services.CreateScope();
        var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
        var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        if (exception != null)
        {
            Log.Error("An error occurred: {Error}", exception);

            if (exception is ForbiddenException)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;

                await messageService.SendErrorAsync(currentUserService.UserId, "You do not have permission to perform this action.");

                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = exception.Message
                });
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await messageService.SendErrorAsync(currentUserService.UserId, "An internal server error occurred.");

                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "An internal server error occurred."
                });
            }
        }
    });
});

app.MapHub<AppHub>(builder.Configuration["SignalR:HubUrl"] ?? "/hub/app");

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

    // ========== Hangfire ========== //

    // ========== Done with hangfire ========== //

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // CORS
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

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddScoped<IAuthorizationHandler, CustomAuthorizationHandler>();

    builder.Services.AddSingleton<IConnectionMultiplexer>(
        ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]!));
    builder.Services.AddSingleton<IConnectedClientManager, ConnectedClientManager>();
    
    
    builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
    builder.Services.AddSignalR(options =>
    {
        options.EnableDetailedErrors = true;
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    });
    builder.Services.AddSingleton<IMessageService, SignalRMessageService>();
    
    // Keycloak JWT Authentication
    var keycloakAuthority = builder.Configuration["Keycloak:Authority"];
    var keycloakAudience = builder.Configuration["Keycloak:Audience"];
    var keycloakIssuer = builder.Configuration["Keycloak:Issuer"];

    // Make Keycloak optional in development
    if (!builder.Environment.IsDevelopment())
    {
        if (string.IsNullOrEmpty(keycloakAuthority)) throw new ArgumentNullException("Keycloak:Authority is missing in configuration.");
        if (string.IsNullOrEmpty(keycloakAudience)) throw new ArgumentNullException("Keycloak:Audience is missing in configuration.");
    }

    if (!string.IsNullOrEmpty(keycloakAuthority) && !string.IsNullOrEmpty(keycloakAudience))
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = keycloakAuthority;
                options.Audience = keycloakAudience;
                options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = keycloakIssuer ?? keycloakAuthority,
                    ValidAudience = keycloakAudience,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var path = context.HttpContext.Request.Path;
                        if (path.ToString().Contains("/hub/app"))
                        {
                            var accessToken = context.Request.Query["access_token"];
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                context.Token = accessToken;
                            }
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
                        Log.Information("Token validated for user: {Username}", context.Principal?.Identity?.Name);
                        return Task.CompletedTask;
                    }
                };
            });
    }
    else
    {
        // Fallback for development without Keycloak
        builder.Services.AddAuthentication("Test")
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });
    }

    builder.Services.AddAuthorization(options =>
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        options.AddPolicy(PolicyConstants.RequireAdminOrScientist, policy =>
            policy.RequireAssertion(context =>
            {
                var user = context.User;
                var roles = GetKeycloakRoles(user);
                return roles.Contains(RoleConstants.Admin) || roles.Contains(RoleConstants.Scientist);
            }));
    });

    // Response Compression
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
    });
}

static IEnumerable<string> GetKeycloakRoles(System.Security.Claims.ClaimsPrincipal user)
{
    // Try to get roles from realm_access claim
    var realmAccessClaim = user.FindFirst("realm_access")?.Value;
    if (!string.IsNullOrEmpty(realmAccessClaim))
    {
        try
        {
            var json = System.Text.Json.JsonDocument.Parse(realmAccessClaim);
            if (json.RootElement.TryGetProperty("roles", out var rolesElement))
            {
                return rolesElement.EnumerateArray()
                    .Select(role => role.GetString())
                    .Where(role => !string.IsNullOrEmpty(role))
                    .Cast<string>();
            }
        }
        catch
        {
            // Fallback to standard role claims
        }
    }

    // Fallback to standard role claims
    return user.FindAll(System.Security.Claims.ClaimTypes.Role)?.Select(c => c.Value) ?? Enumerable.Empty<string>();
}