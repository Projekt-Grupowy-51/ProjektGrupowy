using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Infrastructure.Data;

namespace ProjektGrupowy.Infrastructure.Repositories.Impl;

public class KeycloakUserRepository : IKeycloakUserRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<KeycloakUserRepository> _logger;

    public KeycloakUserRepository(AppDbContext context, ILogger<KeycloakUserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> FindByIdAsync(string userId)
    {
        try
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding user by ID: {UserId}", userId);
            return null;
        }
    }

    public async Task<User?> FindByNameAsync(string userName)
    {
        try
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding user by username: {UserName}", userName);
            return null;
        }
    }

    public async Task<bool> CheckUserExistsAsync(string userId)
    {
        try
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user exists: {UserId}", userId);
            return false;
        }
    }

    public async Task<Optional<IEnumerable<User>>> GetAllUsersAsync()
    {
        try
        {
            var users = await _context.Users.ToListAsync();
            return Optional<IEnumerable<User>>.Success(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            return Optional<IEnumerable<User>>.Failure("Failed to retrieve users");
        }
    }
}
