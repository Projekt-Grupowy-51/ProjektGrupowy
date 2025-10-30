using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Infrastructure.Persistance.Repositories;

public class KeycloakUserRepository(IReadWriteContext context) : IKeycloakUserRepository
{
    public Task<User> FindByIdAsync(string userId)
    {
        return context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }
}
