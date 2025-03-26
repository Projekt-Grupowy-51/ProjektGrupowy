using ProjektGrupowy.API.DTOs.Video;

namespace ProjektGrupowy.API.DTOs.Project;

public class ProjectResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int ScientistId { get; set; }
    public DateOnly CreationDate { get; set; }
    public DateOnly? ModificationDate { get; set; }
    public DateOnly? EndDate { get; set; }
}