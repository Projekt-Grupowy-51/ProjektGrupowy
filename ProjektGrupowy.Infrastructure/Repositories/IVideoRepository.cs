using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

public interface IVideoRepository 
{
    Task<Optional<IEnumerable<Video>>> GetVideosAsync(string userId, bool isAdmin);
    Task<Optional<IEnumerable<Video>>> GetVideosAsync(int videoGroupId, int positionInQueue, string userId, bool isAdmin);
    Task<Optional<Video>> GetVideoAsync(int id, string userId, bool isAdmin);
    Task<Optional<Video>> AddVideoAsync(Video video);
    Task<Optional<Video>> UpdateVideoAsync(Video video);
    Task DeleteVideoAsync(Video video);
}