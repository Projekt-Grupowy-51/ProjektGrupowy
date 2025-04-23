using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.API.DTOs.Label;

public class LabelRequest
{
    public string Name { get; set; }
    
    [StringLength(7, MinimumLength = 7, ErrorMessage = "Hex koloru musi mieć 7 znaków")]
    public string ColorHex { get; set; }
    public string Type { get; set; }
    public char Shortcut { get; set; }
    public int SubjectId { get; set; }
    public string OwnerId { get; set; } = string.Empty;
}