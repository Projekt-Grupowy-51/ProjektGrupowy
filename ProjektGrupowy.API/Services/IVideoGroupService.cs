using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services
{
    public interface IVideoGroupService
    {
        Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsAsync();
        Task<Optional<VideoGroup>> GetVideoGroupAsync(int id);
        Task<Optional<VideoGroup>> AddVideoGroupAsync(VideoGroup videoGroup);
        Task<Optional<VideoGroup>> UpdateVideoGroupAsync(VideoGroup videoGroup);
        Task DeleteVideoGroupAsync(int id);
    }
}
