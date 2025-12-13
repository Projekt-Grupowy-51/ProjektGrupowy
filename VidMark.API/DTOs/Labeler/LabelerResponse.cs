namespace VidMark.API.DTOs.Labeler;

/// <summary>
/// DTO for labeler response - simple representation of a labeler
/// </summary>
public class LabelerResponse
{
    /// <summary>
    /// The unique identifier of the labeler.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The name of the labeler.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}