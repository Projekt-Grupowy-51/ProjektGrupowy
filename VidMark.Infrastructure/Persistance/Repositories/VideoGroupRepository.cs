using Microsoft.EntityFrameworkCore;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Domain.Models;

namespace VidMark.Infrastructure.Persistance.Repositories;

public class VideoGroupRepository(IReadWriteContext context) : IVideoGroupRepository
{
    public Task<List<VideoGroup>> GetVideoGroupsAsync(string userId, bool isAdmin)
    {
        return context.VideoGroups.FilteredVideoGroups(userId, isAdmin)
                .ToListAsync();
    }

    public Task<VideoGroup> GetVideoGroupAsync(int id, string userId, bool isAdmin)
    {
        return context.VideoGroups.FilteredVideoGroups(userId, isAdmin)
                .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task AddVideoGroupAsync(VideoGroup videoGroup)
    {
        _ = await context.VideoGroups.AddAsync(videoGroup);
    }

    public Task<List<VideoGroup>> GetVideoGroupsByProjectAsync(int projectId, string userId, bool isAdmin)
    {
        return context.VideoGroups.FilteredVideoGroups(userId, isAdmin)
                .Where(v => v.Project.Id == projectId)
                .ToListAsync();
    }

    public void DeleteVideoGroup(VideoGroup videoGroup)
    {
        _ = context.VideoGroups.Remove(videoGroup);
    }

    public Task<List<Video>> GetVideosByVideoGroupIdAsync(int id, string userId, bool isAdmin)
    {
        return context.VideoGroups.FilteredVideoGroups(userId, isAdmin)
            .Where(x => x.Id == id)
            .SelectMany(x => x.Videos)
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }
}