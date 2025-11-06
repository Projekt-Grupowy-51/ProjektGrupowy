namespace ProjektGrupowy.Application.DTOs.AssignedLabel;

/// <summary>
/// DTO for assigned label page response
/// </summary>
public class AssignedLabelPageResponse
{
    /// <summary>
    /// The list of assigned labels.
    /// </summary>
    public List<AssignedLabelResponse> AssignedLabels { get; set; } = [];
    
    /// <summary>
    /// The total count of labels available.
    /// </summary>
    public int TotalLabelCount { get; set; }
}