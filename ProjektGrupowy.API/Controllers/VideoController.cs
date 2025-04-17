using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.AssignedLabel;
using ProjektGrupowy.API.DTOs.Video;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class VideoController(
    IVideoService videoService,
    IAssignedLabelService assignedLabelService,
    IAuthorizationHelper authHelper,
    IMapper mapper) : ControllerBase
{
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VideoResponse>>> GetVideosAsync()
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        var videos = checkResult.IsAdmin
            ? await videoService.GetVideosAsync()
            : await videoService.GetVideosByScientistIdAsync(checkResult.Scientist!.Id);
            
        return videos.IsSuccess
            ? Ok(mapper.Map<IEnumerable<VideoResponse>>(videos.GetValueOrThrow()))
            : NotFound(videos.GetErrorOrThrow());
    }

    [HttpGet("batch/{videoGroupId:int}/{positionInQueue:int}")]
    public async Task<ActionResult<IEnumerable<VideoResponse>>> GetVideosAsync(int videoGroupId, int positionInQueue)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsVideoGroupAsync(User, videoGroupId);
            if (authResult != null)
            {
                return authResult;
            }
        }

        if (checkResult.IsLabeler)
        {
            var authResult = await authHelper.CanLabelerAccessVideoGroupAsync(User, videoGroupId);
            if (!authResult)
            {
                return Forbid();
            }
        }

        var videos = await videoService.GetVideosAsync(videoGroupId, positionInQueue);
        return videos.IsSuccess
            ? Ok(mapper.Map<IEnumerable<VideoResponse>>(videos.GetValueOrThrow()))
            : NotFound(videos.GetErrorOrThrow());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VideoResponse>> GetVideoAsync(int id)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsVideoAsync(User, id);
            if (authResult != null)
            {
                return authResult;
            }
        }

        if (checkResult.IsLabeler) 
        {
            var authResult = await authHelper.EnsureLabelerCanAccessVideoAsync(User, id);
            if (authResult != null)
            {
                return authResult;
            }
        }

        var video = await videoService.GetVideoAsync(id);
        return video.IsSuccess
            ? Ok(mapper.Map<VideoResponse>(video.GetValueOrThrow()))
            : NotFound(video.GetErrorOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost]
    public async Task<ActionResult<VideoResponse>> AddVideoAsync(VideoRequest videoRequest)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsVideoGroupAsync(User, videoRequest.VideoGroupId);
            if (authResult != null)
            {
                return authResult;
            }
        }

        var result = await videoService.AddVideoAsync(videoRequest);

        if (result.IsFailure)
            return BadRequest(result.GetErrorOrThrow());

        var createdVideo = result.GetValueOrThrow();

        var videoResponse = mapper.Map<VideoResponse>(createdVideo);

        return CreatedAtAction("GetVideo", new { id = createdVideo.Id }, videoResponse);
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutVideoAsync(int id, VideoRequest videoRequest)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var videoAuthResult = await authHelper.EnsureScientistOwnsVideoAsync(User, id);
            if (videoAuthResult != null)
            {
                return videoAuthResult;
            }
            
            var videoGroupAuthResult = await authHelper.EnsureScientistOwnsVideoGroupAsync(User, videoRequest.VideoGroupId);
            if (videoGroupAuthResult != null)
            {
                return videoGroupAuthResult;
            }
        }

        var result = await videoService.UpdateVideoAsync(id, videoRequest);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
    }

    [HttpGet("{id:int}/stream")]
    public async Task<IActionResult> GetVideoStreamAsync(int id)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsVideoAsync(User, id);
            if (authResult != null)
            {
                return authResult;
            }
        }

        if (checkResult.IsLabeler) 
        {
            var authResult = await authHelper.EnsureLabelerCanAccessVideoAsync(User, id);
            if (authResult != null)
            {
                return authResult;
            }
        }

        var videoOptional = await videoService.GetVideoAsync(id);
        if (videoOptional.IsFailure)
        {
            return NotFound(videoOptional.GetErrorOrThrow());
        }

        var video = videoOptional.GetValueOrThrow();
        var path = $"http://localhost:8080/videos/{video.VideoGroup.Project.Id}/{video.VideoGroupId}/{Path.GetFileName(video.Path)}";
        return Redirect(path);
        // return File(
        //     video.ToStream(),
        //     video.ContentType, 
        //     Path.GetFileName(video.Path), 
        //     enableRangeProcessing: true);
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteVideoAsync(int id)
    {
        var checkResult = await authHelper.CheckGeneralAccessAsync(User);
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            var authResult = await authHelper.EnsureScientistOwnsVideoAsync(User, id);
            if (authResult != null)
            {
                return authResult;
            }
        }

        await videoService.DeleteVideoAsync(id);
        return NoContent();
    }

    [HttpGet("{id:int}/assignedlabels")]
    public async Task<ActionResult<IEnumerable<AssignedLabelResponse>>> GetAssignedLabelsByVideoIdAsync(int id)
    {
        if (User.IsInRole(RoleConstants.Labeler))
        {
            var labelerResult = await authHelper.GetLabelerFromUserAsync(User);
            if (labelerResult.Error != null)
                return labelerResult.Error;

            var authResult = await authHelper.EnsureLabelerCanAccessVideoAsync(User, id);
            if (authResult != null)
                return authResult;

            var labelerAssignedLabels = await assignedLabelService.GetAssignedLabelsByLabelerIdAsync(labelerResult.Labeler!.Id);
            var filteredLabels = labelerAssignedLabels.GetValueOrThrow().Where(al => al.Video.Id == id);
            
            return Ok(mapper.Map<IEnumerable<AssignedLabelResponse>>(filteredLabels));
        }
        else
        {
            var checkResult = await authHelper.CheckGeneralAccessAsync(User);
            if (checkResult.Error != null)
                return checkResult.Error;

            if (checkResult.IsScientist)
            {
                var authResult = await authHelper.EnsureScientistOwnsVideoAsync(User, id);
                if (authResult != null)
                    return authResult;
            }

            var video = await videoService.GetVideoAsync(id);
            if (!video.IsSuccess)
                return NotFound(video.GetErrorOrThrow());

            var assignedLabels = await assignedLabelService.GetAssignedLabelsByVideoIdAsync(id);
            
            return assignedLabels.IsSuccess
                ? Ok(mapper.Map<IEnumerable<AssignedLabelResponse>>(assignedLabels.GetValueOrThrow()))
                : NotFound(assignedLabels.GetErrorOrThrow());
        }
    }
}