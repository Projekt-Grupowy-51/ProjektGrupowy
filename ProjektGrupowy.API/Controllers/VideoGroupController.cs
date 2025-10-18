using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.DTOs.Video;
using ProjektGrupowy.Application.DTOs.VideoGroup;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

/// <summary>
/// Controller for managing video groups. Handles operations such as retrieving, adding, updating, deleting video groups, and fetching videos by video group ID.
/// </summary>
/// <param name="videoGroupService"></param>
/// <param name="videoService"></param>
/// <param name="mapper"></param>
[Route("api/video-groups")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class VideoGroupController(
    IVideoGroupService videoGroupService,
    IVideoService videoService,
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
        var videoGroups = await videoGroupService.GetVideoGroupsAsync();

        return videoGroups.IsSuccess
            ? Ok(mapper.Map<IEnumerable<VideoGroupResponse>>(videoGroups.GetValueOrThrow()))
            : NotFound(videoGroups.GetErrorOrThrow());
    }

    /// <summary>
    /// Get a specific video group by its ID.
    /// </summary>
    /// <param name="id">The ID of the video group.</param>
    /// <returns>The video group with the specified ID.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VideoGroupResponse>> GetVideoGroupAsync(int id)
    {
        var videoGroupResult = await videoGroupService.GetVideoGroupAsync(id);
        return videoGroupResult.IsSuccess
            ? Ok(mapper.Map<VideoGroupResponse>(videoGroupResult.GetValueOrThrow()))
            : NotFound(videoGroupResult.GetErrorOrThrow());
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
        var result = await videoGroupService.AddVideoGroupAsync(videoGroupRequest);
        if (result.IsFailure)
            return BadRequest(result.GetErrorOrThrow());

        var createdVideoGroup = result.GetValueOrThrow();
        var videoGroupResponse = mapper.Map<VideoGroupResponse>(createdVideoGroup);

        return CreatedAtAction("GetVideoGroup", new { id = createdVideoGroup.Id }, videoGroupResponse);
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
        var videoGroupResult = await videoGroupService.GetVideoGroupAsync(id);
        if (videoGroupResult.IsFailure)
        {
            return NotFound(videoGroupResult.GetErrorOrThrow());
        }

        var result = await videoGroupService.UpdateVideoGroupAsync(id, videoGroupRequest);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
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
        var videoGroupResult = await videoGroupService.GetVideoGroupAsync(id);
        if (videoGroupResult.IsFailure)
        {
            return NotFound(videoGroupResult.GetErrorOrThrow());
        }

        await videoGroupService.DeleteVideoGroupAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Get all videos associated with a specific video group ID.
    /// </summary>
    /// <param name="id">The ID of the video group.</param>
    /// <returns>A collection of videos associated with the specified video group ID.</returns>
    [HttpGet("{id:int}/videos")]
    public async Task<ActionResult<IEnumerable<VideoResponse>>> GetVideosByVideoGroupIdAsync(int id)
    {
        var videosResult = await videoGroupService.GetVideosByVideoGroupIdAsync(id);
        return videosResult.IsSuccess
            ? Ok(mapper.Map<IEnumerable<VideoResponse>>(videosResult.GetValueOrThrow()))
            : NotFound(videosResult.GetErrorOrThrow());
    }
}