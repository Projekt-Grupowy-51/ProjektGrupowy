using System.Security.Claims;

namespace ProjektGrupowy.Domain.Services;
public interface ICurrentUserService
{
    string UserId { get; }
    bool IsAdmin { get; }
    ClaimsPrincipal User { get; }
}
