using Microsoft.IdentityModel.Tokens;
using ProjektGrupowy.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProjektGrupowy.Application.Services
{
    public interface ITokenService
    {
        Task<JwtSecurityToken> CreateAccessTokenAsync(User user);
        RefreshToken GenerateRefreshToken(string userId);
        ClaimsPrincipal? ValidateExpiredToken(string token);
    }
}
