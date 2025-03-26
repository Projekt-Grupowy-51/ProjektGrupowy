using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Video;
using ProjektGrupowy.API.DTOs.VideoGroup;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class VideoGroupController(
    IVideoGroupService videoGroupService,
    IVideoService videoService,
    IAuthorizationHelper authHelper,
    IMapper mapper) : ControllerBase
{
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VideoGroupResponse>>> GetVideoGroupsAsync()
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        var videoGroups = checkResult.IsAdmin
            ? await videoGroupService.GetVideoGroupsAsync()
            : await videoGroupService.GetVideoGroupsByScientistIdAsync(checkResult.Scientist!.Id);

        return videoGroups.IsSuccess
            ? Ok(mapper.Map<IEnumerable<VideoGroupResponse>>(videoGroups.GetValueOrThrow()))
            : NotFound(videoGroups.GetErrorOrThrow());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VideoGroupResponse>> GetVideoGroupAsync(int id)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsVideoGroupAsync(User, id);
            if (authResult != null)
            {
                return authResult;
            }
        }

        if (checkResult.IsLabeler)
        {
            var authResult = await authHelper.CanLabelerAccessVideoGroupAsync(User, id);
            if (!authResult)
            {
                return Forbid();
            }
        }

        var videoGroupResult = await videoGroupService.GetVideoGroupAsync(id);
        return videoGroupResult.IsSuccess
            ? Ok(mapper.Map<VideoGroupResponse>(videoGroupResult.GetValueOrThrow()))
            : NotFound(videoGroupResult.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<VideoGroupResponse>> PostVideoGroupAsync(VideoGroupRequest videoGroupRequest)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsProjectAsync(User, videoGroupRequest.ProjectId);
            if (authResult != null)
            {
                return authResult;
            }
        }

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
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        var videoGroupResult = await videoGroupService.GetVideoGroupAsync(id);
        if (videoGroupResult.IsFailure)
        {
            return NotFound(new { Message = "VideoGroup not found" });
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsVideoGroupAsync(User, id);
            if (authResult != null)
            {
                return authResult;
            }
            
            var projectAuthResult = await authHelper.EnsureScientistOwnsProjectAsync(User, videoGroupRequest.ProjectId);
            if (projectAuthResult != null)
            {
                return projectAuthResult;
            }
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
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        var videoGroupResult = await videoGroupService.GetVideoGroupAsync(id);
        if (videoGroupResult.IsFailure)
        {
            return NotFound(new { Message = "VideoGroup not found" });
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsVideoGroupAsync(User, id);
            if (authResult != null)
            {
                return authResult;
            }
        }

        await videoGroupService.DeleteVideoGroupAsync(id);
        return NoContent();
    }

    [HttpGet("{id:int}/videos")]
    public async Task<ActionResult<IEnumerable<VideoResponse>>> GetVideosByVideoGroupIdAsync(int id)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsVideoGroupAsync(User, id);
            if (authResult != null)
            {
                return authResult;
            }
        }

        if (checkResult.IsLabeler)
        {
            var authResult = await authHelper.CanLabelerAccessVideoGroupAsync(User, id);
            if (!authResult)
            {
                return Forbid();
            }
        }

        var videosResult = await videoGroupService.GetVideosByVideoGroupIdAsync(id);
        return videosResult.IsSuccess
            ? Ok(mapper.Map<IEnumerable<VideoResponse>>(videosResult.GetValueOrThrow()))
            : NotFound(videosResult.GetErrorOrThrow());
    }
}