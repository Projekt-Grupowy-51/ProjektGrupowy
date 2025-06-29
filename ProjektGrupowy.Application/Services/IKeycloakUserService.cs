using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Application.Services;

/// <summary>
/// Service for user management compatible with Keycloak
/// This replaces ASP.NET Identity UserManager functionality
/// </summary>
public interface IKeycloakUserService
{
    Task<User?> FindByIdAsync(string userId);
    Task<User?> FindByNameAsync(string userName);
    Task<IEnumerable<string>> GetRolesAsync(User user);
    Task<bool> CheckUserExistsAsync(string userId);
    Task<Optional<IEnumerable<User>>> GetAllUsersAsync();
}
