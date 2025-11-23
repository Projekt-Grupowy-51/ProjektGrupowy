namespace ProjektGrupowy.Domain.Events;

public class DomainEvent
{
    public int Id { get; set; }

    public string Message { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

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
