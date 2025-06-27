using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.DTOs.AssignedLabel;
using ProjektGrupowy.Application.DTOs.Video;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
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
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VideoResponse>>> GetVideosAsync()
    {
        var videos = await videoService.GetVideosAsync();

        return videos.IsSuccess
            ? Ok(mapper.Map<IEnumerable<VideoResponse>>(videos.GetValueOrThrow()))
            : NotFound(videos.GetErrorOrThrow());
    }

    [HttpGet("batch/{videoGroupId:int}/{positionInQueue:int}")]
    public async Task<ActionResult<IEnumerable<VideoResponse>>> GetVideosAsync(int videoGroupId, int positionInQueue)
    {
        var videos = await videoService.GetVideosAsync(videoGroupId, positionInQueue);
        return videos.IsSuccess
            ? Ok(mapper.Map<IEnumerable<VideoResponse>>(videos.GetValueOrThrow()))
            : NotFound(videos.GetErrorOrThrow());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VideoResponse>> GetVideoAsync(int id)
    {
        var video = await videoService.GetVideoAsync(id);
        return video.IsSuccess
            ? Ok(mapper.Map<VideoResponse>(video.GetValueOrThrow()))
            : NotFound(video.GetErrorOrThrow());
    }

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

    [HttpGet("{id:int}/assignedlabels")]
    public async Task<ActionResult<IEnumerable<AssignedLabelResponse>>> GetAssignedLabelsByVideoIdAsync(int id)
    {
        var assignedLabels = await assignedLabelService.GetAssignedLabelsByVideoIdAsync(id);
        return assignedLabels.IsSuccess
            ? Ok(mapper.Map<IEnumerable<AssignedLabelResponse>>(assignedLabels.GetValueOrThrow()))
            : NotFound(assignedLabels.GetErrorOrThrow());
    }

    [HttpGet("{videoId:int}/{subjectId:int}/assignedlabels")]
    public async Task<ActionResult<IEnumerable<AssignedLabelResponse>>> GetAssignedLabelsByVideoIdAndSubjectIdAsync(int videoId, int subjectId)
    {
        var assignedLabels = await assignedLabelService.GetAssignedLabelsByVideoIdAndSubjectIdAsync(videoId, subjectId);
        return assignedLabels.IsSuccess
            ? Ok(mapper.Map<IEnumerable<AssignedLabelResponse>>(assignedLabels.GetValueOrThrow()))
            : NotFound(assignedLabels.GetErrorOrThrow());
    }
}