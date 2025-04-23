using ProjektGrupowy.API.Utils.Constants;
using System.Security.Claims;

namespace ProjektGrupowy.API.Services.Impl
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public bool IsAdmin => _httpContextAccessor.HttpContext?.User?.IsInRole(RoleConstants.Admin) ?? false;
    }

}
