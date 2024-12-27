using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories;

public interface IVideoGroupRepository
{
    Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsAsync();
    Task<Optional<VideoGroup>> GetVideoGroupAsync(int id);
    Task<Optional<VideoGroup>> AddVideoGroupAsync(VideoGroup videoGroup);
    Task<Optional<VideoGroup>> UpdateVideoGroupAsync(VideoGroup videoGroup);
    Task DeleteVideoGroupAsync(VideoGroup videoGroup);
}