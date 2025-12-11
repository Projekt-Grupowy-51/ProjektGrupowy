using MediatR;

namespace ProjektGrupowy.Application.Events;

public class DomainEventNotification : INotification
{
    public string Message { get; }
    public string UserId { get; }
    public DateTime OccurredAt { get; }

    public DomainEventNotification(string message, string userId, DateTime occurredAt)
    {
        Message = message;
        UserId = userId;
        OccurredAt = occurredAt;
    }
}
