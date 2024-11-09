namespace ProjektGrupowy.API.DTOs.Video;

public class VideoRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public IFormFile File { get; set; }
}