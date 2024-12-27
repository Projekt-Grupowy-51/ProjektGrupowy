using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.API.Models;

[Table("Scientists")]
public class Scientist
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "wymagane")]
    [StringLength(255, ErrorMessage = "max 255 znakow")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "wymagane")]
    [StringLength(255, ErrorMessage = "max 255 znakow")]
    public string LastName { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public virtual ICollection<Project>? Projects { get; set; } = new List<Project>();
}
