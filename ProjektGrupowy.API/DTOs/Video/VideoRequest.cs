namespace ProjektGrupowy.API.DTOs.Video;

public class VideoRequest
{
    public string Title { get; set; }
    public IFormFile File { get; set; }
    public int VideoGroupId { get; set; }
    public int PositionInQueue { get; set; }
}