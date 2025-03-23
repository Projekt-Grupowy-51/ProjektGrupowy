using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories.Impl;

public class VideoRepository : IVideoRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<VideoRepository> _logger;

    public VideoRepository(AppDbContext dbContext, ILogger<VideoRepository> logger)
    {
        _context = dbContext;
        _logger = logger;
    }

    public async Task<Optional<IEnumerable<Video>>> GetVideosAsync()
    {
        try
        {
            var videos = await _context.Videos.ToListAsync();
            return Optional<IEnumerable<Video>>.Success(videos);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while getting videos");
            return Optional<IEnumerable<Video>>.Failure(e.Message);
        }
    }

    public async Task<Optional<IEnumerable<Video>>> GetVideosAsync(int videoGroupId, int positionInQueue)
    {
        try
        {
            // Index lookup
            var videos = await _context.Videos
                .AsNoTracking()
                .Where(v => v.VideoGroupId == videoGroupId)
                .Where(v => v.PositionInQueue == positionInQueue)
                .ToListAsync();

            return Optional<IEnumerable<Video>>.Success(videos);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while getting videos");
            return Optional<IEnumerable<Video>>.Failure(e.Message);
        }
    }

    public async Task<Optional<Video>> GetVideoAsync(int id)
    {
        try
        {
            var video = await _context.Videos.FindAsync(id);
            return video is null
                ? Optional<Video>.Failure("Video not found")
                : Optional<Video>.Success(video);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while getting video");
            return Optional<Video>.Failure(e.Message);
        }
    }

    public async Task<Optional<Video>> AddVideoAsync(Video video)
    {
        try
        {
            var v = await _context.Videos.AddAsync(video);
            await _context.SaveChangesAsync();

            return Optional<Video>.Success(v.Entity);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while adding video");
            return Optional<Video>.Failure(e.Message);
        }
    }

    public async Task<Optional<Video>> UpdateVideoAsync(Video video)
    {
        try
        {
            _context.Videos.Update(video);
            await _context.SaveChangesAsync();

            return Optional<Video>.Success(video);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while updating video");
            return Optional<Video>.Failure(e.Message);
        }
    }

    public async Task DeleteVideoAsync(Video video)
    {
        try
        {
            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while deleting video");
        }
    }

    public async Task<Optional<IEnumerable<Video>>> GetVideosByScientistIdAsync(int scientistId)
    {
        try
        {
            var videos = await _context.Videos
                .Include(v => v.VideoGroup)
                .ThenInclude(vg => vg.Project)
                .Where(v => v.VideoGroup.Project.Scientist.Id == scientistId)
                .ToListAsync();

            return Optional<IEnumerable<Video>>.Success(videos);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while getting videos by scientist ID");
            return Optional<IEnumerable<Video>>.Failure(e.Message);
        }
    }
}