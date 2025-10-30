using Microsoft.AspNetCore.Http;

namespace ProjektGrupowy.Application.DTOs.Video;

/// <summary>
/// DTO for video request - used for creating or updating a video
/// </summary>
public class VideoRequest
{
    /// <summary>
    /// The title of the video.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The video file to be uploaded.
    /// </summary>
    public IFormFile File { get; set; } = default!;
    
    /// <summary>
    /// The unique identifier of the video group associated with the video.
    /// </summary>
    public int VideoGroupId { get; set; }
    
    /// <summary>
    /// The position of the video in the display queue.
    /// </summary>
    public int PositionInQueue { get; set; }
}