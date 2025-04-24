using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.API.Models;

public class GeneratedReport : IOwnedEntity
{
    [Key]
    public int Id { get; set; }
    
    public string Path { get; set; }
    public string OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public virtual User Owner { get; set; }
    
    public virtual Project Project { get; set; }
    public DateTime? DelDate { get; set; }
}