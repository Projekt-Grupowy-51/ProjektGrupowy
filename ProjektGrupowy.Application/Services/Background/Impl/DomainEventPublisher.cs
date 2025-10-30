using MediatR;
using Microsoft.Extensions.Logging;
using ProjektGrupowy.Application.Events;
using ProjektGrupowy.Application.Interfaces.Persistence;
using ProjektGrupowy.Application.Interfaces.UnitOfWork;

namespace ProjektGrupowy.Application.Services.Background.Impl;

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
        try
        {
            var unpublishedEvents = await _domainEventRepository.GetUnpublishedEventsAsync();

            if (!unpublishedEvents.Any())
            {
                return;
            }

            _logger.LogInformation("Publishing {Count} domain events", unpublishedEvents.Count);

            foreach (var domainEvent in unpublishedEvents)
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
                        domainEvent.Message,
                        domainEvent.UserId,
                        domainEvent.OccurredAt
                    );

                    await _mediator.Publish(notification);
                }

                domainEvent.MarkAsPublished();
                _domainEventRepository.Update(domainEvent);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully published {Count} domain events", unpublishedEvents.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while publishing domain events");
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
