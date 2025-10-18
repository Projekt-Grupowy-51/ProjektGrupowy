using System.Text.Json.Serialization;
using AutoMapper;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjektGrupowy.API.Authentication;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Http;
using ProjektGrupowy.Application.Mapper;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Application.Services.Background;
using ProjektGrupowy.Application.Services.Background.Impl;
using ProjektGrupowy.Application.Services.Impl;
using ProjektGrupowy.Application.SignalR;
using ProjektGrupowy.Domain.Utils.Constants;
using ProjektGrupowy.Infrastructure.Data;
using ProjektGrupowy.Infrastructure.Repositories;
using ProjektGrupowy.Infrastructure.Repositories.Impl;
using Serilog;
using Serilog.Extensions.Logging;

namespace ProjektGrupowy.Notifications.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder RegisterHttpServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.Services.AddHttpContextAccessor();
        
        return builder;
    }

    public static WebApplicationBuilder ConfigureJson(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
        
        return builder;
    }

    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .WriteTo.Console()
            .WriteTo.File("Logs/internal_api.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        
        builder.Host.UseSerilog();
        
        return builder;
    }

    public static WebApplicationBuilder ConfigureHealthChecks(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks();
        return builder;
    }
    
    public static WebApplicationBuilder RegisterCors(this WebApplicationBuilder builder)
    {
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
        
        return builder;
    }

    public static WebApplicationBuilder ApplySwaggerConfig(this WebApplicationBuilder builder)
    {
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

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        
        return builder;
    }
    
    public static WebApplicationBuilder ApplyKestrelConfig(this WebApplicationBuilder builder)
    {
        var maxBodySize = int.TryParse(builder.Configuration["Limits:MaxBodySizeMb"], out var parsedMaxBodySize)
            ? parsedMaxBodySize * 1024 * 1024
            : 500 * 1024 * 1024; // fallback

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = maxBodySize;
        });
    
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        });
        
        return builder;
    }
    
    public static WebApplicationBuilder RegisterScopedServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddScoped<IAuthorizationHandler, CustomAuthorizationHandler>();
        
        return builder;
    }

    public static WebApplicationBuilder RegisterSignalR(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IConnectedClientManager, ConnectedClientManager>();
        builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
        
        builder.Services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            options.HandshakeTimeout = TimeSpan.FromSeconds(15);
        });
        builder.Services.AddSingleton<IMessageService, SignalRMessageService>();

        return builder;
    }

    public static WebApplicationBuilder RegisterFilters(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ValidateModelStateFilter>();
        builder.Services.AddScoped<NonSuccessGetFilter>();
        
        return builder;
    }

    public static WebApplicationBuilder RegisterAuth(this WebApplicationBuilder builder)
    {
        var keycloakAuthority = builder.Configuration["Keycloak:Authority"];
        var keycloakAudience = builder.Configuration["Keycloak:Audience"];
        var keycloakIssuer = builder.Configuration["Keycloak:Issuer"];
        
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
                    options.RequireHttpsMetadata =
                        builder.Configuration.GetValue("Keycloak:RequireHttpsMetadata", false);

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
        
        return builder;
    }
    private static IEnumerable<string> GetKeycloakRoles(System.Security.Claims.ClaimsPrincipal user)
    {
        var roles = new List<string>();

        // Try to get roles from realm_access.claim
        var realmAccessClaim = user.FindFirst("realm_access")?.Value;
        if (!string.IsNullOrEmpty(realmAccessClaim))
        {
            try
            {
                var json = System.Text.Json.JsonDocument.Parse(realmAccessClaim);
                if (json.RootElement.TryGetProperty("roles", out var rolesElement))
                {
                    roles.AddRange(
                        rolesElement.EnumerateArray()
                            .Select(role => role.GetString())
                            .Where(role => !string.IsNullOrEmpty(role))
                            .Cast<string>()
                    );
                }
            }
            catch
            {
                // fallback silently
            }
        }

        // Add standard role claims (ClaimTypes.Role)
        roles.AddRange(
            user.FindAll(System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
        );
    
        var userRoleClaim = user.FindFirst("userRole")?.Value;
        if (!string.IsNullOrEmpty(userRoleClaim))
        {
            roles.Add(userRoleClaim);
        }

        return roles.Distinct();
    }
}