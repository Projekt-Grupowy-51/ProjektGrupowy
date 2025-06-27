using Microsoft.AspNetCore.Http;
using ProjektGrupowy.Domain.Utils.Constants;
using System.Security.Claims;

namespace ProjektGrupowy.Domain.Services.Impl;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string UserId => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsAdmin => httpContextAccessor.HttpContext?.User?.IsInRole(RoleConstants.Admin) ?? false;
    public ClaimsPrincipal User => httpContextAccessor.HttpContext?.User;
}