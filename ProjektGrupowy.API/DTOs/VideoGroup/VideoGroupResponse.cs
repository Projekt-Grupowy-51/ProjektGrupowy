namespace ProjektGrupowy.API.DTOs.VideoGroup;

public class VideoGroupResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ProjectId { get; set; }
    // public int VideoCount { get; set; }
    public Dictionary<int, int> VideosAtPositions { get; set; }
}