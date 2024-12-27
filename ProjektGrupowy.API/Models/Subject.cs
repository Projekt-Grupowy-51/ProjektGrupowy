using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.API.Models;

[Table("Subjects")]
public class Subject
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "wymagane")]
    [StringLength(255, ErrorMessage = "max 255 znakow")]
    public string Name { get; set; }

    public string Description { get; set; }

    [Required]
    public virtual Project Project { get; set; }

    public virtual ICollection<Label>? Labels { get; set; }

    public virtual ICollection<SubjectVideoGroupAssignment>? SubjectVideoGroupAssignments { get; set; }
}
