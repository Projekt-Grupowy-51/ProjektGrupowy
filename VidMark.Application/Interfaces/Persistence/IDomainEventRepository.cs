using VidMark.Domain.Events;

namespace VidMark.Application.Interfaces.Persistence;

public interface IDomainEventRepository
{
    Task<List<DomainEvent>> GetUnpublishedEventsAsync();
    void Update(DomainEvent domainEvent);
}
