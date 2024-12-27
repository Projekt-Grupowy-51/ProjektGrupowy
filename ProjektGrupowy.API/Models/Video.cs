using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.API.Models;

[Table("Vidoes")]
public class Video
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Title { get; set; }

    [Required]
    [StringLength(255)]
    public string Description { get; set; }

    [Required]
    [StringLength(255)] 
    public string Path { get; set; }

    [Required]
    public virtual VideoGroup VideoGroup { get; set; }

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