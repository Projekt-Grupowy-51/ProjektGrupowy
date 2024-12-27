using ProjektGrupowy.API.DTOs.Video;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class VideoService(
    IVideoRepository videoRepository,
    IConfiguration configuration) : IVideoService
{
    public async Task<Optional<IEnumerable<Video>>> GetVideosAsync() => await videoRepository.GetVideosAsync();

    public async Task<Optional<Video>> GetVideoAsync(int id) => await videoRepository.GetVideoAsync(id);

    public async Task<Optional<Video>> AddVideoAsync(Video videoRequest, IFormFile file)
    {
        var filename = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var videoPath = Path.Combine(configuration["Videos:Path"]!, filename);

        await using (var fileStream = new FileStream(videoPath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        var video = new Video
        {
            Title = videoRequest.Title,
            Description = videoRequest.Description,
            Path = videoPath
        };

        return await videoRepository.AddVideoAsync(video);
    }

    public async Task<Optional<Video>> UpdateVideoAsync(Video video) => await videoRepository.UpdateVideoAsync(video);

    public async Task DeleteVideoAsync(int id)
    {
        var video = await videoRepository.GetVideoAsync(id);
        if (video.IsSuccess)
            await videoRepository.DeleteVideoAsync(video.GetValueOrThrow());
    }
}