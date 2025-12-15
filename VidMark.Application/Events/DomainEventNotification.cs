using MediatR;
using VidMark.Domain.Models;

namespace VidMark.Application.Events;

public class DomainEventNotification : INotification
{
    public MessageContent Content { get; }
    public string UserId { get; }
    public DateTime OccurredAt { get; }

    public DomainEventNotification(MessageContent content, string userId, DateTime occurredAt)
    {
        Content = content;
        UserId = userId;
        OccurredAt = occurredAt;
    }
}
