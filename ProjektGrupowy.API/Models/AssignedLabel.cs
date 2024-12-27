using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.API.Models;

[Table("AssignedLabels")]
public class AssignedLabel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public virtual Label Label { get; set; }

    [Required]
    public virtual SubjectVideoGroupAssignment SubjectVideoGroupAssignment { get; set; }

    [Required]
    public virtual Labeler Labeler { get; set; }
}