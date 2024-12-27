using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl
{
    public class VideoGroupService(IVideoGroupRepository videoGroupRepository) : IVideoGroupService
    {
        public async Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsAsync()
        {
            return await videoGroupRepository.GetVideoGroupsAsync();
        }

        public async Task<Optional<VideoGroup>> GetVideoGroupAsync(int id)
        {
            return await videoGroupRepository.GetVideoGroupAsync(id);
        }

        public async Task<Optional<VideoGroup>> AddVideoGroupAsync(VideoGroup videoGroup)
        {
            return await videoGroupRepository.AddVideoGroupAsync(videoGroup);
        }

        public async Task<Optional<VideoGroup>> UpdateVideoGroupAsync(VideoGroup videoGroup)
        {
            return await videoGroupRepository.UpdateVideoGroupAsync(videoGroup);
        }

        public async Task DeleteVideoGroupAsync(int id)
        {
            var videoGroup = await videoGroupRepository.GetVideoGroupAsync(id);
            if (videoGroup.IsSuccess)
            {
                await videoGroupRepository.DeleteVideoGroupAsync(videoGroup.GetValueOrThrow());
            }
        }
    }
}
