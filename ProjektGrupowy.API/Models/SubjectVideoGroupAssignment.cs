using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.API.Models;

[Table("SubjectVideoGroupAssignments")]
public class SubjectVideoGroupAssignment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public virtual Subject Subject { get; set; }

    [Required]
    public virtual VideoGroup VideoGroup { get; set; }

    public virtual ICollection<Labeler>? Labelers { get; set; }

    public virtual ICollection<AssignedLabel>? AssignedLabels { get; set; }
}