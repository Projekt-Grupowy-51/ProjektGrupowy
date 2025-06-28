using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

public interface IVideoGroupRepository
{
    Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsAsync(string userId, bool isAdmin);
    Task<Optional<VideoGroup>> GetVideoGroupAsync(int id, string userId, bool isAdmin);
    Task<Optional<VideoGroup>> AddVideoGroupAsync(VideoGroup videoGroup);
    Task<Optional<VideoGroup>> UpdateVideoGroupAsync(VideoGroup videoGroup);

    Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsByProjectAsync(int projectId, string userId, bool isAdmin);

    Task DeleteVideoGroupAsync(VideoGroup videoGroup);
    Task<Optional<IEnumerable<Video>>> GetVideosByVideoGroupIdAsync(int id, string userId, bool isAdmin);
}