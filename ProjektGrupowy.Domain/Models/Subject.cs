using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.Domain.Models;

[Table("Subjects")]
public class Subject : IOwnedEntity
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "wymagane")]
    [StringLength(255, ErrorMessage = "max 255 znakow")]
    public string Name { get; set; }

    public string Description { get; set; }

    [Required]
    public virtual Project Project { get; set; }

    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();

    public virtual ICollection<SubjectVideoGroupAssignment> SubjectVideoGroupAssignments { get; set; } = new List<SubjectVideoGroupAssignment>();
    public string CreatedById { get; set; }

    [ForeignKey(nameof(CreatedById))]
    public virtual User CreatedBy { get; set; }
    public DateTime? DelDate { get; set; } = null;
}
