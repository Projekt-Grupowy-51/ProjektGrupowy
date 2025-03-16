using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories;

public interface IVideoRepository 
{
    Task<Optional<IEnumerable<Video>>> GetVideosAsync();
    Task<Optional<IEnumerable<Video>>> GetVideosAsync(int videoGroupId, int positionInQueue);
    Task<Optional<Video>> GetVideoAsync(int id);
    Task<Optional<Video>> AddVideoAsync(Video video);
    Task<Optional<Video>> UpdateVideoAsync(Video video);
    Task DeleteVideoAsync(Video video);
}