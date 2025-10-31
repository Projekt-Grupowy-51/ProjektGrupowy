using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Utils;
using ProjektGrupowy.Application.DTOs.AssignedLabel;
using ProjektGrupowy.Application.DTOs.Video;
using ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabelsByVideoId;
using ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabelsByVideoIdAndSubjectId;
using ProjektGrupowy.Application.Features.AssignedLabels.Queries.GetAssignedLabelsPage;
using ProjektGrupowy.Application.Features.Videos.Commands.AddVideo;
using ProjektGrupowy.Application.Features.Videos.Commands.DeleteVideo;
using ProjektGrupowy.Application.Features.Videos.Commands.UpdateVideo;
using ProjektGrupowy.Application.Features.Videos.Queries.GetVideo;
using ProjektGrupowy.Application.Features.Videos.Queries.GetVideos;
using ProjektGrupowy.Application.Features.Videos.Queries.GetVideosByGroupAndPosition;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

/// <summary>
/// Controller for managing videos. Handles operations such as retrieving, adding, updating, deleting, and streaming videos.
/// </summary>
[Route("api/videos")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class VideoController(
    IMediator mediator,
    ICurrentUserService currentUserService,
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
        var query = new GetVideosQuery(currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<VideoResponse>>(result.Value);
        return Ok(response);
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
        var query = new GetVideosByGroupAndPositionQuery(videoGroupId, positionInQueue, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<VideoResponse>>(result.Value);
        return Ok(response);
    }

    /// <summary>
    /// Get a specific video by its ID.
    /// </summary>
    /// <param name="id">The ID of the video.</param>
    /// <returns>The video with the specified ID.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VideoResponse>> GetVideoAsync(int id)
    {
        var query = new GetVideoQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<VideoResponse>(result.Value);
        return Ok(response);
    }

    /// <summary>
    /// Add a new video.
    /// </summary>
    /// <param name="videoRequest">The request containing the details of the video to be added.</param>
    /// <returns>201 Created with the created video details or 400 Bad Request if the operation fails.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<VideoResponse>> AddVideoAsync([FromForm] VideoRequest videoRequest)
    {
        var command = new AddVideoCommand(
            videoRequest.Title,
            videoRequest.File,
            videoRequest.VideoGroupId,
            videoRequest.PositionInQueue,
            currentUserService.UserId,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        var response = mapper.Map<VideoResponse>(result.Value);
        return CreatedAtAction("GetVideo", new { id = result.Value.Id }, response);
    }

    /// <summary>
    /// Update an existing video.
    /// </summary>
    /// <param name="id">The ID of the video to be updated.</param>
    /// <param name="videoRequest">The request containing the updated details of the video.</param>
    /// <returns>204 No Content if the update is successful or 400 Bad Request if the operation fails.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutVideoAsync(int id, [FromForm] VideoRequest videoRequest)
    {
        var command = new UpdateVideoCommand(
            id,
            videoRequest.Title,
            videoRequest.File,
            videoRequest.VideoGroupId,
            videoRequest.PositionInQueue,
            currentUserService.UserId,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.Errors);
    }

    /// <summary>
    /// Stream a video file by its ID. If running in Docker, redirects to Nginx URL; otherwise, serves the file directly.
    /// </summary>
    /// <param name="id">The ID of the video to be streamed.</param>
    /// <returns>A redirect to the Nginx URL or the video file stream. Might produce 200, 302, 400 or 206 Partial Content for range requests.</returns>
    [HttpGet("{id:int}/stream")]
    public async Task<IActionResult> GetVideoStreamAsync(int id)
    {
        var query = new GetVideoQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var video = result.Value;

        if (DockerDetector.IsRunningInDocker())
        {
            var baseUrl = configuration["Videos:NginxUrl"];
            var path = $"{baseUrl}/{video.VideoGroup.Project.Id}/{video.VideoGroupId}/{Path.GetFileName(video.Path)}";
            return Redirect(path);
        }

        return File(video.ToStream(), video.ContentType, Path.GetFileName(video.Path), enableRangeProcessing: true);
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
        var command = new DeleteVideoCommand(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? NoContent()
            : NotFound(result.Errors);
    }

    /// <summary>
    /// Get assigned labels for a specific video by its ID.
    /// </summary>
    /// <param name="id">The ID of the video.</param>
    /// <returns>A collection of assigned labels for the specified video.</returns>
    [HttpGet("{id:int}/assigned-labels")]
    public async Task<ActionResult<IEnumerable<AssignedLabelResponse>>> GetAssignedLabelsByVideoIdAsync(int id)
    {
        var query = new GetAssignedLabelsByVideoIdQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<AssignedLabelResponse>>(result.Value);
        return Ok(response);
    }

    /// <summary>
    /// Get a paginated list of assigned labels.
    /// </summary>
    /// <param name="id">The ID of the video.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated list of assigned labels.</returns>
    [HttpGet("{id:int}/assigned-labels/page")]
    public async Task<ActionResult<AssignedLabelPageResponse>> GetAssignedLabelsPageAsync(
        [FromRoute] int id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var pageQuery = new GetAssignedLabelsPageQuery(
            id,
            pageNumber,
            pageSize,
            currentUserService.UserId,
            currentUserService.IsAdmin);
        
        var countQuery = new GetAssignedLabelsByVideoIdQuery(
            id,
            currentUserService.UserId,
            currentUserService.IsAdmin);

        var pageResult = await mediator.Send(pageQuery);
        if (pageResult.IsFailed)
            return NotFound(pageResult.Errors);
        
        var countResult = await mediator.Send(countQuery);
        if (countResult.IsFailed)
            return NotFound(countResult.Errors);

        return Ok(new AssignedLabelPageResponse
        {
            AssignedLabels = mapper.Map<List<AssignedLabelResponse>>(pageResult.Value),
            TotalLabelCount = countResult.Value.Count
        });
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
        var query = new GetAssignedLabelsByVideoIdAndSubjectIdQuery(videoId, subjectId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<AssignedLabelResponse>>(result.Value);
        return Ok(response);
    }
}
