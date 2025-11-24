using Microsoft.EntityFrameworkCore;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Domain.Models;

namespace VidMark.Infrastructure.Persistance.Repositories;

public class KeycloakUserRepository(IReadWriteContext context) : IKeycloakUserRepository
{
    public Task<User> FindByIdAsync(string userId)
    {
        return context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }
}
