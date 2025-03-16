using ProjektGrupowy.API.DTOs.Video;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface IVideoService
{
    Task<Optional<IEnumerable<Video>>> GetVideosAsync();
    Task<Optional<IEnumerable<Video>>> GetVideosAsync(int videoGroupId, int pageSize, int pageNumber);
    Task<Optional<Video>> GetVideoAsync(int id);
    Task<Optional<Video>> AddVideoAsync(VideoRequest videoRequest);
    Task<Optional<Video>> UpdateVideoAsync(int videoId, VideoRequest videoRequest);
    Task DeleteVideoAsync(int id);
}