using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using VidMark.Domain.Utils.Constants;
using Serilog;
using System.Text.Json;

namespace VidMark.API.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticationConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var keycloakAuthority = configuration["Keycloak:Authority"];
        var keycloakAudience = configuration["Keycloak:Audience"];
        var keycloakIssuer = configuration["Keycloak:Issuer"];

        if (!environment.IsDevelopment())
        {
            if (string.IsNullOrEmpty(keycloakAuthority))
            {
                throw new ArgumentNullException("Keycloak:Authority is missing in configuration.");
            }

            if (string.IsNullOrEmpty(keycloakAudience))
            {
                throw new ArgumentNullException("Keycloak:Audience is missing in configuration.");
            }
        }

        _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = keycloakAuthority;
                options.Audience = keycloakAudience;
                options.RequireHttpsMetadata = configuration.GetValue("Keycloak:RequireHttpsMetadata", false);

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
                        // Support SignalR - read token from query string
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/signalr"))
                        {
                            context.Token = accessToken;
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
                        Log.Information("Token validated for user: {Username}",
                            context.Principal?.Identity?.Name);
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }

    public static IServiceCollection AddAuthorizationConfiguration(this IServiceCollection services)
    {
        _ = services.AddAuthorization(options =>
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

        return services;
    }

    private static IEnumerable<string> GetKeycloakRoles(System.Security.Claims.ClaimsPrincipal user)
    {
        var roles = new List<string>();

        // Try to get roles from realm_access claim
        var realmAccessClaim = user.FindFirst("realm_access")?.Value;
        if (!string.IsNullOrEmpty(realmAccessClaim))
        {
            try
            {
                var json = JsonDocument.Parse(realmAccessClaim);
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
        roles.AddRange(user.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value));

        var userRoleClaim = user.FindFirst("userRole")?.Value;
        if (!string.IsNullOrEmpty(userRoleClaim))
        {
            roles.Add(userRoleClaim);
        }

        return roles.Distinct();
    }
}
