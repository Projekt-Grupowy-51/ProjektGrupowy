using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories.Impl;

public class LabelerRepository(AppDbContext context, ILogger<LabelerRepository> logger) : ILabelerRepository
{
    public async Task<Optional<IEnumerable<Labeler>>> GetLabelersAsync()
    {
        try
        {
            var labelers = await context.Labelers.ToListAsync();
            return Optional<IEnumerable<Labeler>>.Success(labelers);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting labelers");
            return Optional<IEnumerable<Labeler>>.Failure(e.Message);
        }
    }

    public async Task<Optional<Labeler>> GetLabelerAsync(int id)
    {
        try
        {
            var labeler = await context.Labelers.FirstOrDefaultAsync(l => l.Id == id);
            return labeler is null
                ? Optional<Labeler>.Failure("Labeler not found")
                : Optional<Labeler>.Success(labeler);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting labeler");
            return Optional<Labeler>.Failure(e.Message);
        }
    }

    public async Task<Optional<Labeler>> AddLabelerAsync(Labeler labeler)
    {
        try
        {
            var l = await context.Labelers.AddAsync(labeler);
            await context.SaveChangesAsync();

            return Optional<Labeler>.Success(l.Entity);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while adding labeler");
            return Optional<Labeler>.Failure(e.Message);
        }
    }

    public async Task<Optional<Labeler>> UpdateLabelerAsync(Labeler labeler)
    {
        try
        {
            context.Labelers.Update(labeler);
            await context.SaveChangesAsync();

            return Optional<Labeler>.Success(labeler);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while updating labeler");
            return Optional<Labeler>.Failure(e.Message);
        }
    }

    public async Task DeleteLabelerAsync(Labeler labeler)
    {
        try
        {
            context.Labelers.Remove(labeler);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while deleting labeler");
        }
    }
}