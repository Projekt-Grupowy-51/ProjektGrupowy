using MediatR;
using Microsoft.Extensions.Logging;
using ProjektGrupowy.Application.Interfaces.SignalR;

namespace ProjektGrupowy.Application.Events;

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
            "Domain Event: {Message} | User: {UserId} | Occurred: {OccurredAt}",
            notification.Message,
            notification.UserId,
            notification.OccurredAt
        );

        // Wysyłanie powiadomienia przez SignalR
        await _messageService.SendInfoAsync(notification.UserId, notification.Message, cancellationToken);
    }
}
