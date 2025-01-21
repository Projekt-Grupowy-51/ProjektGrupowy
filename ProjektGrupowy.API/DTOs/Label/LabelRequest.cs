namespace ProjektGrupowy.API.DTOs.Label;

public class LabelRequest
{
    public string Name { get; set; }
    public string ColorHex { get; set; }
    public string Type { get; set; }
    public char Shortcut { get; set; }
    public int SubjectId { get; set; }
}