namespace ProjektGrupowy.Application.Services.Background;

public interface IDomainEventPublisher
{
    Task PublishPendingEventsAsync();
}
