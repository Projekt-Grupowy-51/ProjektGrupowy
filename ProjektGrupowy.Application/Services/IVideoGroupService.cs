using ProjektGrupowy.Application.DTOs.VideoGroup;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Application.Services;

public interface IVideoGroupService
{
    Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsAsync();
    Task<Optional<VideoGroup>> GetVideoGroupAsync(int id);
    Task<Optional<VideoGroup>> AddVideoGroupAsync(VideoGroupRequest videoGroupRequest);
    Task<Optional<VideoGroup>> UpdateVideoGroupAsync(int videoGroupId, VideoGroupRequest videoGroupRequest);

    Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsByProjectAsync(int projectId);

    Task DeleteVideoGroupAsync(int id);

    Task<Optional<IEnumerable<Video>>> GetVideosByVideoGroupIdAsync(int id);
}