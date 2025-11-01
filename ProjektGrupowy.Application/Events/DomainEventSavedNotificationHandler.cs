using MediatR;
using Microsoft.Extensions.Logging;
using ProjektGrupowy.Application.Interfaces.Persistence;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;

namespace ProjektGrupowy.Application.Events;

/// <summary>
/// Handler that processes domain events immediately after they're saved.
/// This replaces the Hangfire polling-based approach with real-time event processing.
/// </summary>
public class DomainEventSavedNotificationHandler(
    IMediator mediator,
    IDomainEventRepository domainEventRepository,
    IUnitOfWork unitOfWork,
    ILogger<DomainEventSavedNotificationHandler> logger)
    : INotificationHandler<DomainEventSavedNotification>
{
    public async Task Handle(DomainEventSavedNotification notification, CancellationToken cancellationToken)
    {
        if (!notification.Events.Any())
        {
            return;
        }

        try
        {
            logger.LogInformation("Processing {Count} domain events in real-time", notification.Events.Count);

            foreach (var domainEvent in notification.Events)
            {
                // Check if it's a typed event
                if (!string.IsNullOrEmpty(domainEvent.EventType))
                {
                    await PublishTypedEventAsync(domainEvent, cancellationToken);
                }
                else
                {
                    // Legacy notification event
                    var domainEventNotification = new DomainEventNotification(
                        domainEvent.Message,
                        domainEvent.UserId,
                        domainEvent.OccurredAt
                    );

                    await mediator.Publish(domainEventNotification, cancellationToken);
                }

                // Mark as published
                domainEvent.MarkAsPublished();
                domainEventRepository.Update(domainEvent);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully processed {Count} domain events", notification.Events.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing domain events in real-time");
            // Events remain unpublished and can be picked up by a fallback mechanism if needed
        }
    }

    private async Task PublishTypedEventAsync(Domain.Events.DomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // Map event type to MediatR notification
        switch (domainEvent.EventType)
        {
            case nameof(ReportGeneratedEvent):
                var reportEvent = System.Text.Json.JsonSerializer.Deserialize<ReportGeneratedEventData>(domainEvent.EventData!);
                if (reportEvent != null)
                {
                    await mediator.Publish(new ReportGeneratedEvent(
                        reportEvent.ProjectId,
                        reportEvent.ReportId,
                        domainEvent.UserId
                    ), cancellationToken);
                }
                break;

            default:
                logger.LogWarning("Unknown event type: {EventType}", domainEvent.EventType);
                break;
        }
    }

    private record ReportGeneratedEventData(int ProjectId, int ReportId);
}

