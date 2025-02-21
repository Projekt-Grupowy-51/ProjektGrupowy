using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.API.Models;

[Table("Labelers")]
public class Labeler
{
    [Key] public int Id { get; set; }
    public string Name { get; set; }

    public virtual ICollection<AssignedLabel>? AssignedLabels { get; set; }

    public virtual ICollection<SubjectVideoGroupAssignment>? SubjectVideoGroups { get; set; }

    // Many-to-Many Relation
    public virtual ICollection<Project> ProjectLabelers { get; set; } = new List<Project>();
}