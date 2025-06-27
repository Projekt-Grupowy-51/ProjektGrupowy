using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

public interface IVideoRepository 
{
    Task<Optional<IEnumerable<Video>>> GetVideosAsync();
    Task<Optional<IEnumerable<Video>>> GetVideosAsync(int videoGroupId, int positionInQueue);
    Task<Optional<Video>> GetVideoAsync(int id);
    Task<Optional<Video>> AddVideoAsync(Video video);
    Task<Optional<Video>> UpdateVideoAsync(Video video);
    Task DeleteVideoAsync(Video video);
}