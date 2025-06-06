﻿using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories.Impl;

public class AssignedLabelRepository(AppDbContext context, ILogger<AssignedLabelRepository> logger, ICurrentUserService currentUserService) : IAssignedLabelRepository
{
    public async Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsAsync()
    {
        try
        {
            var assignedLabels = await context.AssignedLabels.FilteredAssignedLabels(currentUserService.UserId, currentUserService.IsAdmin)
                .OrderByDescending(a => a.InsDate)
                .ToListAsync();
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
            var assignedLabel = await context.AssignedLabels.FilteredAssignedLabels(currentUserService.UserId, currentUserService.IsAdmin).FirstOrDefaultAsync(a => a.Id == id);
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

    public async Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByVideoIdAsync(int videoId)
    {
        try
        {
            var assignedLabels = await context.AssignedLabels.FilteredAssignedLabels(currentUserService.UserId, currentUserService.IsAdmin)
                .Where(a => a.Video.Id == videoId)
                .OrderByDescending(a => a.InsDate)
                .ToListAsync();
            return Optional<IEnumerable<AssignedLabel>>.Success(assignedLabels);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting assigned labels by video");
            return Optional<IEnumerable<AssignedLabel>>.Failure(e.Message);
        }
    }

    public async Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByVideoIdAndSubjectIdAsync(int videoId, int subjectId)
    {
        try
        {
            var assignedLabels = await context.AssignedLabels.FilteredAssignedLabels(currentUserService.UserId, currentUserService.IsAdmin)
                .Where(a => a.Video.Id == videoId && a.Label.Subject.Id == subjectId)
                .OrderByDescending(a => a.InsDate)
                .ToListAsync();
            return Optional<IEnumerable<AssignedLabel>>.Success(assignedLabels);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting assigned labels by video and subject");
            return Optional<IEnumerable<AssignedLabel>>.Failure(e.Message);
        }
    }
}