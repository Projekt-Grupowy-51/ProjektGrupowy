using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.DTOs.Auth;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))] // ValidateModelStateFilter is a custom filter to validate model state
public class AuthController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
                                AppDbContext context, IConfiguration configuration) : ControllerBase
{
    private readonly string JwtCookieName = "jwt";

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var userExists = await userManager.FindByNameAsync(model.UserName);
        if (userExists != null)
            return StatusCode(500, new { Status = "Error", Message = "User already exists!" });

        var user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return StatusCode(500, new { Status = "Error", Message = "User creation failed!", Errors = errors });
            }

            if (!await roleManager.RoleExistsAsync(model.Role.ToString()))
                return StatusCode(500, new { Status = "Error", Message = "Role does not exist!" });

            var roleResult = await userManager.AddToRoleAsync(user, model.Role.ToString());
            if (!roleResult.Succeeded)
                return StatusCode(500, new { Status = "Error", Message = "Failed to assign role to user." });

            await transaction.CommitAsync();

            return Ok(new { Status = "Success", Message = "User created and assigned to role successfully!" });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { Status = "Error", Message = "An error occurred during registration.", Detail = ex.Message });
        }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await userManager.FindByNameAsync(model.UserName);
        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized(new { Status = "Error", Message = "Invalid UserName or password!" });

        var authClaims = new List<Claim>
    {
        new(ClaimTypes.Name, user.UserName),
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
            return Unauthorized(new { Status = "Error", Message = "No token provided." });

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetTokenValidationParameters();

        try
        {
            // Walidacja tokenu
            var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out var validatedToken);

            // Sprawdzenie czy token jest JWT
            if (!(validatedToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return Unauthorized(new { Status = "Error", Message = "Invalid token." });
            }

            // Pobranie użytkownika
            var user = await userManager.FindByNameAsync(principal.Identity.Name);
            if (user == null)
                return Unauthorized(new { Status = "Error", Message = "User not found." });

            // Generowanie nowych claimów
            var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

            var userRoles = await userManager.GetRolesAsync(user);
            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Generowanie nowego tokenu
            var newToken = GenerateJwtToken(authClaims);

            // Aktualizacja ciasteczka
            Response.Cookies.Append(JwtCookieName, new JwtSecurityTokenHandler().WriteToken(newToken), new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = newToken.ValidTo
            });

            return Ok(new
            {
                Message = "Token refreshed successfully!",
                ExpiresAt = newToken.ValidTo
            });
        }
        catch (SecurityTokenExpiredException)
        {
            return Unauthorized(new { Status = "Error", Message = "Token expired." });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { Status = "Error", Message = "Invalid token.", Details = ex.Message });
        }
    }

    [HttpGet("VerifyToken")]
    public async Task<IActionResult> VerifyToken()
    {
        var jwtToken = Request.Cookies[JwtCookieName];
        if (string.IsNullOrEmpty(jwtToken))
            return Unauthorized(new { IsAuthenticated = false });

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetTokenValidationParameters();

        try
        {
            // Walidacja tokenu
            var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out var validatedToken);

            // Dodatkowe sprawdzenie algorytmu podpisu
            if (!(validatedToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return Unauthorized(new { IsAuthenticated = false });
            }

            // Pobranie użytkownika
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
        catch (Exception)
        {
            return Unauthorized(new { IsAuthenticated = false });
        }
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
