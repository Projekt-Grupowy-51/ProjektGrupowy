using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Interfaces.Repositories;

public interface IVideoRepository
{
    Task<List<Video>> GetVideosAsync(string userId, bool isAdmin);
    Task<List<Video>> GetVideosAsync(int videoGroupId, int positionInQueue, string userId, bool isAdmin);
    Task<Video> GetVideoAsync(int id, string userId, bool isAdmin);
    Task AddVideoAsync(Video video);
    void DeleteVideo(Video video);
}