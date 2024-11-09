using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.API.Models;

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

    public ICollection<Video> Videos { get; set; } = new List<Video>();
}
