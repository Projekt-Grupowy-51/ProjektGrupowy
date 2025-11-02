using MediatR;
using Microsoft.Extensions.Logging;
using ProjektGrupowy.Application.Services.Background;

namespace ProjektGrupowy.Application.Events;

/// <summary>
/// Handler that processes unpublished domain events from the outbox when ProcessOutboxNotification is received.
/// This handler is invoked by the OutboxProcessingBehavior after each command/query execution.
/// </summary>
public class ProcessOutboxNotificationHandler : INotificationHandler<ProcessOutboxNotification>
{
    private readonly IDomainEventPublisher _domainEventPublisher;
    private readonly ILogger<ProcessOutboxNotificationHandler> _logger;

    public ProcessOutboxNotificationHandler(
        IDomainEventPublisher domainEventPublisher,
        ILogger<ProcessOutboxNotificationHandler> logger)
    {
        _domainEventPublisher = domainEventPublisher;
        _logger = logger;
    }

    public async Task Handle(ProcessOutboxNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            await _domainEventPublisher.PublishPendingEventsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing outbox");
            // Don't rethrow - we don't want to fail the original request
        }
    }
}
