using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.API.DTOs.VideoGroup;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.SignalR;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class VideoGroupService(
    IVideoGroupRepository videoGroupRepository, 
    IProjectRepository projectRepository,
    IMessageService messageService,
    UserManager<User> userManager) : IVideoGroupService
{
    public async Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsAsync()
    {
        return await videoGroupRepository.GetVideoGroupsAsync();
    }

    public async Task<Optional<VideoGroup>> GetVideoGroupAsync(int id)
    {
        return await videoGroupRepository.GetVideoGroupAsync(id);
    }

    public async Task<Optional<VideoGroup>> AddVideoGroupAsync(VideoGroupRequest videoGroupRequest)
    {
        var owner = await userManager.FindByIdAsync(videoGroupRequest.OwnerId);
        if (owner == null)
        {
            return Optional<VideoGroup>.Failure("No labeler found");
        }

        var projectOptional = await projectRepository.GetProjectAsync(videoGroupRequest.ProjectId);

        if (projectOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                videoGroupRequest.OwnerId,
                "No project found");
            return Optional<VideoGroup>.Failure("No project found!");
        }

        var videoGroup = new VideoGroup
        {
            Name = videoGroupRequest.Name,
            Project = projectOptional.GetValueOrThrow(),
            Description = videoGroupRequest.Description,
            Owner = owner,
        };

        // return await videoGroupRepository.AddVideoGroupAsync(videoGroup);
        var result = await videoGroupRepository.AddVideoGroupAsync(videoGroup);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                videoGroupRequest.OwnerId,
                "Failed to add video group");
            return result;
        }
        await messageService.SendSuccessAsync(
            videoGroupRequest.OwnerId,
            "Video group added successfully");
        return result;
    }

    public async Task<Optional<VideoGroup>> UpdateVideoGroupAsync(int videoGroupId, VideoGroupRequest videoGroupRequest)
    {
        var owner = await userManager.FindByIdAsync(videoGroupRequest.OwnerId);
        if (owner == null)
        {
            return Optional<VideoGroup>.Failure("No labeler found");
        }

        var videoGroupOptional = await videoGroupRepository.GetVideoGroupAsync(videoGroupId);

        if (videoGroupOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                videoGroupRequest.OwnerId,
                "No video group found");
            return videoGroupOptional;
        }

        var videoGroup = videoGroupOptional.GetValueOrThrow();

        var projectOptional = await projectRepository.GetProjectAsync(videoGroupRequest.ProjectId);

        if (projectOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                videoGroupRequest.OwnerId,
                "No project found");
            return Optional<VideoGroup>.Failure("No project found!");
        }

        videoGroup.Name = videoGroupRequest.Name;
        videoGroup.Project = projectOptional.GetValueOrThrow();
        videoGroup.Description = videoGroupRequest.Description;
        videoGroup.Owner = owner;

        // return await videoGroupRepository.UpdateVideoGroupAsync(videoGroup);
        var result = await videoGroupRepository.UpdateVideoGroupAsync(videoGroup);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                videoGroupRequest.OwnerId,
                "Failed to update video group");
            return result;
        }
        await messageService.SendSuccessAsync(
            videoGroupRequest.OwnerId,
            "Video group updated successfully");
        return result;
    }

    public async Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsByProjectAsync(int projectId)
        => await videoGroupRepository.GetVideoGroupsByProjectAsync(projectId);

    public async Task DeleteVideoGroupAsync(int id)
    {
        var videoGroupOpt = await videoGroupRepository.GetVideoGroupAsync(id);
        if (videoGroupOpt.IsSuccess)
        {
            var videoGroup = videoGroupOpt.GetValueOrThrow();
            await messageService.SendInfoAsync(
                videoGroup.Owner.Id,
                "Video group deleted successfully");
            await videoGroupRepository.DeleteVideoGroupAsync(videoGroup);
        }
    }

    public async Task<Optional<IEnumerable<Video>>> GetVideosByVideoGroupIdAsync(int id)
    {
        return await videoGroupRepository.GetVideosByVideoGroupIdAsync(id);
    }
}