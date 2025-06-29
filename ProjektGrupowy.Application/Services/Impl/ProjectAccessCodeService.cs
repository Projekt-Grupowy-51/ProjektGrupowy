using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.DTOs.AccessCode;
using ProjektGrupowy.Application.Enums;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.SignalR;
using ProjektGrupowy.Application.Utils;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Infrastructure.Repositories;

namespace ProjektGrupowy.Application.Services.Impl;

public class ProjectAccessCodeService(
    IProjectAccessCodeRepository repository,
    IProjectRepository projectRepository,
    IMessageService messageService,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService,
    ILogger<ProjectAccessCodeService> logger) : IProjectAccessCodeService
{
    public async Task<bool> ValidateAccessCode(AccessCodeRequest accessCodeRequest)
    {
        try
        {
            var accessCodeOpt = await repository.GetAccessCodeByCodeAsync(accessCodeRequest.Code, currentUserService.UserId, currentUserService.IsAdmin);
            if (accessCodeOpt.IsFailure)
                return false;

            var accessCode = accessCodeOpt.GetValueOrThrow();
            var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, accessCode, new ResourceOperationRequirement(ResourceOperation.Read));

            return accessCode.IsValid;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while validating access code");
            return false;
        }
    }

    public async Task<Optional<IEnumerable<ProjectAccessCode>>> GetAccessCodesByProjectAsync(int projectId)
    {
        var accessCodeOpt = await repository.GetAccessCodesByProjectAsync(projectId, currentUserService.UserId, currentUserService.IsAdmin);
        if (accessCodeOpt.IsFailure)
        {
            return accessCodeOpt;
        }

        var accessCodes = accessCodeOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, accessCodes, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException("You do not have permission to access this projects access codes.");
        }

        return accessCodeOpt;
    }

    public async Task<Optional<ProjectAccessCode>> AddValidCodeToProjectAsync(CreateAccessCodeRequest createCodeRequest)
    {
        await using var transaction = await repository.BeginTransactionAsync();

        try
        {
            var projectOpt = await projectRepository.GetProjectAsync(createCodeRequest.ProjectId, currentUserService.UserId, currentUserService.IsAdmin);
            if (projectOpt.IsFailure)
            {
                await transaction.RollbackAsync();
                return Optional<ProjectAccessCode>.Failure("Project does not exist");
            }

            var project = projectOpt.GetValueOrThrow();
            var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, project, new ResourceOperationRequirement(ResourceOperation.Update));
            if (!authResult.Succeeded)
            {
                await transaction.RollbackAsync();
                throw new ForbiddenException("You do not have permission to access this project.");
            }

            // 2. Check if there is a valid access code for this project
            var accessCodeOpt = await repository.GetValidAccessCodeByProjectAsync(project.Id, currentUserService.UserId, currentUserService.IsAdmin);

            if (accessCodeOpt.IsFailure)
            {
                // 3. If there isn't a valid access code, create a new one
                var expirationDate = GetExpirationDate(createCodeRequest.Expiration, createCodeRequest.CustomExpiration);

                var newAccessCode = AccessCodeGenerator.Create(project, expirationDate);
                newAccessCode.CreatedById = currentUserService.UserId;
                var newAddedAccessCodeOpt = await repository.AddAccessCodeAsync(newAccessCode);

                if (newAddedAccessCodeOpt.IsFailure)
                {
                    await transaction.RollbackAsync();
                    return Optional<ProjectAccessCode>.Failure(newAddedAccessCodeOpt.GetErrorOrThrow());
                }

                await transaction.CommitAsync();
                await messageService.SendSuccessAsync(
                    currentUserService.UserId,
                    "Access code added successfully");

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

                var expirationDate = GetExpirationDate(createCodeRequest.Expiration, createCodeRequest.CustomExpiration);
                var newAccessCode = AccessCodeGenerator.Create(project, expirationDate);

                newAccessCode.CreatedById = currentUserService.UserId;
                var newAddedAccessCodeOpt = await repository.AddAccessCodeAsync(newAccessCode);
                if (newAddedAccessCodeOpt.IsFailure)
                {
                    await transaction.RollbackAsync();
                    return Optional<ProjectAccessCode>.Failure(newAddedAccessCodeOpt.GetErrorOrThrow());
                }

                await transaction.CommitAsync();
                await messageService.SendSuccessAsync(
                    currentUserService.UserId,
                    "Access code added successfully");
                return newAddedAccessCodeOpt;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while adding access code to project");
            await transaction.RollbackAsync();

            return Optional<ProjectAccessCode>.Failure("Error while adding access code to project");
        }
    }

    public async Task<Optional<ProjectAccessCode>> RetireAccessCodeAsync(string code)
    {
        var result = await repository.GetAccessCodeByCodeAsync(code, currentUserService.UserId, currentUserService.IsAdmin);
        if (result.IsFailure)
            return Optional<ProjectAccessCode>.Failure("Access code not found");

        var accessCode = result.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, accessCode, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException("You do not have permission to access this access code.");
        }

        if (!accessCode.IsValid)
        {
            return Optional<ProjectAccessCode>.Failure("Access code is already retired");
        }

        accessCode.Retire();
        var updatedAccessCodeOpt = await repository.UpdateAccessCodeAsync(accessCode);
        if (updatedAccessCodeOpt.IsFailure)
        {
            return Optional<ProjectAccessCode>.Failure(updatedAccessCodeOpt.GetErrorOrThrow());
        }
        else
        {
            await messageService.SendInfoAsync(
                currentUserService.UserId,
                "Access code retired successfully");
            return updatedAccessCodeOpt;
        }
    }

    private static DateTime? GetExpirationDate(AccessCodeExpiration expiration, int days = -1) =>
        expiration switch
        {
            AccessCodeExpiration.In14Days => DateTime.Today.AddDays(14).ToUniversalTime(),
            AccessCodeExpiration.In30Days => DateTime.Today.AddDays(30).ToUniversalTime(),
            AccessCodeExpiration.Never => null,
            AccessCodeExpiration.Custom when days > 0 => DateTime.Today.AddDays(days).ToUniversalTime(),
            _ => throw new ArgumentException("Invalid expiration or days value")
        };
}