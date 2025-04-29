using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Video;
using ProjektGrupowy.API.DTOs.VideoGroup;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils.Constants;
using ProjektGrupowy.API.Utils.Extensions;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class VideoGroupController(
    IVideoGroupService videoGroupService,
    IVideoService videoService,
    IMapper mapper) : ControllerBase
{
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VideoGroupResponse>>> GetVideoGroupsAsync()
    {
        var videoGroups = await videoGroupService.GetVideoGroupsAsync();

        return videoGroups.IsSuccess
            ? Ok(mapper.Map<IEnumerable<VideoGroupResponse>>(videoGroups.GetValueOrThrow()))
            : NotFound(videoGroups.GetErrorOrThrow());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VideoGroupResponse>> GetVideoGroupAsync(int id)
    {
        var videoGroupResult = await videoGroupService.GetVideoGroupAsync(id);
        return videoGroupResult.IsSuccess
            ? Ok(mapper.Map<VideoGroupResponse>(videoGroupResult.GetValueOrThrow()))
            : NotFound(videoGroupResult.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<VideoGroupResponse>> PostVideoGroupAsync(VideoGroupRequest videoGroupRequest)
    {
        videoGroupRequest.OwnerId = User.GetUserId();

        var result = await videoGroupService.AddVideoGroupAsync(videoGroupRequest);
        if (result.IsFailure)
            return BadRequest(result.GetErrorOrThrow());

        var createdVideoGroup = result.GetValueOrThrow();
        var videoGroupResponse = mapper.Map<VideoGroupResponse>(createdVideoGroup);

        return CreatedAtAction("GetVideoGroup", new { id = createdVideoGroup.Id }, videoGroupResponse);
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutVideoGroupAsync(int id, VideoGroupRequest videoGroupRequest)
    {
        videoGroupRequest.OwnerId = User.GetUserId();

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

    [HttpGet("{id:int}/videos")]
    public async Task<ActionResult<IEnumerable<VideoResponse>>> GetVideosByVideoGroupIdAsync(int id)
    {
        var videosResult = await videoGroupService.GetVideosByVideoGroupIdAsync(id);
        return videosResult.IsSuccess
            ? Ok(mapper.Map<IEnumerable<VideoResponse>>(videosResult.GetValueOrThrow()))
            : NotFound(videosResult.GetErrorOrThrow());
    }
}