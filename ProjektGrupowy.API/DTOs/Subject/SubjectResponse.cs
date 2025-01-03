namespace ProjektGrupowy.API.DTOs.Subject;

public class SubjectResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int ProjectId { get; set; }
}