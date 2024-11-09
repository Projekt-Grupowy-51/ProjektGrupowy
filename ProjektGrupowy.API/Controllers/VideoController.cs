using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Video;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
public class VideoController(IVideoService videoService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VideoResponse>>> GetVideosAsync()
    {
        var videos = await videoService.GetVideosAsync();
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

    [HttpGet("stream/{id:int}")]
    public async Task<IActionResult> GetVideoStreamAsync(int id)
    {
        var videoOptional = await videoService.GetVideoAsync(id);
        return videoOptional.IsSuccess
            ? File(videoOptional.GetValueOrThrow().ToStream(), "video/mp4", enableRangeProcessing: true)
            : NotFound(videoOptional.GetErrorOrThrow());
    }

    [HttpGet("project/{projectId:int}")]
    public async Task<ActionResult> GetVideosFromProjectAsync(int projectId)
    {
        var videos = await videoService.GetVideosFromProjectAsync(projectId);
        return videos.IsSuccess
            ? Ok(mapper.Map<IEnumerable<VideoResponse>>(videos.GetValueOrThrow()))
            : NotFound(videos.GetErrorOrThrow());
    }

    [HttpPost("project/{projectId:int}")]
    public async Task<ActionResult> AddVideoToProjectAsync(int projectId, VideoRequest videoRequest) {
        var video = await videoService.AddVideoToProjectAsync(projectId, videoRequest);
        return video.IsSuccess
            ? CreatedAtAction("GetVideo", new { id = video.GetValueOrThrow().Id }, video.GetValueOrThrow())
            : BadRequest(video.GetErrorOrThrow());
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteVideoAsync(int id)
    {
        await videoService.DeleteVideoAsync(id);
        return NoContent();
    }
}