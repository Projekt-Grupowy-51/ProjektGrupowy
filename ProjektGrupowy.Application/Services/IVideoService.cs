using ProjektGrupowy.Application.DTOs.Video;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Application.Services;

public interface IVideoService
{
    Task<Optional<IEnumerable<Video>>> GetVideosAsync();
    Task<Optional<IEnumerable<Video>>> GetVideosAsync(int videoGroupId, int positionInQueue);
    Task<Optional<Video>> GetVideoAsync(int id);
    Task<Optional<Video>> AddVideoAsync(VideoRequest videoRequest);
    Task<Optional<Video>> UpdateVideoAsync(int videoId, VideoRequest videoRequest);
    Task DeleteVideoAsync(int id);
}