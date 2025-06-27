using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.Application.DTOs.Subject;

public class SubjectRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int ProjectId { get; set; }
}