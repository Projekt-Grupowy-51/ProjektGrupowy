using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.Application.Interfaces.Persistence;
using ProjektGrupowy.Domain.Events;

namespace ProjektGrupowy.Infrastructure.Persistance.Repositories;

public class DomainEventRepository : IDomainEventRepository
{
    private readonly AppDbContext _context;

    public DomainEventRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<DomainEvent>> GetUnpublishedEventsAsync()
    {
        return _context.DomainEvents
            .Where(de => !de.IsPublished)
            .OrderBy(de => de.OccurredAt)
            .ToListAsync();
    }

    public void Update(DomainEvent domainEvent)
    {
        _context.DomainEvents.Update(domainEvent);
    }
}
