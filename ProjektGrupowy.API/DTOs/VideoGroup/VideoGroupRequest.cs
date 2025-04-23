namespace ProjektGrupowy.API.DTOs.VideoGroup;

public class VideoGroupRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int ProjectId { get; set; }
    public string OwnerId { get; set; } = string.Empty;
}