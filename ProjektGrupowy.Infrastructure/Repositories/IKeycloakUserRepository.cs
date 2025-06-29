using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

/// <summary>
/// Repository for user data access compatible with Keycloak
/// </summary>
public interface IKeycloakUserRepository
{
    Task<User?> FindByIdAsync(string userId);
    Task<User?> FindByNameAsync(string userName);
    Task<bool> CheckUserExistsAsync(string userId);
    Task<Optional<IEnumerable<User>>> GetAllUsersAsync();
}
