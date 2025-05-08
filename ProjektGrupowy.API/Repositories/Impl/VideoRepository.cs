using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories.Impl;

public class VideoRepository : IVideoRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<VideoRepository> _logger;
    private readonly ICurrentUserService _currentUserService;

    public VideoRepository(AppDbContext dbContext, ILogger<VideoRepository> logger, ICurrentUserService currentUserService)
    {
        _context = dbContext;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<Optional<IEnumerable<Video>>> GetVideosAsync()
    {
        try
        {
            var videos = await _context.Videos.FilteredVideos(_currentUserService.UserId, _currentUserService.IsAdmin)
                .ToListAsync();
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
            var videos = await _context.Videos.FilteredVideos(_currentUserService.UserId, _currentUserService.IsAdmin)
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
            var video = await _context.Videos.FilteredVideos(_currentUserService.UserId, _currentUserService.IsAdmin).FirstOrDefaultAsync(x => x.Id == id);
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
}