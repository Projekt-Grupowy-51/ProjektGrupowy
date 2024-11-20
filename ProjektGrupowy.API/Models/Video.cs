using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.API.Models;

/// <summary>
/// Klasa reprezentująca wideo jako model.
/// </summary>

[Table("Wideo")]
public class Video
{
    [Key] public int Id { get; set; }

    [Required] [StringLength(255)] public string Title { get; set; }

    [Required] [StringLength(255)] public string Description { get; set; }

    [Required] [StringLength(255)] public string Path { get; set; }

    
    // Navigation property to one project
    public Project Project { get; set; }
    public int ProjectId { get; set; }

    public Stream ToStream()
    {
        try
        {
            return new FileStream(Path, FileMode.Open);
        }
        catch (Exception e)
        {
            return new MemoryStream();
        }
    }
}