using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories.Impl;

public class AssignedLabelRepository(AppDbContext context, ILogger<AssignedLabelRepository> logger) : IAssignedLabelRepository
{
    public async Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsAsync()
    {
        try
        {
            var assignedLabels = await context.AssignedLabels.ToListAsync();
            return Optional<IEnumerable<AssignedLabel>>.Success(assignedLabels);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting assigned labels");
            return Optional<IEnumerable<AssignedLabel>>.Failure(e.Message);
        }
    }

    public async Task<Optional<AssignedLabel>> GetAssignedLabelAsync(int id)
    {
        try
        {
            var assignedLabel = await context.AssignedLabels.FirstOrDefaultAsync(a => a.Id == id);
            return assignedLabel is null
                ? Optional<AssignedLabel>.Failure("Assigned label not found")
                : Optional<AssignedLabel>.Success(assignedLabel);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting assigned label");
            return Optional<AssignedLabel>.Failure(e.Message);
        }
    }

    public async Task<Optional<AssignedLabel>> AddAssignedLabelAsync(AssignedLabel assignedLabel)
    {
        try
        {
            var a = await context.AssignedLabels.AddAsync(assignedLabel);
            await context.SaveChangesAsync();

            return Optional<AssignedLabel>.Success(a.Entity);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while adding assigned label");
            return Optional<AssignedLabel>.Failure(e.Message);
        }
    }

    public async Task<Optional<AssignedLabel>> UpdateAssignedLabelAsync(AssignedLabel assignedLabel)
    {
        try
        {
            context.AssignedLabels.Update(assignedLabel);
            await context.SaveChangesAsync();

            return Optional<AssignedLabel>.Success(assignedLabel);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while updating assigned label");
            return Optional<AssignedLabel>.Failure(e.Message);
        }
    }

    public async Task DeleteAssignedLabelAsync(AssignedLabel assignedLabel)
    {
        try
        {
            context.AssignedLabels.Remove(assignedLabel);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while deleting assigned label");
        }
    }
}