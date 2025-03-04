using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories.Impl;

public class ScientistRepository(AppDbContext context, ILogger<ScientistRepository> logger) : IScientistRepository
{
    public async Task<Optional<Scientist>> AddScientistAsync(Scientist scientist)
    {
        try 
        {
            var s = context.Scientists.Add(scientist);
            await context.SaveChangesAsync();
            return Optional<Scientist>.Success(s.Entity);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add scientist");
            return Optional<Scientist>.Failure(e.Message);
        }
    }

    public async Task DeleteScientistAsync(Scientist scientist)
    {
        try
        {
            context.Scientists.Remove(scientist);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete scientist");
        }
    }

    public async Task<Optional<Scientist>> GetScientistAsync(int id)
    {
        try 
        {
            var scientist = await context.Scientists.FirstOrDefaultAsync(s => s.Id == id);
            return scientist is null
                ? Optional<Scientist>.Failure("Scientist not found")
                : Optional<Scientist>.Success(scientist);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get scientist");
            return Optional<Scientist>.Failure(e.Message);
        }
    }

    public async Task<Optional<IEnumerable<Scientist>>> GetScientistsAsync()
    {
        try
        {
            var scientists = await context.Scientists.ToListAsync();
            return Optional<IEnumerable<Scientist>>.Success(scientists);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get scientists");
            return Optional<IEnumerable<Scientist>>.Failure(e.Message);
        }
    }

    public async Task<Optional<Scientist>> UpdateScientistAsync(Scientist scientist)
    {
        try
        {
            context.Scientists.Update(scientist);
            await context.SaveChangesAsync();
            return Optional<Scientist>.Success(scientist);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update scientist");
            return Optional<Scientist>.Failure(e.Message);
        }
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync() => await context.Database.BeginTransactionAsync();

    public async Task<Optional<Scientist>> GetScientistByUserIdAsync(string userId)
    {
        try
        {
            var scientist = await context.Scientists.FirstOrDefaultAsync(s => s.User.Id == userId);
            return scientist is null
                ? Optional<Scientist>.Failure("Scientist not found")
                : Optional<Scientist>.Success(scientist);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get scientist by user id");
            return Optional<Scientist>.Failure(e.Message);
        }
    }
}
