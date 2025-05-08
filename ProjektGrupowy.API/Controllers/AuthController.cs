using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.DTOs.Auth;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils.Constants;
using System.IdentityModel.Tokens.Jwt;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class AuthController(
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    AppDbContext context,
    ITokenService tokenService,
    IConfiguration configuration) : ControllerBase
{
    private readonly string JwtCookieName = configuration["JWT:JwtCookieName"];
    private readonly string RefreshCookieName = configuration["JWT:RefreshCookieName"];
    private readonly bool IsProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
    private readonly string ScientistCreateToken = configuration["JWT:ScientistCreateToken"];

    [HttpPost("Register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var userExists = await userManager.FindByNameAsync(model.UserName);
        if (userExists != null)
            return StatusCode(403, new { status = "Error", message = "User already exists!" });

        if (model.Role == RoleConstants.Admin)
            return StatusCode(403, new { status = "Error", message = "Creating users with Admin role is not allowed." });

        if (model.Role == RoleConstants.Scientist && string.IsNullOrEmpty(model.ScientistCreateToken))
            return StatusCode(403, new { status = "Error", message = "Creating Scientist role requires a valid token." });

        if (model.Role == RoleConstants.Scientist && model.ScientistCreateToken != ScientistCreateToken)
            return StatusCode(403, new { status = "Error", message = "Invalid token for creating Scientist role." });

        var user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            RegistrationDate = DateTime.UtcNow
        };

        await using var transaction = await context.Database.BeginTransactionAsync();
        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return StatusCode(500, new { status = "Error", message = "User creation failed!", Errors = errors });
        }

        if (!await roleManager.RoleExistsAsync(model.Role))
            return StatusCode(500, new { status = "Error", message = "Role does not exist!" });

        var roleResult = await userManager.AddToRoleAsync(user, model.Role);
        if (!roleResult.Succeeded)
            return StatusCode(500, new { status = "Error", message = "Failed to assign role to user." });

        await transaction.CommitAsync();

        return Ok(new { status = "Success", message = "User created and assigned to role successfully!" });
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await userManager.FindByNameAsync(model.UserName);
        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized(new { status = "Error", message = "Invalid UserName or password!" });

        var accessToken = await tokenService.CreateAccessTokenAsync(user);
        var refreshToken = tokenService.GenerateRefreshToken(user.Id);

        var oldTokens = await context.RefreshTokens
            .Where(rt => rt.UserId == user.Id && !rt.IsUsed && !rt.IsRevoked)
            .ToListAsync();

        foreach (var oldToken in oldTokens)
        {
            oldToken.IsRevoked = true;
            context.RefreshTokens.Update(oldToken);
        }

        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();

        SetCookie(JwtCookieName, new JwtSecurityTokenHandler().WriteToken(accessToken));
        SetCookie(RefreshCookieName, refreshToken.Token);

        return Ok(new
        {
            message = "Login successful!",
            roles = await userManager.GetRolesAsync(user),
        });
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshTokenFromClient = Request.Cookies[RefreshCookieName];

        if (!string.IsNullOrEmpty(refreshTokenFromClient))
        {
            var storedToken = await context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshTokenFromClient);
            if (storedToken != null)
            {
                storedToken.IsRevoked = true;
                context.RefreshTokens.Update(storedToken);
                await context.SaveChangesAsync();
            }
        }

        Response.Cookies.Delete(JwtCookieName);
        Response.Cookies.Delete(RefreshCookieName);

        return Ok(new { message = "Logout successful!" });
    }


    [HttpPost("RefreshToken")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken()
    {
        var accessToken = Request.Cookies[JwtCookieName];
        var refreshTokenFromClient = Request.Cookies[RefreshCookieName];

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshTokenFromClient))
            return Unauthorized(new { status = "Error", message = "Tokens not provided." });

        var principal = tokenService.ValidateExpiredToken(accessToken);
        if (principal == null || principal.Identity?.Name == null)
            return Unauthorized(new { status = "Error", message = "Invalid access token." });

        var user = await userManager.FindByNameAsync(principal.Identity.Name);
        if (user == null)
            return Unauthorized(new { status = "Error", message = "User not found." });

        var storedRefreshToken = await context.RefreshTokens
            .Where(rt => rt.Token == refreshTokenFromClient && rt.UserId == user.Id)
            .FirstOrDefaultAsync();

        if (storedRefreshToken == null || storedRefreshToken.IsUsed || storedRefreshToken.IsRevoked || storedRefreshToken.Expires < DateTime.UtcNow)
            return Unauthorized(new { status = "Error", message = "Invalid or expired refresh token." });

        storedRefreshToken.IsUsed = true;
        context.RefreshTokens.Update(storedRefreshToken);

        var newAccessToken = await tokenService.CreateAccessTokenAsync(user);
        var newRefreshToken = tokenService.GenerateRefreshToken(user.Id);
        context.RefreshTokens.Add(newRefreshToken);

        await context.SaveChangesAsync();

        SetCookie(JwtCookieName, new JwtSecurityTokenHandler().WriteToken(newAccessToken));
        SetCookie(RefreshCookieName, newRefreshToken.Token);

        return Ok(new
        {
            message = "Token refreshed successfully!",
        });
    }

    [HttpGet("CheckAuth")]
    public async Task<IActionResult> CheckAuth()
    {
        var user = await userManager.GetUserAsync(User);

        var roles = await userManager.GetRolesAsync(user);

        return Ok(new
        {
            isAuthenticated = true,
            roles
        });
    }

    private void SetCookie(string name, string value)
    {
        var expiresInDays = int.TryParse(configuration["JWT:RefreshTokenExpiresInDays"], out var exp) 
            ? exp 
            : 7;
        Response.Cookies.Append(name, value, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }
}
