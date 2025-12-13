using Microsoft.EntityFrameworkCore;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Domain.Models;

namespace VidMark.Infrastructure.Persistance.Repositories;

public class VideoRepository(IReadWriteContext context) : IVideoRepository
{
    public Task<List<Video>> GetVideosAsync(string userId, bool isAdmin)
    {
        return context.Videos.FilteredVideos(userId, isAdmin)
                .ToListAsync();
    }

    public Task<List<Video>> GetVideosAsync(int videoGroupId, int positionInQueue, string userId, bool isAdmin)
    {
        return context.Videos.FilteredVideos(userId, isAdmin)
                .Where(v => v.VideoGroupId == videoGroupId)
                .Where(v => v.PositionInQueue == positionInQueue)
                .ToListAsync();
    }

    public Task<Video> GetVideoAsync(int id, string userId, bool isAdmin)
    {
        return context.Videos.FilteredVideos(userId, isAdmin).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddVideoAsync(Video video)
    {
        _ = await context.Videos.AddAsync(video);
    }

    public void UpdateVideo(Video video)
    {
        _ = context.Videos.Update(video);
    }

    public void DeleteVideo(Video video)
    {
        _ = context.Videos.Remove(video);
    }
}