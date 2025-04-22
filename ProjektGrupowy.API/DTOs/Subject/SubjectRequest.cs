using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.API.DTOs.Subject;

public class SubjectRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int ProjectId { get; set; }
    public string OwnerId { get; set; } = string.Empty;
}