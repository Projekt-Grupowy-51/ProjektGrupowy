using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Interfaces.Repositories;

public interface IKeycloakUserRepository
{
    Task<User> FindByIdAsync(string userId);
}
