using System.Security.Claims;

namespace ProjektGrupowy.Infrastructure.Services;

public interface ICurrentUserService
{
    string UserId { get; }
    string? UserName { get; }
    IEnumerable<string> Roles { get; }
    bool IsAdmin { get; }
    ClaimsPrincipal User { get; }
    bool IsAuthenticated { get; }
}
