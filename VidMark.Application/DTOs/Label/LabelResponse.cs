namespace VidMark.Application.DTOs.Label;

/// <summary>
/// DTO for label response - simple representation of a label
/// </summary>
public class LabelResponse
{
    /// <summary>
    /// The unique identifier of the label.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The name of the label.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The hexadecimal color code representing the label's color.
    /// </summary>
    public string ColorHex { get; set; } = string.Empty;

    /// <summary>
    /// The type of the label (e.g., "point" or "interval").
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// A single character used as a shortcut for the label.
    /// </summary>
    public char Shortcut { get; set; }
    
    /// <summary>
    /// The unique identifier of the subject associated with the label.
    /// </summary>
    public int SubjectId { get; set; }
}