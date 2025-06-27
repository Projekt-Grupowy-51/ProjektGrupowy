using ProjektGrupowy.Application.DTOs.Video;

namespace ProjektGrupowy.Application.DTOs.Project;

public class ProjectResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ScientistId { get; set; }
    public DateOnly CreationDate { get; set; }
    public DateOnly? ModificationDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool Finished { get; set; }
}