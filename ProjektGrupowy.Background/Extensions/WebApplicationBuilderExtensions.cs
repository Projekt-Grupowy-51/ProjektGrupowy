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

namespace ProjektGrupowy.Background.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder RegisterDataSources(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options
                .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
                .UseLazyLoadingProxies()
        );

        return builder;
    }

    public static WebApplicationBuilder RegisterHttpServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.Services.AddScoped<IHttpMessageClient, HttpMessageClient>();
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

    public static WebApplicationBuilder RegisterBasicHangfire(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfire(config => config.UseSerilogLogProvider());

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddHangfire(config => config.UseMemoryStorage());
        }
        else
        {
            builder.Services.AddHangfire(config =>
            {
                config.UseSimpleAssemblyNameTypeSerializer();
                config.UseRecommendedSerializerSettings();

                var hangfireConnectionString = builder.Configuration.GetConnectionString("HangfireConnection");
                config.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(hangfireConnectionString));
            });
        }
        
        return builder;
    }

    public static WebApplicationBuilder RegisterHangfireServer(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfireServer(options =>
        {
            options.WorkerCount = 4;
        });
        
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

    public static WebApplicationBuilder RegisterScopedRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IKeycloakUserRepository, KeycloakUserRepository>();
        builder.Services.AddScoped<IAssignedLabelRepository, AssignedLabelRepository>();
        builder.Services.AddScoped<ILabelRepository, LabelRepository>();
        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
        builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
        builder.Services.AddScoped<ISubjectVideoGroupAssignmentRepository, SubjectVideoGroupAssignmentRepository>();
        builder.Services.AddScoped<IVideoGroupRepository, VideoGroupRepository>();
        builder.Services.AddScoped<IVideoRepository, VideoRepository>();
        builder.Services.AddScoped<IProjectAccessCodeRepository, ProjectAccessCodeRepository>();
        builder.Services.AddScoped<IProjectReportRepository, ProjectReportRepository>();
        builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
        
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddScoped<IAuthorizationHandler, CustomAuthorizationHandler>();
        
        return builder;
    }

    public static WebApplicationBuilder RegisterScopedServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserIdProvider, CustomUserIdProvider>();
        builder.Services.AddScoped<IMessageService, HttpMessageService>();
        builder.Services.AddScoped<IKeycloakUserService, KeycloakUserService>();
        builder.Services.AddScoped<IAssignedLabelService, AssignedLabelService>();
        builder.Services.AddScoped<ILabelService, LabelService>();
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<ISubjectService, SubjectService>();
        builder.Services.AddScoped<ISubjectVideoGroupAssignmentService, SubjectVideoGroupAssignmentService>();
        builder.Services.AddScoped<IVideoGroupService, VideoGroupService>();
        builder.Services.AddScoped<IVideoService, VideoService>();
        builder.Services.AddScoped<IProjectAccessCodeService, ProjectAccessCodeService>();
        builder.Services.AddScoped<IReportGenerator, ReportGenerator>();
        builder.Services.AddScoped<IProjectReportService, ProjectReportService>();
        
        return builder;
    }

    public static WebApplicationBuilder RegisterMapper(this WebApplicationBuilder builder)
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AccessCodeMap>();
            cfg.AddProfile<AssignedLabelMap>();
            cfg.AddProfile<LabelerMap>();
            cfg.AddProfile<LabelMap>();
            cfg.AddProfile<ProjectMap>();
            cfg.AddProfile<ReportMap>();
            cfg.AddProfile<SubjectMap>();
            cfg.AddProfile<SubjectVideoGroupAssignmentMap>();
            cfg.AddProfile<VideoGroupMap>();
            cfg.AddProfile<VideoMap>();
        }, new SerilogLoggerFactory());
        var mapper = mapperConfig.CreateMapper();
        builder.Services.AddSingleton(mapper);
        
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