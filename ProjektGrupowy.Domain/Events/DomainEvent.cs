using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.Domain.Events;

// [Table("DomainEvents")]
public class DomainEvent
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Message { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string UserId { get; set; } = string.Empty;

    [StringLength(255)]
    public string? EventType { get; set; }

    public string? EventData { get; set; }

    public DateTime OccurredAt { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishedAt { get; set; }

    public static DomainEvent Create(string message, string userId)
    {
        return new DomainEvent
        {
            Message = message,
            UserId = userId,
            OccurredAt = DateTime.UtcNow,
            IsPublished = false
        };
    }

    public static DomainEvent CreateTyped(string message, string userId, string eventType, string eventData)
    {
        return new DomainEvent
        {
            Message = message,
            UserId = userId,
            EventType = eventType,
            EventData = eventData,
            OccurredAt = DateTime.UtcNow,
            IsPublished = false
        };
    }

    public void MarkAsPublished()
    {
        IsPublished = true;
        PublishedAt = DateTime.UtcNow;
    }
}
