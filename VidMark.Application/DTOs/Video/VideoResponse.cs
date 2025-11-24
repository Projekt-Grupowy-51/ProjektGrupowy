namespace VidMark.Application.DTOs.Video;

/// <summary>
/// DTO for video response - simple representation of a video
/// </summary>
public class VideoResponse 
{
    /// <summary>
    /// The unique identifier of the video.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The title of the video.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The path where the video is stored.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier of the video group associated with the video.
    /// </summary>
    public int VideoGroupId { get; set; }

    /// <summary>
    /// The content type of the video (e.g., "video/mp4").
    /// </summary>
    public string ContentType { get; set; } = string.Empty;
    
    /// <summary>
    /// The position of the video in the display queue.
    /// </summary>
    
    public int PositionInQueue { get; set; }
    /// <summary>
    /// The available quality options for the video.
    /// </summary>
    public string[] AvailableQualities { get; set; } = [];
    
    /// <summary>
    /// The original quality of the video.
    /// </summary>
    public string OriginalQuality { get; set; } = string.Empty;
}