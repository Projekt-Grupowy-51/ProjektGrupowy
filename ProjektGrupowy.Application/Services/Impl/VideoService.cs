using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.DTOs.Video;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Infrastructure.Repositories;
using ProjektGrupowy.Application.SignalR;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace ProjektGrupowy.Application.Services.Impl;

public class VideoService(
    IVideoRepository videoRepository,
    IVideoGroupRepository videoGroupRepository,
    IConfiguration configuration,
    IMessageService messageService,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService,
    UserManager<User> userManager) : IVideoService
{
    public async Task<Optional<IEnumerable<Video>>> GetVideosAsync()
    {
        var videosOpt = await videoRepository.GetVideosAsync();
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

    public async Task<Optional<IEnumerable<Video>>> GetVideosAsync(int videoGroupId, int positionInQueue)
    {
        var videosOpt = await videoRepository.GetVideosAsync(videoGroupId, positionInQueue);
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


    public async Task<Optional<Video>> GetVideoAsync(int id)
    {
        var videoOpt = await videoRepository.GetVideoAsync(id);
        if (videoOpt.IsFailure)
        {
            return videoOpt;
        }

        var video = videoOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, video, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        return videoOpt;
    }

    public async Task<Optional<Video>> AddVideoAsync(VideoRequest videoRequest)
    {
        var videoGroupOptional = await videoGroupRepository.GetVideoGroupAsync(videoRequest.VideoGroupId);

        if (videoGroupOptional.IsFailure)
        {
            return Optional<Video>.Failure("No video group found!");
        }

        var videoGroup = videoGroupOptional.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, videoGroup, new ResourceOperationRequirement(ResourceOperation.Create));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var videoProject = videoGroup.Project;

        var videoGroupId = videoGroup.Id.ToString();
        var videoProjectId = videoProject.Id.ToString();

        var videoRootDirectory = configuration["Videos:RootDirectory"] ?? "videos";

        var filename = $"{Guid.NewGuid():N}{Path.GetExtension(videoRequest.File.FileName)}";

        var directoryPath = Path.Combine(AppContext.BaseDirectory, videoRootDirectory, videoProjectId, videoGroupId);

        Directory.CreateDirectory(directoryPath);

        var videoPath = Path.Combine(directoryPath, filename);

        await using (var fileStream = new FileStream(videoPath, FileMode.Create))
        {
            await videoRequest.File.CopyToAsync(fileStream);
        }

        if (videoGroup.Videos is null)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "videoGroup.Videos was null");
            return Optional<Video>.Failure("Error. videoGroup.Videos was null.");
        }

        var video = new Video
        {
            Title = videoRequest.Title,
            Path = videoPath,
            VideoGroup = videoGroup,
            ContentType = videoRequest.File.ContentType,
            PositionInQueue = videoRequest.PositionInQueue,
            CreatedById = currentUserService.UserId,
        };

        var result = await videoRepository.AddVideoAsync(video);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "Failed to add video");
            return result;
        }
        await messageService.SendSuccessAsync(
            currentUserService.UserId,
            "Video added successfully");
        return result;
    }

    public async Task<Optional<Video>> UpdateVideoAsync(int videoId, VideoRequest videoRequest)
    {
        var videoOptional = await videoRepository.GetVideoAsync(videoId);
        if (videoOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "No video found");
            return videoOptional;
        }

        var video = videoOptional.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, video, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        var videoGroupOptional = await videoGroupRepository.GetVideoGroupAsync(videoRequest.VideoGroupId);
        if (videoGroupOptional.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "No video group found");
            return Optional<Video>.Failure("No video group found!");
        }

        var videoGroup = videoGroupOptional.GetValueOrThrow();
        var authResultVG = await authorizationService.AuthorizeAsync(currentUserService.User, videoGroup, new ResourceOperationRequirement(ResourceOperation.Update));
        if (!authResultVG.Succeeded)
        {
            throw new ForbiddenException();
        }

        video.Title = videoRequest.Title;
        video.VideoGroup = videoGroup;

        if (videoRequest.File is null)
            return await videoRepository.UpdateVideoAsync(video);

        var filename = $"{Guid.NewGuid()}{Path.GetExtension(videoRequest.File.FileName)}";
        var videoPath = Path.Combine(configuration["Videos:Path"]!, filename);

        await using (var fileStream = new FileStream(videoPath, FileMode.Create))
        {
            await videoRequest.File.CopyToAsync(fileStream);
        }

        video.Path = videoPath;
        video.ContentType = videoRequest.File.ContentType;
        video.PositionInQueue = videoRequest.PositionInQueue;
        video.CreatedById = currentUserService.UserId;

        var result = await videoRepository.UpdateVideoAsync(video);
        if (result.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "Failed to update video");
            return result;
        }
        await messageService.SendSuccessAsync(
            currentUserService.UserId,
            "Video updated successfully");
        return result;
    }

    public async Task DeleteVideoAsync(int id)
    {
        var videoOpt = await videoRepository.GetVideoAsync(id);
        if (videoOpt.IsFailure)
        {
            await messageService.SendErrorAsync(
                currentUserService.UserId,
                "No video found");
            return;
        }

        var video = videoOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, video, new ResourceOperationRequirement(ResourceOperation.Delete));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException();
        }

        await videoRepository.DeleteVideoAsync(video);

        await messageService.SendInfoAsync(
            currentUserService.UserId,
            "Video deleted successfully");
    }
}