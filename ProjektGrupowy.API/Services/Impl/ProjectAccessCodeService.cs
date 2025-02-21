using ProjektGrupowy.API.DTOs.AccessCode;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class ProjectAccessCodeService(
    IProjectAccessCodeRepository repository,
    IProjectRepository projectRepository,
    ILogger<ProjectAccessCodeService> logger) : IProjectAccessCodeService
{
    public async Task<bool> ValidateAccessCode(AccessCodeRequest accessCodeRequest)
    {
        try
        {
            var accessCodeOpt = await repository.GetAccessCodeByCodeAsync(accessCodeRequest.Code);
            if (accessCodeOpt.IsFailure)
                return false;

            var accessCode = accessCodeOpt.GetValueOrThrow();

            return accessCode.IsValid;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while validating access code");
            return false;
        }
    }

    public async Task<Optional<IEnumerable<ProjectAccessCode>>> GetAccessCodesByProjectAsync(int projectId) =>
        await repository.GetAccessCodesByProjectAsync(projectId);

    public async Task<Optional<ProjectAccessCode>> AddValidCodeToProjectAsync(CreateAccessCodeRequest createCodeRequest)
    {
        await using var transaction = await repository.BeginTransactionAsync();

        try
        {
            // 1. Check if project exists
            var projectOpt = await projectRepository.GetProjectAsync(createCodeRequest.ProjectId);
            if (projectOpt.IsFailure)
            {
                await transaction.RollbackAsync();
                return Optional<ProjectAccessCode>.Failure("Project does not exist");
            }

            var project = projectOpt.GetValueOrThrow();

            // 2. Check if there is a valid access code for this project
            var accessCodeOpt = await repository.GetValidAccessCodeByProjectAsync(project.Id);

            if (accessCodeOpt.IsFailure)
            {
                // 3. If there isn't a valid access code, create a new one
                
                var newAccessCode = AccessCodeGenerator.Create(project, createCodeRequest.ExpiresAtUtc);
                var newAddedAccessCodeOpt = await repository.AddAccessCodeAsync(newAccessCode);

                if (newAddedAccessCodeOpt.IsFailure)
                {
                    await transaction.RollbackAsync();
                    return Optional<ProjectAccessCode>.Failure(newAddedAccessCodeOpt.GetErrorOrThrow());
                }

                await transaction.CommitAsync();
                return newAddedAccessCodeOpt;
            }
            else
            {
                // 4. If there is a valid access code, retire it and create a new one

                var accessCode = accessCodeOpt.GetValueOrThrow();

                accessCode.Retire();

                var updatedAccessCodeOpt = await repository.UpdateAccessCodeAsync(accessCode);
                if (updatedAccessCodeOpt.IsFailure)
                {
                    await transaction.RollbackAsync();
                    return Optional<ProjectAccessCode>.Failure(updatedAccessCodeOpt.GetErrorOrThrow());
                }

                var newAccessCode = AccessCodeGenerator.Create(project, createCodeRequest.ExpiresAtUtc);

                var newAddedAccessCodeOpt = await repository.AddAccessCodeAsync(newAccessCode);
                if (newAddedAccessCodeOpt.IsFailure)
                {
                    await transaction.RollbackAsync();
                    return Optional<ProjectAccessCode>.Failure(newAddedAccessCodeOpt.GetErrorOrThrow());
                }

                await transaction.CommitAsync();
                return newAddedAccessCodeOpt;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while adding access code to project");
            await transaction.RollbackAsync();

            return Optional<ProjectAccessCode>.Failure(e.Message);
        }
    }
}