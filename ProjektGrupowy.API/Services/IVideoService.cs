using ProjektGrupowy.API.DTOs.Video;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface IVideoService {
    Task<Optional<IEnumerable<Video>>> GetVideosAsync();
    Task<Optional<IEnumerable<Video>>> GetVideosFromProjectAsync(int projectId);
    Task<Optional<Video>> GetVideoAsync(int id);
    Task<Optional<Video>> AddVideoAsync(VideoRequest video);
    Task<Optional<Video>> AddVideoToProjectAsync(int projectId, VideoRequest video);
    Task<Optional<Video>> UpdateVideoAsync(Video video);
    Task DeleteVideoAsync(int id);
}