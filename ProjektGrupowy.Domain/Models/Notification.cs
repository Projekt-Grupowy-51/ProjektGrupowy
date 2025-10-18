using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.Domain.Models;

[Table("Notifications")]
public class Notification
{
    [Key] public int Id { get; set; }
    public string RecipientId { get; set; }
    [ForeignKey(nameof(RecipientId))] public virtual User User { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string Type { get; set; }
    public string Message { get; set; }
}