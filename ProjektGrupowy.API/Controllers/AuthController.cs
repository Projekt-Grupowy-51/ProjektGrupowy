using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.DTOs.Auth;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))] // ValidateModelStateFilter is a custom filter to validate model state
[Authorize]
public class AuthController(
    UserManager<User> userManager, 
    RoleManager<IdentityRole> roleManager,
    AppDbContext context, 
    IConfiguration configuration) : ControllerBase
{
    private readonly string JwtCookieName = configuration["JWT:JwtCookieName"];

    [HttpPost("Register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var userExists = await userManager.FindByNameAsync(model.UserName);
        if (userExists != null)
            return StatusCode(500, new { status = "Error", message = "User already exists!" });

        if (model.Role == RoleConstants.Admin)
        {
            return StatusCode(403, new { status = "Error", message = "Creating users with Admin role is not allowed." });
        }

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

        var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

        var userRoles = await userManager.GetRolesAsync(user);
        authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        var token = GenerateJwtToken(authClaims);

        // Ustawienie tokenu jako ciasteczko HTTP Only
        Response.Cookies.Append(JwtCookieName, new JwtSecurityTokenHandler().WriteToken(token), new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // Używaj tylko w przypadku HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = token.ValidTo
        });

        return Ok(new
        {
            Message = "Login successful!",
            ExpiresAt = token.ValidTo // Zwróć czas wygaśnięcia tokena
        });
    }

    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        // Usuń ciasteczko JWT
        Response.Cookies.Delete(JwtCookieName);
        return Ok(new { Message = "Logout successful!" });
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken()
    {
        var jwtToken = Request.Cookies[JwtCookieName];
        if (string.IsNullOrEmpty(jwtToken))
            return Unauthorized(new { status = "Error", message = "No token provided." });

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetTokenValidationParameters();

        var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out var validatedToken);

        if (!(validatedToken is JwtSecurityToken jwtSecurityToken) ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            return Unauthorized(new { status = "Error", message = "Invalid token." });
        }

        var user = await userManager.FindByNameAsync(principal.Identity.Name);
        if (user == null)
            return Unauthorized(new { status = "Error", message = "User not found." });

        var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

        var userRoles = await userManager.GetRolesAsync(user);
        authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var newToken = GenerateJwtToken(authClaims);

        Response.Cookies.Append(JwtCookieName, new JwtSecurityTokenHandler().WriteToken(newToken), new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = newToken.ValidTo
        });

        return Ok(new
        {
            message = "Token refreshed successfully!",
            ExpiresAt = newToken.ValidTo
        });
    }

    [HttpGet("VerifyToken")]
    public async Task<IActionResult> VerifyToken()
    {
        var jwtToken = Request.Cookies[JwtCookieName];
        if (string.IsNullOrEmpty(jwtToken))
            return Unauthorized(new { IsAuthenticated = false });

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetTokenValidationParameters();

        var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out var validatedToken);

        if (!(validatedToken is JwtSecurityToken jwtSecurityToken) ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            return Unauthorized(new { IsAuthenticated = false });
        }

        var user = await userManager.FindByNameAsync(principal.Identity.Name);
        if (user == null)
            return Unauthorized(new { IsAuthenticated = false });

        return Ok(new
        {
            IsAuthenticated = true,
            Username = user.UserName,
            Roles = await userManager.GetRolesAsync(user)
        });
    }

    private TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JWT:ValidIssuer"],
            ValidAudience = configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
            ClockSkew = TimeSpan.Zero // Brak tolerancji dla czasu wygaśnięcia
        };
    }

    private JwtSecurityToken GenerateJwtToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: configuration["JWT:ValidIssuer"],
            audience: configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddMinutes(6),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}
