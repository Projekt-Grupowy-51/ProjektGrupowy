using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Infrastructure.Services;

namespace ProjektGrupowy.API.Controllers;

[Route("api/users")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class UserController(ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet("profile")]
    public IActionResult GetUserProfile()
    {
        return Ok(new
        {
            userId = currentUserService.UserId,
            username = currentUserService.UserName,
            roles = currentUserService.Roles,
            isAdmin = currentUserService.IsAdmin,
        });
    }

    [HttpGet("check-auth")]
    public IActionResult CheckAuth()
    {
        return Ok(new
        {
            isAuthenticated = true,
            userId = currentUserService.UserId,
            username = currentUserService.UserName,
            roles = currentUserService.Roles
        });
    }
}
