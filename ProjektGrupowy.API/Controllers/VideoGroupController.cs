using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Video;
using ProjektGrupowy.API.DTOs.VideoGroup;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class VideoGroupController(IVideoGroupService videoGroupService, IMapper mapper) : ControllerBase
{
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
        var videoGroup = await videoGroupService.GetVideoGroupAsync(id);
        return videoGroup.IsSuccess
            ? Ok(mapper.Map<VideoGroupResponse>(videoGroup.GetValueOrThrow()))
            : NotFound(videoGroup.GetErrorOrThrow());
    }



    [HttpPost]
    public async Task<ActionResult<VideoGroupResponse>> AddVideoGroupAsync(VideoGroupRequest videoGroupRequest)
    {
        var result = await videoGroupService.AddVideoGroupAsync(videoGroupRequest);

        if (result.IsFailure)
            return BadRequest(result.GetErrorOrThrow());

        var createdVideoGroup = result.GetValueOrThrow();

        var videoGroupResponse = mapper.Map<VideoGroupResponse>(createdVideoGroup);

        return CreatedAtAction("GetVideoGroup", new { id = createdVideoGroup.Id }, videoGroupResponse);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutVideoGroupAsync(int id, VideoGroupRequest videoGroupRequest)
    {
        var result = await videoGroupService.UpdateVideoGroupAsync(id, videoGroupRequest);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteVideoGroupAsync(int id)
    {
        await videoGroupService.DeleteVideoGroupAsync(id);

        return NoContent();
    }

    [HttpGet("{id:int}/videos")]
    public async Task<ActionResult<IEnumerable<VideoResponse>>> GetVideosByVideoGroupIdAsync(int id)
    {
        var videosResult = await videoGroupService.GetVideosByVideoGroupIdAsync(id);

        if (videosResult.IsSuccess)
        {
            return Ok(mapper.Map<IEnumerable<VideoResponse>>(videosResult.GetValueOrThrow()));
        }
        return NotFound(videosResult.GetErrorOrThrow());
    }

}