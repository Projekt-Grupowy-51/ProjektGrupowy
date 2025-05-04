using System.Security.Claims;

namespace ProjektGrupowy.API.Services
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        bool IsAdmin { get; }
        ClaimsPrincipal User { get; }
    }
}
