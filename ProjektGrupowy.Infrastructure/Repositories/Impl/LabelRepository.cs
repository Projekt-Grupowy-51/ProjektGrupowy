using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Infrastructure.Data;

namespace ProjektGrupowy.Infrastructure.Repositories.Impl;

public class LabelRepository(AppDbContext context, ILogger<LabelRepository> logger) : ILabelRepository
{
    public async Task<Optional<IEnumerable<Label>>> GetLabelsAsync(string userId, bool isAdmin)
    {
        try
        {
            var labels = await context.Labels.FilteredLabels(userId, isAdmin).ToListAsync();
            return Optional<IEnumerable<Label>>.Success(labels);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting labels");
            return Optional<IEnumerable<Label>>.Failure(e.Message);
        }
    }

    public async Task<Optional<Label>> GetLabelAsync(int id, string userId, bool isAdmin)
    {
        try
        {
            var label = await context.Labels.FilteredLabels(userId, isAdmin).FirstOrDefaultAsync(l => l.Id == id);
            return label is null
                ? Optional<Label>.Failure("Label not found")
                : Optional<Label>.Success(label);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting label");
            return Optional<Label>.Failure(e.Message);
        }
    }

    public async Task<Optional<Label>> AddLabelAsync(Label label)
    {
        try
        {
            var l = await context.Labels.AddAsync(label);
            await context.SaveChangesAsync();

            return Optional<Label>.Success(l.Entity);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while adding label");
            return Optional<Label>.Failure(e.Message);
        }
    }

    public async Task<Optional<Label>> UpdateLabelAsync(Label label)
    {
        try
        {
            context.Labels.Update(label);
            await context.SaveChangesAsync();

            return Optional<Label>.Success(label);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while updating label");
            return Optional<Label>.Failure(e.Message);
        }
    }

    public async Task DeleteLabelAsync(Label label)
    {
        try
        {
            context.Labels.Remove(label);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while deleting label");
        }
    }
}