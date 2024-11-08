using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.DTOs.Auth;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))] // ValidateModelStateFilter is a custom filter to validate model state
public class AuthController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
                                AppDbContext context, IConfiguration configuration ) : ControllerBase
{
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

        using (var transaction = await context.Database.BeginTransactionAsync())
        {
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
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await userManager.FindByNameAsync(model.UserName);
        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized(new { Status = "Error", Message = "Invalid UserName or password!" });

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var userRoles = await userManager.GetRolesAsync(user);
        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = GenerateJwtToken(authClaims);

        return Ok(new
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = token.ValidTo
        });
    }

    private JwtSecurityToken GenerateJwtToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: configuration["JWT:ValidIssuer"],
            audience: configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}
