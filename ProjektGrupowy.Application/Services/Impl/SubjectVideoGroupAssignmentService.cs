using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.DTOs.SubjectVideoGroupAssignment;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.SignalR;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Infrastructure.Repositories;

namespace ProjektGrupowy.Application.Services.Impl;

public class SubjectVideoGroupAssignmentService(
    ISubjectVideoGroupAssignmentRepository subjectVideoGroupAssignmentRepository,
    ISubjectRepository subjectRepository,
    IVideoGroupRepository videoGroupRepository,
    IMessageService messageService,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService,
    UserManager<User> userManager) : ISubjectVideoGroupAssignmentService
{
    public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsAsync()
    {
        var svgaOpt = await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentsAsync(currentUserService.UserId, currentUserService.IsAdmin);
        if (svgaOpt.IsFailure)
        {
            return svgaOpt;
        }

        var svga = svgaOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, svga, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return svgaOpt;
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> GetSubjectVideoGroupAssignmentAsync(int id)
    {
        var subjectVideoGroupAssignmentOpt = await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(id, currentUserService.UserId, currentUserService.IsAdmin);
        if (subjectVideoGroupAssignmentOpt.IsFailure)
        {
            return subjectVideoGroupAssignmentOpt;
        }

        var subjectVideoGroupAssignment = subjectVideoGroupAssignmentOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, subjectVideoGroupAssignment, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return subjectVideoGroupAssignmentOpt;
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> AddSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignmentRequest subjectVideoGroupAssignmentRequest)
    {
        var subjectOptional = await subjectRepository.GetSubjectAsync(subjectVideoGroupAssignmentRequest.SubjectId, currentUserService.UserId, currentUserService.IsAdmin);

        if (subjectOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "No subject found");
            return Optional<SubjectVideoGroupAssignment>.Failure("No subject found!");
        }

        var subject = subjectOptional.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, subject, new ResourceOperationRequirement(ResourceOperation.Create));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var videoGroupOptional = await videoGroupRepository.GetVideoGroupAsync(subjectVideoGroupAssignmentRequest.VideoGroupId, currentUserService.UserId, currentUserService.IsAdmin);
        if (videoGroupOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "No video group found");
            return Optional<SubjectVideoGroupAssignment>.Failure("No video group found!");
        }

        var videoGroup = videoGroupOptional.GetValueOrThrow();
        var authResultVG = await authorizationService.AuthorizeAsync(currentUserService.User, videoGroup, new ResourceOperationRequirement(ResourceOperation.Create));
        if (!authResultVG.Succeeded)
        {
            throw new ForbiddenException();
        }

        var subjectVideoGroupAssignment = new SubjectVideoGroupAssignment
        {
            Subject = subjectOptional.GetValueOrThrow(),
            VideoGroup = videoGroupOptional.GetValueOrThrow(),
            CreationDate = DateOnly.FromDateTime(DateTime.Today),
            CreatedById = currentUserService.UserId,
        };

        var result = await subjectVideoGroupAssignmentRepository.AddSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignment);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "Failed to add subject video group assignment");
        }
        else
        {
            await messageService.SendSuccessAsync(
                currentUserService.UserId,
                "Subject video group assignment added successfully");
        }

        return result;
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> UpdateSubjectVideoGroupAssignmentAsync(int subjectVideoGroupAssignmentId, SubjectVideoGroupAssignmentRequest subjectVideoGroupAssignmentRequest)
    {
        var subjectVideoGroupAssignmentOptional = await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignmentId, currentUserService.UserId, currentUserService.IsAdmin);

        if (subjectVideoGroupAssignmentOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "No subject video group assignment found");
            return subjectVideoGroupAssignmentOptional;
        }

        var subjectVideoGroupAssignment = subjectVideoGroupAssignmentOptional.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, subjectVideoGroupAssignment, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var subjectOptional = await subjectRepository.GetSubjectAsync(subjectVideoGroupAssignmentRequest.SubjectId, currentUserService.UserId, currentUserService.IsAdmin);
        if (subjectOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "No subject found");
            return Optional<SubjectVideoGroupAssignment>.Failure("No subject found!");
        }

        var subject = subjectOptional.GetValueOrThrow();
        var authResultSubject = await authorizationService.AuthorizeAsync(currentUserService.User, subject, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResultSubject.Succeeded)
        {
            throw new ForbiddenException();
        }

        var videoGroupOptional = await videoGroupRepository.GetVideoGroupAsync(subjectVideoGroupAssignmentRequest.VideoGroupId, currentUserService.UserId, currentUserService.IsAdmin);
        if (videoGroupOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "No video group found");
            return Optional<SubjectVideoGroupAssignment>.Failure("No video group found!");
        }

        var videoGroup = videoGroupOptional.GetValueOrThrow();
        var authResultVideoGroup = await authorizationService.AuthorizeAsync(currentUserService.User, videoGroup, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResultVideoGroup.Succeeded)
        {
            throw new ForbiddenException();
        }

        subjectVideoGroupAssignment.Subject = subjectOptional.GetValueOrThrow();
        subjectVideoGroupAssignment.VideoGroup = videoGroupOptional.GetValueOrThrow();
        subjectVideoGroupAssignment.ModificationDate = DateOnly.FromDateTime(DateTime.Today);
        subjectVideoGroupAssignment.CreatedById = currentUserService.UserId;

        var result = await subjectVideoGroupAssignmentRepository.UpdateSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignment);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "Failed to update subject video group assignment");
        }
        else
        {
            await messageService.SendSuccessAsync(
                currentUserService.UserId,
                "Subject video group assignment updated successfully");
        }

        return result;
    }

    public async Task DeleteSubjectVideoGroupAssignmentAsync(int subjectVideoGroupAssignmentId)
    {
        var subjectVideoGroupAssignmentOpt = await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignmentId, currentUserService.UserId, currentUserService.IsAdmin);

        if (subjectVideoGroupAssignmentOpt.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "No subject video group assignment found");
            return;
        }

        var subjectVideoGroupAssignment = subjectVideoGroupAssignmentOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, subjectVideoGroupAssignment, new ResourceOperationRequirement(ResourceOperation.Delete));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        await messageService.SendInfoAsync(
            currentUserService.UserId,
            "Subject video group assignment deleted successfully");

        await subjectVideoGroupAssignmentRepository.DeleteSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignment);
    }

    public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsByProjectAsync(int projectId)
    {
        var subjectVideoGroupAssignmentsOpt = await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentsByProjectAsync(projectId, currentUserService.UserId, currentUserService.IsAdmin);
        if (subjectVideoGroupAssignmentsOpt.IsFailure)
        {
            return subjectVideoGroupAssignmentsOpt;
        }

        var subjectVideoGroupAssignments = subjectVideoGroupAssignmentsOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, subjectVideoGroupAssignments, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return subjectVideoGroupAssignmentsOpt;
    }

    public async Task<Optional<IEnumerable<User>>> GetSubjectVideoGroupAssignmentsLabelersAsync(int id)
    {
        var subjectVideoGroupAssignmentOpt = await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(id, currentUserService.UserId, currentUserService.IsAdmin);
        if (subjectVideoGroupAssignmentOpt.IsFailure)
        {
            return Optional<IEnumerable<User>>.Failure("No subject video group assignment found!");
        }

        var subjectVideoGroupAssignment = subjectVideoGroupAssignmentOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, subjectVideoGroupAssignment, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentsLabelersAsync(id, currentUserService.UserId, currentUserService.IsAdmin);
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> AssignLabelerToAssignmentAsync(int assignmentId, string labelerId)
    {
        var assignmentOpt = await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(assignmentId, currentUserService.UserId, currentUserService.IsAdmin);
        if (assignmentOpt.IsFailure)
        {
            return assignmentOpt;
        }

        var assignment = assignmentOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, assignment, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var labelerOpt = await userManager.FindByIdAsync(labelerId);
        if (labelerOpt == null)
        {
            await messageService.SendWarningAsync(
                currentUserService.UserId,
                "No labeler found");
            return Optional<SubjectVideoGroupAssignment>.Failure("No labeler found!");
        }

        var result = await subjectVideoGroupAssignmentRepository.AssignLabelerToAssignmentAsync(assignment.Id, labelerOpt.Id, currentUserService.UserId, currentUserService.IsAdmin);
        if (result.IsFailure)
        {
            await messageService.SendWarningAsync(
                currentUserService.UserId,
                $"Failed to assign labeler to assignment: {result.GetErrorOrThrow()}");
        }
        else
        {
            await messageService.SendSuccessAsync(
                currentUserService.UserId,
                "Labeler assigned to assignment successfully");
        }

        return result;
    }

    public async Task<Optional<SubjectVideoGroupAssignment>> UnassignLabelerFromAssignmentAsync(int assignmentId, string labelerId)
    {
        var assignmentOpt = await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(assignmentId, currentUserService.UserId, currentUserService.IsAdmin);
        if (assignmentOpt.IsFailure)
        {
            return assignmentOpt;
        }

        var assignment = assignmentOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, assignment, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var labelerOpt = await userManager.FindByIdAsync(labelerId);
        if (labelerOpt == null)
        {
            await messageService.SendWarningAsync(
                currentUserService.UserId,
                "No labeler found");
            return Optional<SubjectVideoGroupAssignment>.Failure("No labeler found!");
        }

        return await subjectVideoGroupAssignmentRepository.UnassignLabelerFromAssignmentAsync(assignment.Id, labelerOpt.Id, currentUserService.UserId, currentUserService.IsAdmin);
    }
}