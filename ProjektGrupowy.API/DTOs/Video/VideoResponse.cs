namespace ProjektGrupowy.API.DTOs.Video;

public class VideoResponse 
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Path { get; set; }
    public int VideoGroupId { get; set; }
    public string ContentType { get; set; }
    public int PositionInQueue { get; set; }
}