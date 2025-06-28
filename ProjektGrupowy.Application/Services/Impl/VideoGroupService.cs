using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.DTOs.VideoGroup;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Infrastructure.Repositories;
using ProjektGrupowy.Application.SignalR;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Application.Services;

namespace ProjektGrupowy.Application.Services.Impl;

public class VideoGroupService(
    IVideoGroupRepository videoGroupRepository,
    IProjectRepository projectRepository,
    IMessageService messageService,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService,
    UserManager<User> userManager) : IVideoGroupService
{
    public async Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsAsync()
    {
        var videoGroupsOpt = await videoGroupRepository.GetVideoGroupsAsync(currentUserService.UserId, currentUserService.IsAdmin);
        if (videoGroupsOpt.IsFailure)
        {
            return videoGroupsOpt;
        }

        var videoGroups = videoGroupsOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, videoGroups, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return videoGroupsOpt;
    }

    public async Task<Optional<VideoGroup>> GetVideoGroupAsync(int id)
    {
        var videoGroupOpt = await videoGroupRepository.GetVideoGroupAsync(id, currentUserService.UserId, currentUserService.IsAdmin);
        if (videoGroupOpt.IsFailure)
        {
            return videoGroupOpt;
        }
        var videoGroup = videoGroupOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, videoGroup, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }
        return videoGroupOpt;
    }

    public async Task<Optional<VideoGroup>> AddVideoGroupAsync(VideoGroupRequest videoGroupRequest)
    {
        var projectOptional = await projectRepository.GetProjectAsync(videoGroupRequest.ProjectId, currentUserService.UserId, currentUserService.IsAdmin);

        if (projectOptional.IsFailure)
        {
            return Optional<VideoGroup>.Failure("No project found!");
        }

        var project = projectOptional.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, project, new ResourceOperationRequirement(ResourceOperation.Create));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var videoGroup = new VideoGroup
        {
            Name = videoGroupRequest.Name,
            Project = projectOptional.GetValueOrThrow(),
            Description = videoGroupRequest.Description,
            CreatedById = currentUserService.UserId,
        };

        var result = await videoGroupRepository.AddVideoGroupAsync(videoGroup);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "Failed to add video group");
            return result;
        }
        await messageService.SendSuccessAsync(
            currentUserService.UserId,
            "Video group added successfully");
        return result;
    }

    public async Task<Optional<VideoGroup>> UpdateVideoGroupAsync(int videoGroupId, VideoGroupRequest videoGroupRequest)
    {
        var videoGroupOptional = await videoGroupRepository.GetVideoGroupAsync(videoGroupId, currentUserService.UserId, currentUserService.IsAdmin);

        if (videoGroupOptional.IsFailure)
        {
            return videoGroupOptional;
        }

        var videoGroup = videoGroupOptional.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, videoGroup, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var projectOptional = await projectRepository.GetProjectAsync(videoGroupRequest.ProjectId, currentUserService.UserId, currentUserService.IsAdmin);
        if (projectOptional.IsFailure)
        {
            return Optional<VideoGroup>.Failure("No project found!");
        }

        var project = projectOptional.GetValueOrThrow();
        var authResultProject = await authorizationService.AuthorizeAsync(currentUserService.User, project, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResultProject.Succeeded)
        {
            throw new ForbiddenException();
        }

        videoGroup.Name = videoGroupRequest.Name;
        videoGroup.Project = projectOptional.GetValueOrThrow();
        videoGroup.Description = videoGroupRequest.Description;
        videoGroup.CreatedById = currentUserService.UserId;

        var result = await videoGroupRepository.UpdateVideoGroupAsync(videoGroup);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "Failed to update video group");
            return result;
        }
        await messageService.SendSuccessAsync(
            currentUserService.UserId,
            "Video group updated successfully");
        return result;
    }

    public async Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsByProjectAsync(int projectId)
    {
        var videoGroupsOpt = await videoGroupRepository.GetVideoGroupsByProjectAsync(projectId, currentUserService.UserId, currentUserService.IsAdmin);
        if (videoGroupsOpt.IsFailure)
        {
            return videoGroupsOpt;
        }

        var videoGroups = videoGroupsOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, videoGroups, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return videoGroupsOpt;
    }

    public async Task DeleteVideoGroupAsync(int id)
    {
        var videoGroupOpt = await videoGroupRepository.GetVideoGroupAsync(id, currentUserService.UserId, currentUserService.IsAdmin);

        if (videoGroupOpt.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "No video group found");
            return;
        }

        var videoGroup = videoGroupOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, videoGroup, new ResourceOperationRequirement(ResourceOperation.Delete));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        await messageService.SendInfoAsync(
            currentUserService.UserId,
            "Video group deleted successfully");

        await videoGroupRepository.DeleteVideoGroupAsync(videoGroup);
    }

    public async Task<Optional<IEnumerable<Video>>> GetVideosByVideoGroupIdAsync(int id)
    {
        var videosOpt = await videoGroupRepository.GetVideosByVideoGroupIdAsync(id, currentUserService.UserId, currentUserService.IsAdmin);
        if (videosOpt.IsFailure)
        {
            return videosOpt;
        }

        var videos = videosOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, videos, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return videosOpt;
    }
}