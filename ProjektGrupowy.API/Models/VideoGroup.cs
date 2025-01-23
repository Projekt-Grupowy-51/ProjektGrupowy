using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.API.Models;

[Table("VideoGroups")]
public class VideoGroup
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }

    public virtual ICollection<Video>? Videos { get; set; }

    [Required]
    public virtual Project Project { get; set; }

    public virtual ICollection<SubjectVideoGroupAssignment>? SubjectVideoGroupAssignments { get; set; }
}