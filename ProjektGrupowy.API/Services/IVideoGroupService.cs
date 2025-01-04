using ProjektGrupowy.API.DTOs.VideoGroup;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface IVideoGroupService
{
    Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsAsync();
    Task<Optional<VideoGroup>> GetVideoGroupAsync(int id);
    Task<Optional<VideoGroup>> AddVideoGroupAsync(VideoGroupRequest videoGroupRequest);
    Task<Optional<VideoGroup>> UpdateVideoGroupAsync(int videoGroupId, VideoGroupRequest videoGroupRequest);

    Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsByProjectAsync(int projectId);
    Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsByProjectAsync(Project project);

    Task DeleteVideoGroupAsync(int id);
}