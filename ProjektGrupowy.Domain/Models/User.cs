using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.Domain.Models;

[Table("user_entity")]
public class User
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = string.Empty;

    [Column("username")]
    public string UserName { get; set; } = string.Empty;

    [Column("email")]
    public string? Email { get; set; }

    [Column("first_name")]
    public string? FirstName { get; set; }

    [Column("last_name")]
    public string? LastName { get; set; }

    [Column("enabled")]
    public bool Enabled { get; set; }

    [Column("created_timestamp")]
    public long CreatedTimestamp { get; set; }

    public virtual ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
    public virtual ICollection<Project> LabeledProjects { get; set; } = new List<Project>();
    public virtual ICollection<SubjectVideoGroupAssignment> OwnedAssignments { get; set; } = new List<SubjectVideoGroupAssignment>();
    public virtual ICollection<SubjectVideoGroupAssignment> LabeledAssignments { get; set; } = new List<SubjectVideoGroupAssignment>();

    [NotMapped]
    public DateTime RegistrationDate => DateTimeOffset.FromUnixTimeMilliseconds(CreatedTimestamp).DateTime;
}
