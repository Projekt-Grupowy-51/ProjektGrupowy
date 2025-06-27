namespace ProjektGrupowy.Application.DTOs.Label;

public class LabelResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ColorHex { get; set; }
    public string Type { get; set; }
    public char Shortcut { get; set; }
    public int SubjectId { get; set; }
}