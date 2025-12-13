using System.Text.Json.Serialization;

namespace VidMark.API.DTOs.SubjectVideoGroupAssignment;

public class ToggleCompletionRequest
{
    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; }
}
