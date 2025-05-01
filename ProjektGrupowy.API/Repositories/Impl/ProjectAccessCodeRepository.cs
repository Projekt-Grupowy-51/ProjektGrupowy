using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils;
using System.Linq;

namespace ProjektGrupowy.API.Repositories.Impl;

public class ProjectAccessCodeRepository(
    AppDbContext dbContext,
    ILogger<ProjectAccessCodeRepository> logger,
    ICurrentUserService currentUserService) : IProjectAccessCodeRepository
{
    public async Task<Optional<ProjectAccessCode>> GetAccessCodeByCodeAsync(string code)
    {
        try
        {
            var accessCode = await dbContext
                .ProjectAccessCodes.FilteredProjectAccessCodes(currentUserService.UserId, currentUserService.IsAdmin)
                .FirstOrDefaultAsync(p => p.Code == code);

            return accessCode is null
                ? Optional<ProjectAccessCode>.Failure("The code is invalid.")
                : Optional<ProjectAccessCode>.Success(accessCode);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting the access code by code.");
            return Optional<ProjectAccessCode>.Failure("An error occurred while getting the access code by code.");
        }
    }

    public async Task<Optional<IEnumerable<ProjectAccessCode>>> GetAccessCodesByProjectAsync(int projectId)
    {
        try
        {
            var accessCodes = await dbContext
                .ProjectAccessCodes.FilteredProjectAccessCodes(currentUserService.UserId, currentUserService.IsAdmin)
                .Where(p => p.Project.Id == projectId)
                .ToArrayAsync();

            return Optional<IEnumerable<ProjectAccessCode>>.Success(accessCodes);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting the access codes by project.");
            return Optional<IEnumerable<ProjectAccessCode>>.Failure("An error occurred while getting the access codes by project.");
        }
    }

    public async Task<Optional<ProjectAccessCode>> GetValidAccessCodeByProjectAsync(int projectId)
    {
        try
        {
            var validAccessCode = await dbContext
                .ProjectAccessCodes.FilteredProjectAccessCodes(currentUserService.UserId, currentUserService.IsAdmin)
                .Where(p => p.Project.Id == projectId)
                .Where(p => (p.ExpiresAtUtc == null || p.ExpiresAtUtc > DateTime.UtcNow))
                .SingleOrDefaultAsync();

            return validAccessCode is null
                ? Optional<ProjectAccessCode>.Failure("There is no valid access code for this project.")
                : Optional<ProjectAccessCode>.Success(validAccessCode);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting the valid access code by project.");
            return Optional<ProjectAccessCode>.Failure("An error occurred while getting the valid access code by project.");
        }
    }

    public async Task<Optional<ProjectAccessCode>> AddAccessCodeAsync(ProjectAccessCode accessCode)
    {
        try
        {
            await dbContext.ProjectAccessCodes.AddAsync(accessCode);
            await dbContext.SaveChangesAsync();

            return Optional<ProjectAccessCode>.Success(accessCode);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while adding the access code.");
            return Optional<ProjectAccessCode>.Failure("An error occurred while adding the access code.");
        }
    }

    public async Task<Optional<ProjectAccessCode>> UpdateAccessCodeAsync(ProjectAccessCode accessCode)
    {
        try
        {
            dbContext.ProjectAccessCodes.Update(accessCode);
            await dbContext.SaveChangesAsync();

            return Optional<ProjectAccessCode>.Success(accessCode);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while updating the access code.");
            return Optional<ProjectAccessCode>.Failure("An error occurred while updating the access code.");
        }
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync() =>
        await dbContext.Database.BeginTransactionAsync();
}