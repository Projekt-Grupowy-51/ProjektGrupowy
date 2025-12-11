using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using ProjektGrupowy.Domain.Events;

namespace ProjektGrupowy.Domain.Models;

public abstract class BaseEntity
{
    [NotMapped]
    public List<DomainEvent> DomainEvents { get; private set; } = new();

    public void AddDomainEvent(string message, string userId)
    {
        var domainEvent = DomainEvent.Create(message, userId);
        DomainEvents.Add(domainEvent);
    }

    public void AddTypedDomainEvent(string message, string userId, string eventType, object eventData)
    {
        var eventDataJson = JsonSerializer.Serialize(eventData);
        var domainEvent = DomainEvent.CreateTyped(message, userId, eventType, eventDataJson);
        DomainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        DomainEvents.Clear();
    }
}
