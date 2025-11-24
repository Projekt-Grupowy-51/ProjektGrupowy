using VidMark.Domain.Models;

namespace VidMark.Application.Interfaces.Repositories;

public interface IKeycloakUserRepository
{
    Task<User> FindByIdAsync(string userId);
}
