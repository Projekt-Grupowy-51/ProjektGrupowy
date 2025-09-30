using System.Text.Json.Serialization;

namespace ProjektGrupowy.Application.DTOs.Project;

public class ProjectStatsResponse
{
    /// <summary>
    /// Number of subjects in the project.
    /// </summary>
    [JsonPropertyName("subjects")]
    public int Subjects { get; set; }
    
    /// <summary>
    /// Number of videos in the project.
    /// </summary>
    [JsonPropertyName("videos")]
    public int Videos { get; set; }
    
    /// <summary>
    /// Number of assignments in the project.
    /// </summary>
    [JsonPropertyName("assignments")]
    public int Assignments { get; set; }
    
    /// <summary>
    /// Number of labels in the project.
    /// </summary>
    [JsonPropertyName("labelers")]
    public int Labelers { get; set; }
    
    /// <summary>
    /// Number of access codes in the project.
    /// </summary>
    [JsonPropertyName("access_codes")]
    public int AccessCodes { get; set; }
}