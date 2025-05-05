using Microsoft.AspNetCore.Identity;

namespace ProjektGrupowy.API.Models;

public class User : IdentityUser
{
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public virtual ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
    public virtual ICollection<Project> LabeledProjects { get; set; } = new List<Project>();
    public virtual ICollection<SubjectVideoGroupAssignment> OwnedAssignments { get; set; } = new List<SubjectVideoGroupAssignment>();
    public virtual ICollection<SubjectVideoGroupAssignment> LabeledAssignments { get; set; } = new List<SubjectVideoGroupAssignment>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
