using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.DTOs.AssignedLabel;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Infrastructure.Repositories;
using ProjektGrupowy.Application.SignalR;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Services.Impl;

public class AssignedLabelService(
    IAssignedLabelRepository assignedLabelRepository,
    ILabelRepository labelRepository,
    ISubjectVideoGroupAssignmentRepository subjectVideoGroupAssignmentRepository,
    UserManager<User> userManager,
    IMessageService messageService,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService,
    IVideoRepository videoRepository) : IAssignedLabelService

{
    public async Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsAsync()
    {
        var assignedLabelsOpt = await assignedLabelRepository.GetAssignedLabelsAsync(currentUserService.UserId, currentUserService.IsAdmin);
        if (assignedLabelsOpt.IsFailure)
        {
            return assignedLabelsOpt;
        }

        var assignedLabels = assignedLabelsOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, assignedLabels, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return assignedLabelsOpt;
    }

    public async Task<Optional<AssignedLabel>> GetAssignedLabelAsync(int id)
    {
        var assignedLabelOpt = await assignedLabelRepository.GetAssignedLabelAsync(id, currentUserService.UserId, currentUserService.IsAdmin);
        if (assignedLabelOpt.IsFailure)
        {
            return assignedLabelOpt;
        }

        var assignedLabel = assignedLabelOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, assignedLabel, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return assignedLabelOpt;
    }

    public async Task<Optional<AssignedLabel>> AddAssignedLabelAsync(AssignedLabelRequest assignedLabelRequest)
    {
        var labelOptional = await labelRepository.GetLabelAsync(assignedLabelRequest.LabelId, currentUserService.UserId, currentUserService.IsAdmin);

        if (labelOptional.IsFailure)
        {
            return Optional<AssignedLabel>.Failure("No label found");
        }

        var label = labelOptional.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, label, new ResourceOperationRequirement(ResourceOperation.Create));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var videoOptional = await videoRepository.GetVideoAsync(assignedLabelRequest.VideoId, currentUserService.UserId, currentUserService.IsAdmin);
        if (videoOptional.IsFailure)
        {
            return Optional<AssignedLabel>.Failure("No subject video group assignment found");
        }

        var video = videoOptional.GetValueOrThrow();
        var authResultVideo = await authorizationService.AuthorizeAsync(currentUserService.User, video, new ResourceOperationRequirement(ResourceOperation.Create));
        if (!authResultVideo.Succeeded)
        {
            throw new ForbiddenException();
        }

        var assignedLabel = new AssignedLabel
        {
            Label = labelOptional.GetValueOrThrow(),
            CreatedById = currentUserService.UserId,
            Video = video,
            Start = assignedLabelRequest.Start,
            End = assignedLabelRequest.End
        };

        var result = await assignedLabelRepository.AddAssignedLabelAsync(assignedLabel);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "Failed to add assigned label");
            return result;
        }

        await messageService.SendSuccessAsync(
            currentUserService.UserId,
            "Assigned label added successfully");
        return result;
    }

    public async Task DeleteAssignedLabelAsync(int id)
    {
        var assignedLabelOpt = await assignedLabelRepository.GetAssignedLabelAsync(id, currentUserService.UserId, currentUserService.IsAdmin);
        if (assignedLabelOpt.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "No assigned label found");
            return;
        }

        var assignedLabel = assignedLabelOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, assignedLabel, new ResourceOperationRequirement(ResourceOperation.Delete));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        await messageService.SendInfoAsync(
            currentUserService.UserId,
            "Assigned label deleted successfully");

        await assignedLabelRepository.DeleteAssignedLabelAsync(assignedLabel);
    }

    public async Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByVideoIdAsync(int videoId)
    {
        var assignedLabelsOpt = await assignedLabelRepository.GetAssignedLabelsByVideoIdAsync(videoId, currentUserService.UserId, currentUserService.IsAdmin);
        if (assignedLabelsOpt.IsFailure)
        {
            return assignedLabelsOpt;
        }

        var assignedLabels = assignedLabelsOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, assignedLabels, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return assignedLabelsOpt;
    }

     public async Task<Optional<IEnumerable<AssignedLabel>>> GetAssignedLabelsByVideoIdAndSubjectIdAsync(int videoId, int subjectId)
    {
        var assignedLabelsOpt = await assignedLabelRepository.GetAssignedLabelsByVideoIdAndSubjectIdAsync(videoId, subjectId, currentUserService.UserId, currentUserService.IsAdmin);
        if (assignedLabelsOpt.IsFailure)
        {
            return assignedLabelsOpt;
        }

        var assignedLabels = assignedLabelsOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, assignedLabels, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return assignedLabelsOpt;
    }
}