using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VidMark.API.DTOs.Video;
using VidMark.API.DTOs.VideoGroup;
using VidMark.API.Filters;
using VidMark.Application.Features.VideoGroups.Commands.AddVideoGroup;
using VidMark.Application.Features.VideoGroups.Commands.DeleteVideoGroup;
using VidMark.Application.Features.VideoGroups.Commands.UpdateVideoGroup;
using VidMark.Application.Features.VideoGroups.Queries.GetVideoGroup;
using VidMark.Application.Features.VideoGroups.Queries.GetVideoGroups;
using VidMark.Application.Features.VideoGroups.Queries.GetVideosByVideoGroupId;
using VidMark.Application.Services;
using VidMark.Domain.Utils.Constants;

namespace VidMark.API.Controllers;

/// <summary>
/// Controller for managing video groups. Handles operations such as retrieving, adding, updating, deleting video groups, and fetching videos by video group ID.
/// </summary>
[Route("api/video-groups")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class VideoGroupController(
    IMediator mediator,
    ICurrentUserService currentUserService,
    IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Get all video groups.
    /// </summary>
    /// <returns>A collection of video groups.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VideoGroupResponse>>> GetVideoGroupsAsync()
    {
        var query = new GetVideoGroupsQuery(currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<VideoGroupResponse>>(result.Value);
        return Ok(response);
    }

    /// <summary>
    /// Get a specific video group by its ID.
    /// </summary>
    /// <param name="id">The ID of the video group.</param>
    /// <returns>The video group with the specified ID.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VideoGroupResponse>> GetVideoGroupAsync(int id)
    {
        var query = new GetVideoGroupQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<VideoGroupResponse>(result.Value);
        return Ok(response);
    }

    /// <summary>
    /// Add a new video group.
    /// </summary>
    /// <param name="videoGroupRequest">The video group request containing the details of the new video group.</param>
    /// <returns>201 Created with the details of the created video group or 400 Bad Request if the request fails.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<VideoGroupResponse>> PostVideoGroupAsync(VideoGroupRequest videoGroupRequest)
    {
        var command = new AddVideoGroupCommand(
            videoGroupRequest.Name,
            videoGroupRequest.Description,
            videoGroupRequest.ProjectId,
            currentUserService.UserId,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        var response = mapper.Map<VideoGroupResponse>(result.Value);
        return CreatedAtAction("GetVideoGroup", new { id = result.Value.Id }, response);
    }

    /// <summary>
    /// Update an existing video group.
    /// </summary>
    /// <param name="id">The ID of the video group to be updated.</param>
    /// <param name="videoGroupRequest">The video group request containing the updated details of the video group.</param>
    /// <returns>204 No Content if the update is successful, 400 Bad Request if the request fails, or 404 Not Found if the video group does not exist.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutVideoGroupAsync(int id, VideoGroupRequest videoGroupRequest)
    {
        var command = new UpdateVideoGroupCommand(
            id,
            videoGroupRequest.Name,
            videoGroupRequest.Description,
            videoGroupRequest.ProjectId,
            currentUserService.UserId,
            currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.Errors);
    }

    /// <summary>
    /// Delete a video group by its ID.
    /// </summary>
    /// <param name="id">The ID of the video group to be deleted.</param>
    /// <returns>204 No Content if the deletion is successful or 404 Not Found if the video group does not exist.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteVideoGroupAsync(int id)
    {
        var command = new DeleteVideoGroupCommand(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? NoContent()
            : NotFound(result.Errors);
    }

    /// <summary>
    /// Get all videos associated with a specific video group ID.
    /// </summary>
    /// <param name="id">The ID of the video group.</param>
    /// <returns>A collection of videos associated with the specified video group ID.</returns>
    [HttpGet("{id:int}/videos")]
    public async Task<ActionResult<IEnumerable<VideoResponse>>> GetVideosByVideoGroupIdAsync(int id)
    {
        var query = new GetVideosByVideoGroupIdQuery(id, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<VideoResponse>>(result.Value);
        return Ok(response);
    }
}
