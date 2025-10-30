using ProjektGrupowy.Domain.Events;

namespace ProjektGrupowy.Application.Interfaces.Persistence;

public interface IDomainEventRepository
{
    Task<List<DomainEvent>> GetUnpublishedEventsAsync();
    void Update(DomainEvent domainEvent);
}
