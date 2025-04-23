using Microsoft.AspNetCore.Identity;

namespace ProjektGrupowy.API.Models;

public class User : IdentityUser
{
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public virtual ICollection<Project> OwnedProjects { get; set; }
    public virtual ICollection<Project> LabeledProjects { get; set; }
    public virtual ICollection<SubjectVideoGroupAssignment> OwnedAssignments { get; set; }
    public virtual ICollection<SubjectVideoGroupAssignment> LabeledAssignments { get; set; }

}
