using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Interfaces.Repositories;

public interface IVideoGroupRepository
{
    Task<List<VideoGroup>> GetVideoGroupsAsync(string userId, bool isAdmin);
    Task<VideoGroup> GetVideoGroupAsync(int id, string userId, bool isAdmin);
    Task AddVideoGroupAsync(VideoGroup videoGroup);
    Task<List<VideoGroup>> GetVideoGroupsByProjectAsync(int projectId, string userId, bool isAdmin);
    void DeleteVideoGroup(VideoGroup videoGroup);
    Task<List<Video>> GetVideosByVideoGroupIdAsync(int id, string userId, bool isAdmin);
}