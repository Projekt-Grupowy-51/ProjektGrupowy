using System.Text.Json;
using VidMark.Domain.Events;

namespace VidMark.Domain.Models;

public abstract class BaseEntity
{
    public List<DomainEvent> DomainEvents { get; private set; } = new();

    public void AddDomainEvent(MessageContent content, string userId)
    {
        var domainEvent = DomainEvent.Create(content, userId);
        DomainEvents.Add(domainEvent);
    }

    public void AddTypedDomainEvent(MessageContent content, string userId, string eventType, object eventData)
    {
        var eventDataJson = JsonSerializer.Serialize(eventData);
        var domainEvent = DomainEvent.CreateTyped(content, userId, eventType, eventDataJson);
        DomainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        DomainEvents.Clear();
    }
}
