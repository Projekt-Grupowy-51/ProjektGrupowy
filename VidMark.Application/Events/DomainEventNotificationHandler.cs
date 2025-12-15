using MediatR;
using Microsoft.Extensions.Logging;
using VidMark.Application.Interfaces.SignalR;

namespace VidMark.Application.Events;

public class DomainEventNotificationHandler : INotificationHandler<DomainEventNotification>
{
    private readonly ILogger<DomainEventNotificationHandler> _logger;
    private readonly IMessageService _messageService;

    public DomainEventNotificationHandler(
        ILogger<DomainEventNotificationHandler> logger,
        IMessageService messageService)
    {
        _logger = logger;
        _messageService = messageService;
    }

    public async Task Handle(DomainEventNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Domain Event: {Content} | User: {UserId} | Occurred: {OccurredAt}",
            notification.Content,
            notification.UserId,
            notification.OccurredAt
        );

        // Wysyłanie powiadomienia przez SignalR
        await _messageService.SendInfoAsync(notification.UserId, notification.Content, cancellationToken);
    }
}
