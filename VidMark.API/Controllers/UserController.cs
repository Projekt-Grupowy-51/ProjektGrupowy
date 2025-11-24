using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VidMark.API.DTOs.User;
using VidMark.API.Filters;
using VidMark.Application.Services;

namespace VidMark.API.Controllers;

/// <summary>
/// Controller for managing user-related operations, such as retrieving the current user's profile and checking authentication status.
/// </summary>
/// <param name="currentUserService"></param>
[Route("api/users")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class UserController(ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// Get the profile of the currently authenticated user.
    /// </summary>
    /// <returns>The profile information of the current user.</returns>
    [HttpGet("profile")]
    public ActionResult<SimpleUserDto> GetUserProfile()
    {
        return Ok(new SimpleUserDto(currentUserService.UserId,
            currentUserService.UserName,
            currentUserService.Roles,
            currentUserService.IsAdmin));
    }

    /// <summary>
    /// Check if the user is authenticated and retrieve their basic information.
    /// </summary>
    /// <returns></returns>
    [HttpGet("check-auth")]
    public ActionResult<SimpleUserDto> CheckAuth()
    {
        return Ok(new SimpleUserDto(currentUserService.UserId,
            currentUserService.UserName,
            currentUserService.Roles,
            currentUserService.IsAdmin));
    }
}