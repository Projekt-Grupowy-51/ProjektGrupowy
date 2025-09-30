namespace ProjektGrupowy.Application.DTOs.ProjectReport;

/// <summary>
/// DTO for generated report response - simple representation of a generated report
/// </summary>
public class GeneratedReportResponse
{
    /// <summary>
    /// The unique identifier of the generated report.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The name of the generated report.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The date and time when the report was created in UTC.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }
}