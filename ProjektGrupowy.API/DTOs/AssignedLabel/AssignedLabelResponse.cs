namespace ProjektGrupowy.API.DTOs.AssignedLabel;

public class AssignedLabelResponse
{
    public int Id { get; set; }
    public int LabelId { get; set; }
    public string LabelName { get; set; }
    public int VideoId { get; set; }
    public string LabelerId { get; set; }
    public string LabelerName { get; set; }
    public string Start { get; set; }
    public string End { get; set; }
    public DateTime InsDate { get; set; }
}