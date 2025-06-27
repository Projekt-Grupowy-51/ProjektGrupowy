using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.Domain.Models;

[Table("VideoGroups")]
public class VideoGroup : IOwnedEntity
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();

    [Required]
    public virtual Project Project { get; set; }

    public virtual ICollection<SubjectVideoGroupAssignment> SubjectVideoGroupAssignments { get; set; } = new List<SubjectVideoGroupAssignment>();
    public string CreatedById { get; set; }

    [ForeignKey(nameof(CreatedById))]
    public virtual User CreatedBy { get; set; }
    public DateTime? DelDate { get; set; } = null;
}