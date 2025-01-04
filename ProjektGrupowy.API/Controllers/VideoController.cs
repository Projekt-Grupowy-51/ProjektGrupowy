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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutVideoAsync(int id, VideoRequest videoRequest)
    {
        var result = await videoService.UpdateVideoAsync(id, videoRequest);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
    }

    [HttpGet("{id:int}/stream")]
    public async Task<IActionResult> GetVideoStreamAsync(int id)
    {
        var videoOptional = await videoService.GetVideoAsync(id);
        return videoOptional.IsSuccess
            ? File(videoOptional.GetValueOrThrow().ToStream(), "video/mp4", enableRangeProcessing: true)
            : NotFound(videoOptional.GetErrorOrThrow());
    }


    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteVideoAsync(int id)
    {
        await videoService.DeleteVideoAsync(id);
        return NoContent();
    }
}