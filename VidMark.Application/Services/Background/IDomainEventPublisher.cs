namespace VidMark.Application.Services.Background;

public interface IDomainEventPublisher
{
    Task PublishPendingEventsAsync();
}
