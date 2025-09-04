using System.Text.Json.Serialization;

namespace ProjektGrupowy.Application.DTOs.Project;

public class ProjectStatsResponse
{
    [JsonPropertyName("subjects")]
    public int Subjects { get; set; }
    
    [JsonPropertyName("videos")]
    public int Videos { get; set; }
    
    [JsonPropertyName("assignments")]
    public int Assignments { get; set; }
    
    [JsonPropertyName("labelers")]
    public int Labelers { get; set; }
    
    [JsonPropertyName("access_codes")]
    public int AccessCodes { get; set; }
}