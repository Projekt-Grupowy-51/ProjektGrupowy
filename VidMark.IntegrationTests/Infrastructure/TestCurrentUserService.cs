using Microsoft.AspNetCore.Http;
using VidMark.Application.Services;
using System.Security.Claims;

namespace VidMark.IntegrationTests.Infrastructure;

public class TestCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TestCurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User
                ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userId ?? "test-user-id";
        }
    }

    public string? UserName
    {
        get
        {
            var userName = _httpContextAccessor.HttpContext?.User
                ?.FindFirst(ClaimTypes.Name)?.Value;

            return userName ?? "test-user";
        }
    }

    public IEnumerable<string> Roles
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return Enumerable.Empty<string>();

            return GetUserRoles(user);
        }
    }

    public ClaimsPrincipal User
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null && user.Identity?.IsAuthenticated == true)
            {
                return user;
            }

            // Return a default authenticated user for testing
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, UserId),
                new Claim(ClaimTypes.Name, UserName ?? "test-user")
            };

            // Add roles if available
            foreach (var role in Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        }
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsAdmin
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return false;

            // Check both standard role claims and Keycloak realm_access
            var roles = GetUserRoles(user);
            return roles.Contains("Admin") || roles.Contains("admin");
        }
    }

    private IEnumerable<string> GetUserRoles(ClaimsPrincipal user)
    {
        return user.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }
}
