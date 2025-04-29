using ProjektGrupowy.API.Utils.Constants;
using System.Security.Claims;

namespace ProjektGrupowy.API.Services.Impl
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public string UserId => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public bool IsAdmin => httpContextAccessor.HttpContext?.User?.IsInRole(RoleConstants.Admin) ?? false;
    }
}
