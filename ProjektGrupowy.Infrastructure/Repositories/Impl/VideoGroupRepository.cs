using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Services;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Infrastructure.Data;

namespace ProjektGrupowy.Infrastructure.Repositories.Impl;

public class VideoGroupRepository(AppDbContext context, ILogger<VideoGroupRepository> logger, ICurrentUserService currentUserService)
    : IVideoGroupRepository
{
    public async Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsAsync()
    {
        try
        {
            var videoGroups = await context.VideoGroups.FilteredVideoGroups(currentUserService.UserId, currentUserService.IsAdmin)
                .ToListAsync();
            return Optional<IEnumerable<VideoGroup>>.Success(videoGroups);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting video groups");
            return Optional<IEnumerable<VideoGroup>>.Failure(ex.Message);
        }
    }

    public async Task<Optional<VideoGroup>> GetVideoGroupAsync(int id)
    {
        try
        {
            var videoGroup = await context.VideoGroups.FilteredVideoGroups(currentUserService.UserId, currentUserService.IsAdmin)
                .FirstOrDefaultAsync(v => v.Id == id);
            return videoGroup is null
                ? Optional<VideoGroup>.Failure("Video group not found")
                : Optional<VideoGroup>.Success(videoGroup);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting video group");
            return Optional<VideoGroup>.Failure(e.Message);
        }
    }

    public async Task<Optional<VideoGroup>> AddVideoGroupAsync(VideoGroup videoGroup)
    {
        try
        {
            var v = await context.VideoGroups.AddAsync(videoGroup);
            await context.SaveChangesAsync();

            return Optional<VideoGroup>.Success(v.Entity);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while adding video group");
            return Optional<VideoGroup>.Failure(e.Message);
        }
    }

    public async Task<Optional<VideoGroup>> UpdateVideoGroupAsync(VideoGroup videoGroup)
    {
        try
        {
            context.VideoGroups.Update(videoGroup);
            await context.SaveChangesAsync();

            return Optional<VideoGroup>.Success(videoGroup);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while updating video group");
            return Optional<VideoGroup>.Failure(e.Message);
        }
    }

    public async Task<Optional<IEnumerable<VideoGroup>>> GetVideoGroupsByProjectAsync(int projectId)
    {
        try
        {
            // This is efficient because it uses an indexed column ("IX_VideoGroups_ProjectId" btree ("ProjectId"))
            // Check with psql: mydatabase=# EXPLAIN SELECT * FROM "VideoGroups" WHERE "ProjectId" = ...;
            var videoGroups = await context.VideoGroups.FilteredVideoGroups(currentUserService.UserId, currentUserService.IsAdmin)
                .Where(v => v.Project.Id == projectId)
                .ToArrayAsync();

            return Optional<IEnumerable<VideoGroup>>.Success(videoGroups);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting video groups by project");
            return Optional<IEnumerable<VideoGroup>>.Failure(e.Message);
        }
    }

    public async Task DeleteVideoGroupAsync(VideoGroup videoGroup)
    {
        try
        {
            context.VideoGroups.Remove(videoGroup);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while deleting video group");
        }
    }

    public async Task<Optional<IEnumerable<Video>>> GetVideosByVideoGroupIdAsync(int id)
    {
        try
        {
            var videoGroup = await context.VideoGroups.FilteredVideoGroups(currentUserService.UserId, currentUserService.IsAdmin)
                .FirstOrDefaultAsync(vg => vg.Id == id);

            if (videoGroup is null)
            {
                logger.LogWarning("Video group with ID {VideoGroupId} not found", id);
                return Optional<IEnumerable<Video>>.Failure("Video group not found");
            }

            return Optional<IEnumerable<Video>>.Success(videoGroup.Videos.OrderByDescending(v => v.Id));
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting videos for video group with ID {VideoGroupId}", id);
            return Optional<IEnumerable<Video>>.Failure("An error occurred while processing your request");
        }
    }
}