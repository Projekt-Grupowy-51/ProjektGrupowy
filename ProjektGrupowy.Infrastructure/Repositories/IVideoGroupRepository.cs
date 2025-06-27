using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

public interface IVideoGroupRepository
{
    Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsAsync();
    Task<Optional<VideoGroup>> GetVideoGroupAsync(int id);
    Task<Optional<VideoGroup>> AddVideoGroupAsync(VideoGroup videoGroup);
    Task<Optional<VideoGroup>> UpdateVideoGroupAsync(VideoGroup videoGroup);

    Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsByProjectAsync(int projectId);

    Task DeleteVideoGroupAsync(VideoGroup videoGroup);
    Task<Optional<IEnumerable<Video>>> GetVideosByVideoGroupIdAsync(int id);
}