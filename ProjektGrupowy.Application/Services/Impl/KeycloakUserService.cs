using Microsoft.AspNetCore.Http;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Infrastructure.Repositories;
using System.Security.Claims;

namespace ProjektGrupowy.Application.Services.Impl;

public class KeycloakUserService : IKeycloakUserService
{
    private readonly IKeycloakUserRepository _keycloakUserRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public KeycloakUserService(IKeycloakUserRepository keycloakUserRepository, IHttpContextAccessor httpContextAccessor)
    {
        _keycloakUserRepository = keycloakUserRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<User?> FindByIdAsync(string userId)
    {
        return await _keycloakUserRepository.FindByIdAsync(userId);
    }

    public async Task<User?> FindByNameAsync(string userName)
    {
        return await _keycloakUserRepository.FindByNameAsync(userName);
    }

    public async Task<bool> CheckUserExistsAsync(string userId)
    {
        return await _keycloakUserRepository.CheckUserExistsAsync(userId);
    }

    public async Task<Optional<IEnumerable<User>>> GetAllUsersAsync()
    {
        return await _keycloakUserRepository.GetAllUsersAsync();
    }

    public async Task<IEnumerable<string>> GetRolesAsync(User user)
    {
        // For Keycloak, roles come from JWT claims, not database
        // We get them from the current user context
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            return GetKeycloakRoles(httpContext.User);
        }

        return new List<string>();
    }

    private static IEnumerable<string> GetKeycloakRoles(ClaimsPrincipal user)
    {
        // Try to get roles from realm_access claim (Keycloak standard)
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
                        .Cast<string>()
                        .ToList();
                }
            }
            catch (System.Text.Json.JsonException)
            {
                // Fall through to backup method
            }
        }

        // Fallback: try to get roles from standard role claims
        return user.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .Where(role => !string.IsNullOrEmpty(role))
            .ToList();
    }
}
