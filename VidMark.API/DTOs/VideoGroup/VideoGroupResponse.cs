namespace VidMark.API.DTOs.VideoGroup;

/// <summary>
/// DTO for video group response - simple representation of a video group
/// </summary>
public class    VideoGroupResponse
{
    /// <summary>
    /// The unique identifier of the video group.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The name of the video group.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the video group.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier of the project associated with the video group.
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// A dictionary/map mapping positions to video IDs in the video group.
    /// </summary>
    public Dictionary<int, int> VideosAtPositions { get; set; } = new();
}