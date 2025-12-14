using MediatR;
using Microsoft.Extensions.Logging;
using VidMark.Application.Events;
using VidMark.Application.Interfaces.Persistence;
using VidMark.Application.Interfaces.UnitOfWork;

namespace VidMark.Application.Services.Background.Impl;

public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IDomainEventRepository _domainEventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventPublisher> _logger;

    public DomainEventPublisher(
        IDomainEventRepository domainEventRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<DomainEventPublisher> logger)
    {
        _domainEventRepository = domainEventRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task PublishPendingEventsAsync()
    {
        // Get unpublished events with row-level lock (FOR UPDATE SKIP LOCKED)
        // This prevents race conditions when multiple instances try to process the same events
        var unpublishedEvents = await _domainEventRepository.GetUnpublishedEventsAsync();

        if (!unpublishedEvents.Any())
        {
            return;
        }

        _logger.LogInformation("Publishing {Count} domain events", unpublishedEvents.Count);

        var publishedCount = 0;
        var failedCount = 0;

        foreach (var domainEvent in unpublishedEvents)
        {
            try
            {
                // Check if it's a typed event
                if (!string.IsNullOrEmpty(domainEvent.EventType))
                {
                    await PublishTypedEventAsync(domainEvent);
                }
                else
                {
                    // Legacy notification event
                    var notification = new DomainEventNotification(
                        domainEvent.MessageContent,
                        domainEvent.UserId,
                        domainEvent.OccurredAt
                    );

                    await _mediator.Publish(notification);
                }

                // Mark as published and save immediately after successful publication
                // This ensures that if next event fails, we won't republish this one
                domainEvent.MarkAsPublished();
                _domainEventRepository.Update(domainEvent);
                await _unitOfWork.SaveChangesAsync();

                publishedCount++;
            }
            catch (Exception ex)
            {
                failedCount++;
                _logger.LogError(ex,
                    "Error occurred while publishing domain event {EventId}. Event type: {EventType}, MessageContent: {MessageContent}",
                    domainEvent.Id,
                    domainEvent.EventType ?? "notification",
                    domainEvent.MessageContent);
                // Continue processing other events - don't mark this one as published
            }
        }

        if (publishedCount > 0)
        {
            _logger.LogInformation("Successfully published {PublishedCount} domain events", publishedCount);
        }

        if (failedCount > 0)
        {
            _logger.LogWarning("Failed to publish {FailedCount} domain events. They will be retried on next processing cycle.", failedCount);
        }
    }

    private async Task PublishTypedEventAsync(Domain.Events.DomainEvent domainEvent)
    {
        // Map event type to MediatR notification
        switch (domainEvent.EventType)
        {
            case nameof(ReportGeneratedEvent):
                var reportEvent = System.Text.Json.JsonSerializer.Deserialize<ReportGeneratedEventData>(domainEvent.EventData!);
                if (reportEvent != null)
                {
                    await _mediator.Publish(new ReportGeneratedEvent(
                        reportEvent.ProjectId,
                        reportEvent.ReportId,
                        domainEvent.UserId
                    ));
                }
                break;

            default:
                _logger.LogWarning("Unknown event type: {EventType}", domainEvent.EventType);
                break;
        }
    }

    private record ReportGeneratedEventData(int ProjectId, int ReportId);
}
