using System.ComponentModel.DataAnnotations;

namespace VidMark.Application.DTOs.Label;

/// <summary>
/// DTO for label request
/// </summary>
public class LabelRequest
{
    /// <summary>
    /// The name of the label.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The hexadecimal color code representing the label's color (exactly 7 characters, e.g., "#FFFFFF").
    /// </summary>
    [StringLength(7, MinimumLength = 7, ErrorMessage = "Hex koloru musi mieć 7 znaków")]
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