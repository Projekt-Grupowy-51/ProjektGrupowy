using Microsoft.IdentityModel.Tokens;
using ProjektGrupowy.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProjektGrupowy.API.Services
{
    public interface ITokenService
    {
        Task<JwtSecurityToken> CreateAccessTokenAsync(User user);
        RefreshToken GenerateRefreshToken(string userId);
        ClaimsPrincipal? ValidateExpiredToken(string token);
    }
}
