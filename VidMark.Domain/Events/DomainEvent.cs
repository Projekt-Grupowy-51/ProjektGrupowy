using VidMark.Domain.Models;

namespace VidMark.Domain.Events;

public class DomainEvent
{
    public int Id { get; set; }

    public MessageContent MessageContent { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string? EventType { get; set; }

    public string? EventData { get; set; }

    public DateTime OccurredAt { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishedAt { get; set; }

    public static DomainEvent Create(MessageContent content, string userId)
    {
        return new DomainEvent
        {
            MessageContent = content,
            UserId = userId,
            OccurredAt = DateTime.UtcNow,
            IsPublished = false
        };
    }

    public static DomainEvent CreateTyped(MessageContent content, string userId, string eventType, string eventData)
    {
        return new DomainEvent
        {
            MessageContent = content,
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
