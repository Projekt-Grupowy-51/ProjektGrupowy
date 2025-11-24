using Microsoft.AspNetCore.Http;
using VidMark.Domain.Utils.Constants;
using System.Security.Claims;

namespace VidMark.Application.Services.Impl;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId => User?.FindFirst("sub")?.Value ?? 
                           User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                           string.Empty;

    public string? UserName => User?.FindFirst("preferred_username")?.Value ?? 
                              User?.FindFirst(ClaimTypes.Name)?.Value ??
                              User?.FindFirst("name")?.Value;

    public IEnumerable<string> Roles
    {
        get
        {
            // Try to get roles from realm_access claim (Keycloak standard)
            var realmAccessClaim = User?.FindFirst("realm_access")?.Value;
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
                    // Continue to fallback
                }
            }

            // Try resource_access claim
            var resourceAccessClaim = User?.FindFirst("resource_access")?.Value;
            if (!string.IsNullOrEmpty(resourceAccessClaim))
            {
                try
                {
                    var json = System.Text.Json.JsonDocument.Parse(resourceAccessClaim);
                    if (json.RootElement.TryGetProperty("VidMark-api", out var clientElement) &&
                        clientElement.TryGetProperty("roles", out var rolesElement))
                    {
                        return rolesElement.EnumerateArray()
                            .Select(role => role.GetString())
                            .Where(role => !string.IsNullOrEmpty(role))
                            .Cast<string>();
                    }
                }
                catch
                {
                    // Continue to fallback
                }
            }

            // Fallback to standard role claims
            return User?.FindAll(ClaimTypes.Role)?.Select(c => c.Value) ?? Enumerable.Empty<string>();
        }
    }

    public bool IsAdmin => Roles.Contains(RoleConstants.Admin);

    public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
