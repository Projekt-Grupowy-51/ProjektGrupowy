using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.DTOs.AssignedLabel;
using ProjektGrupowy.Application.DTOs.Video;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Application.Utils;
using ProjektGrupowy.Domain.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

/// <summary>
/// Controller for managing videos. Handles operations such as retrieving, adding, updating, deleting, and streaming videos.
/// </summary>
/// <param name="videoService"></param>
/// <param name="assignedLabelService"></param>
/// <param name="configuration"></param>
/// <param name="mapper"></param>
[Route("api/videos")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class VideoController(
    IVideoService videoService,
    IAssignedLabelService assignedLabelService,
    IConfiguration configuration,
    IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Get all videos.
    /// </summary>
    /// <returns>A collection of videos.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VideoResponse>>> GetVideosAsync()
    {
        var videos = await videoService.GetVideosAsync();

        return videos.IsSuccess
            ? Ok(mapper.Map<IEnumerable<VideoResponse>>(videos.GetValueOrThrow()))
            : NotFound(videos.GetErrorOrThrow());
    }

    /// <summary>
    /// Get a batch of videos based on video group ID and position in queue.
    /// </summary>
    /// <param name="videoGroupId">The ID of the video group.</param>
    /// <param name="positionInQueue">The position in the queue.</param>
    /// <returns>A collection of videos matching the criteria.</returns>
    [HttpGet("batch/{videoGroupId:int}/{positionInQueue:int}")]
    public async Task<ActionResult<IEnumerable<VideoResponse>>> GetVideosAsync(int videoGroupId, int positionInQueue)
    {
        var videos = await videoService.GetVideosAsync(videoGroupId, positionInQueue);
        return videos.IsSuccess
            ? Ok(mapper.Map<IEnumerable<VideoResponse>>(videos.GetValueOrThrow()))
            : NotFound(videos.GetErrorOrThrow());
    }

    /// <summary>
    /// Get a specific video by its ID.
    /// </summary>
    /// <param name="id">The ID of the video.</param>
    /// <returns>The video with the specified ID.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VideoResponse>> GetVideoAsync(int id)
    {
        var video = await videoService.GetVideoAsync(id);
        return video.IsSuccess
            ? Ok(mapper.Map<VideoResponse>(video.GetValueOrThrow()))
            : NotFound(video.GetErrorOrThrow());
    }

    /// <summary>
    /// Add a new video.
    /// </summary>
    /// <param name="videoRequest">The request containing the details of the video to be added.</param>
    /// <returns>201 Created with the created video details or 400 Bad Request if the operation fails.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<VideoResponse>> AddVideoAsync(VideoRequest videoRequest)
    {
        var result = await videoService.AddVideoAsync(videoRequest);

        if (result.IsFailure)
            return BadRequest(result.GetErrorOrThrow());

        var createdVideo = result.GetValueOrThrow();

        var videoResponse = mapper.Map<VideoResponse>(createdVideo);

        return CreatedAtAction("GetVideo", new { id = createdVideo.Id }, videoResponse);
    }

    /// <summary>
    /// Update an existing video.
    /// </summary>
    /// <param name="id">The ID of the video to be updated.</param>
    /// <param name="videoRequest">The request containing the updated details of the video.</param>
    /// <returns>204 No Content if the update is successful or 400 Bad Request if the operation fails.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutVideoAsync(int id, VideoRequest videoRequest)
    {
        var video = await videoService.GetVideoAsync(id);
        if (video.IsFailure)
            return NotFound(video.GetErrorOrThrow());

        var result = await videoService.UpdateVideoAsync(id, videoRequest);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
    }

    /// <summary>
    /// Stream a video file by its ID. If running in Docker, redirects to Nginx URL; otherwise, serves the file directly.
    /// </summary>
    /// <param name="id">The ID of the video to be streamed.</param>
    /// <returns>A redirect to the Nginx URL or the video file stream. Might produce 200, 302, 400 or 206 Partial Content for range requests.</returns>
    [HttpGet("{id:int}/stream")]
    public async Task<IActionResult> GetVideoStreamAsync(int id)
    {
        var videoOptional = await videoService.GetVideoAsync(id);
        if (videoOptional.IsFailure)
        {
            return NotFound(videoOptional.GetErrorOrThrow());
        }

        var video = videoOptional.GetValueOrThrow();

        if (DockerDetector.IsRunningInDocker())
        {
            var baseUrl = configuration["Videos:NginxUrl"];
            var path = $"{baseUrl}/{video.VideoGroup.Project.Id}/{video.VideoGroupId}/{Path.GetFileName(video.Path)}";
            return Redirect(path);
        }
        else
        {
            return File(video.ToStream(), video.ContentType, Path.GetFileName(video.Path), enableRangeProcessing: true);
        }
    }

    /// <summary>
    /// Delete a video by its ID.
    /// </summary>
    /// <param name="id">The ID of the video to be deleted.</param>
    /// <returns>204 No Content if the deletion is successful or 404 Not Found if the video does not exist.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteVideoAsync(int id)
    {
        var video = await videoService.GetVideoAsync(id);
        if (video.IsFailure)
            return NotFound(video.GetErrorOrThrow());

        await videoService.DeleteVideoAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Get assigned labels for a specific video by its ID.
    /// </summary>
    /// <param name="id">The ID of the video.</param>
    /// <returns>A collection of assigned labels for the specified video.</returns>
    [HttpGet("{id:int}/assigned-labels")]
    public async Task<ActionResult<IEnumerable<AssignedLabelResponse>>> GetAssignedLabelsByVideoIdAsync(int id)
    {
        var assignedLabels = await assignedLabelService.GetAssignedLabelsByVideoIdAsync(id);
        return assignedLabels.IsSuccess
            ? Ok(mapper.Map<IEnumerable<AssignedLabelResponse>>(assignedLabels.GetValueOrThrow()))
            : NotFound(assignedLabels.GetErrorOrThrow());
    }

    /// <summary>
    /// Get assigned labels for a specific video and subject by their IDs.
    /// </summary>
    /// <param name="videoId">The ID of the video.</param>
    /// <param name="subjectId">The ID of the subject.</param>
    /// <returns>A collection of assigned labels for the specified video and subject.</returns>
    [HttpGet("{videoId:int}/{subjectId:int}/assigned-labels")]
    public async Task<ActionResult<IEnumerable<AssignedLabelResponse>>> GetAssignedLabelsByVideoIdAndSubjectIdAsync(int videoId, int subjectId)
    {
        var assignedLabels = await assignedLabelService.GetAssignedLabelsByVideoIdAndSubjectIdAsync(videoId, subjectId);
        return assignedLabels.IsSuccess
            ? Ok(mapper.Map<IEnumerable<AssignedLabelResponse>>(assignedLabels.GetValueOrThrow()))
            : NotFound(assignedLabels.GetErrorOrThrow());
    }
}