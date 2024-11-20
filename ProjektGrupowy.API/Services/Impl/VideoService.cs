using ProjektGrupowy.API.DTOs.Video;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class VideoService(
    IVideoRepository videoRepository,
    IProjectRepository projectRepository,
    IConfiguration configuration) : IVideoService
{
    public async Task<Optional<IEnumerable<Video>>> GetVideosAsync() => await videoRepository.GetVideosAsync();

    public async Task<Optional<IEnumerable<Video>>> GetVideosFromProjectAsync(int projectId)
    {
        var project = await projectRepository.GetProjectAsync(projectId);
        return project.IsFailure
            ? Optional<IEnumerable<Video>>.Failure(project.GetErrorOrThrow())
            : Optional<IEnumerable<Video>>.Success(project.GetValueOrThrow().Videos);
    }

    public async Task<Optional<Video>> GetVideoAsync(int id) => await videoRepository.GetVideoAsync(id);

    public async Task<Optional<Video>> AddVideoAsync(VideoRequest videoRequest)
    {
        var filename = $"{Guid.NewGuid()}{Path.GetExtension(videoRequest.File.FileName)}";
        var videoPath = Path.Combine(configuration["Videos:Path"]!, filename);

        await using (var fileStream = new FileStream(videoPath, FileMode.Create))
        {
            await videoRequest.File.CopyToAsync(fileStream);
        }

        var video = new Video
        {
            Title = videoRequest.Title,
            Description = videoRequest.Description,
            Path = videoPath
        };

        return await videoRepository.AddVideoAsync(video);
    }

    public async Task<Optional<Video>> AddVideoToProjectAsync(int projectId, VideoRequest videoRequest)
    {
        await using var transaction = await projectRepository.BeginTransactionAsync();

        try
        {
            var project = await projectRepository.GetProjectAsync(projectId);
            if (project.IsFailure)
                return Optional<Video>.Failure(project.GetErrorOrThrow());

            var videoOptional = await AddVideoAsync(videoRequest);
            if (videoOptional.IsFailure)
                return Optional<Video>.Failure(videoOptional.GetErrorOrThrow());

            var projectEntity = project.GetValueOrThrow();
            projectEntity.Videos.Add(videoOptional.GetValueOrThrow());

            await projectRepository.UpdateProjectAsync(projectEntity);

            await transaction.CommitAsync(); // Commit transaction if everything is successful

            return videoOptional;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();          // Rollback transaction in case of any error
            return Optional<Video>.Failure(ex.Message); // Return failure with exception details
        }
    }

    public async Task<Optional<Video>> UpdateVideoAsync(Video video) => await videoRepository.UpdateVideoAsync(video);

    public async Task DeleteVideoAsync(int id)
    {
        var video = await videoRepository.GetVideoAsync(id);
        if (video.IsSuccess)
            await videoRepository.DeleteVideoAsync(video.GetValueOrThrow());
    }
}