using Microsoft.AspNetCore.Identity;
using ProjektGrupowy.API.DTOs.Video;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class VideoService(
    IVideoRepository videoRepository,
    IVideoGroupRepository videoGroupRepository,
    IConfiguration configuration,
    UserManager<User> userManager) : IVideoService
{
    public async Task<Optional<IEnumerable<Video>>> GetVideosAsync() => await videoRepository.GetVideosAsync();

    public async Task<Optional<IEnumerable<Video>>> GetVideosAsync(int videoGroupId, int positionInQueue) => await videoRepository.GetVideosAsync(videoGroupId, positionInQueue);

    public async Task<Optional<Video>> GetVideoAsync(int id) => await videoRepository.GetVideoAsync(id);

    public async Task<Optional<Video>> AddVideoAsync(VideoRequest videoRequest)
    {
        var videoGroupOptional = await videoGroupRepository.GetVideoGroupAsync(videoRequest.VideoGroupId);

        if (videoGroupOptional.IsFailure)
        {
            return Optional<Video>.Failure("No video group found!");
        }

        var owner = await userManager.FindByIdAsync(videoRequest.OwnerId);
        if (owner == null)
        {
            return Optional<Video>.Failure("No labeler found");
        }

        var videoGroup = videoGroupOptional.GetValueOrThrow();
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
            return Optional<Video>.Failure("Error. videoGroup.Videos was null.");
        }

        var video = new Video
        {
            Title = videoRequest.Title,
            Path = videoPath,
            VideoGroup = videoGroup,
            ContentType = videoRequest.File.ContentType,
            PositionInQueue = videoRequest.PositionInQueue,
            Owner = owner,
        };

        return await videoRepository.AddVideoAsync(video);
    }

    public async Task<Optional<Video>> UpdateVideoAsync(int videoId, VideoRequest videoRequest)
    {
        var videoOptional = await videoRepository.GetVideoAsync(videoId);

        if (videoOptional.IsFailure)
        {
            return videoOptional;
        }

        var owner = await userManager.FindByIdAsync(videoRequest.OwnerId);
        if (owner == null)
        {
            return Optional<Video>.Failure("No labeler found");
        }

        var video = videoOptional.GetValueOrThrow();

        var videoGroupOptional = await videoGroupRepository.GetVideoGroupAsync(videoRequest.VideoGroupId);
        if (videoGroupOptional.IsFailure)
        {
            return Optional<Video>.Failure("No video group found!");
        }

        video.Title = videoRequest.Title;
        video.VideoGroup = videoGroupOptional.GetValueOrThrow();

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
        video.Owner = owner;

        return await videoRepository.UpdateVideoAsync(video);
    }


    public async Task DeleteVideoAsync(int id)
    {
        var video = await videoRepository.GetVideoAsync(id);
        if (video.IsSuccess)
            await videoRepository.DeleteVideoAsync(video.GetValueOrThrow());
    }
}