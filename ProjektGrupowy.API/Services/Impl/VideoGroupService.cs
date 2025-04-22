using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.API.DTOs.VideoGroup;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class VideoGroupService(IVideoGroupRepository videoGroupRepository, IProjectRepository projectRepository, UserManager<User> userManager) : IVideoGroupService
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
        var projectOptional = await projectRepository.GetProjectAsync(videoGroupRequest.ProjectId);

        if (projectOptional.IsFailure)
        {
            return Optional<VideoGroup>.Failure("No project found!");
        }

        var owner = await userManager.FindByIdAsync(videoGroupRequest.OwnerId);
        if (owner == null)
        {
            return Optional<VideoGroup>.Failure("No labeler found");
        }

        var videoGroup = new VideoGroup
        {
            Name = videoGroupRequest.Name,
            Project = projectOptional.GetValueOrThrow(),
            Description = videoGroupRequest.Description,
            Owner = owner,
        };

        return await videoGroupRepository.AddVideoGroupAsync(videoGroup);
    }

    public async Task<Optional<VideoGroup>> UpdateVideoGroupAsync(int videoGroupId, VideoGroupRequest videoGroupRequest)
    {
        var videoGroupOptional = await videoGroupRepository.GetVideoGroupAsync(videoGroupId);

        if (videoGroupOptional.IsFailure)
        {
            return videoGroupOptional;
        }

        var videoGroup = videoGroupOptional.GetValueOrThrow();

        var projectOptional = await projectRepository.GetProjectAsync(videoGroupRequest.ProjectId);

        if (projectOptional.IsFailure)
        {
            return Optional<VideoGroup>.Failure("No project found!");
        }

        var owner = await userManager.FindByIdAsync(videoGroupRequest.OwnerId);
        if (owner == null)
        {
            return Optional<VideoGroup>.Failure("No labeler found");
        }

        videoGroup.Name = videoGroupRequest.Name;
        videoGroup.Project = projectOptional.GetValueOrThrow();
        videoGroup.Description = videoGroupRequest.Description;
        videoGroup.Owner = owner;

        return await videoGroupRepository.UpdateVideoGroupAsync(videoGroup);
    }

    public async Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsByProjectAsync(int projectId)
        => await videoGroupRepository.GetVideoGroupsByProjectAsync(projectId);

    public async Task DeleteVideoGroupAsync(int id)
    {
        var videoGroup = await videoGroupRepository.GetVideoGroupAsync(id);
        if (videoGroup.IsSuccess)
        {
            await videoGroupRepository.DeleteVideoGroupAsync(videoGroup.GetValueOrThrow());
        }
    }

    public async Task<Optional<IEnumerable<Video>>> GetVideosByVideoGroupIdAsync(int id)
    {
        return await videoGroupRepository.GetVideosByVideoGroupIdAsync(id);
    }
}