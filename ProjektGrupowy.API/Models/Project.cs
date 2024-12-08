using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.API.Models;

[Table("Projects")]
public class Project
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Podanie nazwy projektu jest wymagane.")]
    [StringLength(255, ErrorMessage = "Maksymalna długość nazwy projektu wynosi 255 znaków.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Podanie opisu projektu jest wymagane.")]
    [StringLength(1000, ErrorMessage = "Maksymalna długość opisu projektu wynosi 1000 znaków.")]
    public string Description { get; set; }

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();

    public virtual Scientist Scientist { get; set; }

    public int ScientistId { get; set; }
}
