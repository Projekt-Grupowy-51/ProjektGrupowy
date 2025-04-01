namespace ProjektGrupowy.API.DTOs.AssignedLabel;

public class AssignedLabelResponse
{
    public int Id { get; set; }
    public int LabelId { get; set; }
    public int VideoId { get; set; }
    public int LabelerId { get; set; }
    public string Start { get; set; }
    public string End { get; set; }
    public DateTime InsDate { get; set; }
}