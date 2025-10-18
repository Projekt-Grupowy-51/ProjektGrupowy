namespace ProjektGrupowy.Application.DTOs.VideoGroup;

/// <summary>
/// DTO for video group request - used for creating or updating a video group
/// </summary>
public class VideoGroupRequest
{
    /// <summary>
    /// The name of the video group.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The description of the video group.
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// The unique identifier of the project associated with the video group.
    /// </summary>
    public int ProjectId { get; set; }
}