using System.ComponentModel.DataAnnotations;

namespace ProjektGrupowy.API.Models;

/// <summary>
/// Klasa reprezentująca wideo jako model.
/// </summary>
public class Video
{
    [Key] public int Id { get; set; }

    [Required] [StringLength(255)] public string Title { get; set; }

    [Required] [StringLength(255)] public string Description { get; set; }

    [Required] [StringLength(255)] public string Path { get; set; }

    public ICollection<Project> Projects { get; set; } = new List<Project>();

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