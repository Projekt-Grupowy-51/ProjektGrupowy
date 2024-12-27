using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface IVideoService
{
    Task<Optional<IEnumerable<Video>>> GetVideosAsync();
    Task<Optional<Video>> GetVideoAsync(int id);
    Task<Optional<Video>> AddVideoAsync(Video video, IFormFile file);
    Task<Optional<Video>> UpdateVideoAsync(Video video);
    Task DeleteVideoAsync(int id);
}