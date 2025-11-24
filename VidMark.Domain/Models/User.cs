namespace VidMark.Domain.Models;

public class User
{
    public string Id { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool Enabled { get; set; }

    public long CreatedTimestamp { get; set; }

    public virtual ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
    public virtual ICollection<Project> LabeledProjects { get; set; } = new List<Project>();
    public virtual ICollection<SubjectVideoGroupAssignment> OwnedAssignments { get; set; } = new List<SubjectVideoGroupAssignment>();
    public virtual ICollection<SubjectVideoGroupAssignment> LabeledAssignments { get; set; } = new List<SubjectVideoGroupAssignment>();

    public DateTime RegistrationDate => DateTimeOffset.FromUnixTimeMilliseconds(CreatedTimestamp).DateTime;
}
