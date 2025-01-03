using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.API.Models;

[Table("Labels")]
public class Label
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    [Required]
    public virtual Subject Subject { get; set; }

    public virtual ICollection<AssignedLabel>? AssignedLabels { get; set; }
}