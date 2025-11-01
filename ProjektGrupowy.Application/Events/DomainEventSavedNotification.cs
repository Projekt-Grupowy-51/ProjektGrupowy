using MediatR;
using ProjektGrupowy.Domain.Events;

namespace ProjektGrupowy.Application.Events;

/// <summary>
/// Notification published immediately after domain events are saved to the database.
/// This enables real-time event processing without polling.
/// </summary>
public class DomainEventSavedNotification : INotification
{
    public List<DomainEvent> Events { get; }

    public DomainEventSavedNotification(List<DomainEvent> events)
    {
        Events = events;
    }
}

