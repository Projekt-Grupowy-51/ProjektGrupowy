using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories.Impl;

public class VideoRepository(AppDbContext dbContext, ILogger<VideoRepository> logger) : IVideoRepository
{
    public async Task<Optional<IEnumerable<Video>>> GetVideosAsync()
    {
        try
        {
            var videos = await dbContext.Videos.ToListAsync();
            return Optional<IEnumerable<Video>>.Success(videos);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting videos");
            return Optional<IEnumerable<Video>>.Failure(e.Message);
        }
    }

    public async Task<Optional<IEnumerable<Video>>> GetVideosAsync(int videoGroupId, int positionInQueue)
    {
        try
        {
            // Index lookup
            var videos = await dbContext.Videos
                .AsNoTracking()
                .Where(v => v.VideoGroupId == videoGroupId)
                .Where(v => v.PositionInQueue == positionInQueue)
                .ToListAsync();

            return Optional<IEnumerable<Video>>.Success(videos);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting videos");
            return Optional<IEnumerable<Video>>.Failure(e.Message);
        }
    }

    public async Task<Optional<Video>> GetVideoAsync(int id)
    {
        try
        {
            var video = await dbContext.Videos.FindAsync(id);
            return video is null
                ? Optional<Video>.Failure("Video not found")
                : Optional<Video>.Success(video);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting video");
            return Optional<Video>.Failure(e.Message);
        }
    }

    public async Task<Optional<Video>> AddVideoAsync(Video video)
    {
        try
        {
            var v = await dbContext.Videos.AddAsync(video);
            await dbContext.SaveChangesAsync();

            return Optional<Video>.Success(v.Entity);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while adding video");
            return Optional<Video>.Failure(e.Message);
        }
    }

    public async Task<Optional<Video>> UpdateVideoAsync(Video video)
    {
        try
        {
            dbContext.Videos.Update(video);
            await dbContext.SaveChangesAsync();

            return Optional<Video>.Success(video);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while updating video");
            return Optional<Video>.Failure(e.Message);
        }
    }

    public async Task DeleteVideoAsync(Video video)
    {
        try
        {
            dbContext.Videos.Remove(video);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while deleting video");
        }
    }
}