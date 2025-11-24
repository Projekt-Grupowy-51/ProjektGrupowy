namespace ProjektGrupowy.API.DTOs.Video;

/// <summary>
/// DTO for video stream URL response
/// </summary>
public class VideoStreamUrlResponse
{
    /// <summary>
    /// The URL for streaming the video.
    /// </summary>
    public string Url { get; set; } = string.Empty;
}