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

    public async Task<List<DomainEvent>> GetUnpublishedEventsAsync()
    {
        // Use raw SQL with FOR UPDATE SKIP LOCKED to prevent race conditions
        // This ensures that concurrent calls will not process the same events
        // SKIP LOCKED means if row is locked by another transaction, skip it
        var events = await _context.DomainEvents
            .FromSqlRaw(@"
                SELECT * FROM domain_events
                WHERE is_published = false
                ORDER BY occurred_at
                LIMIT 100
                FOR UPDATE SKIP LOCKED
            ")
            .ToListAsync();

        return events;
    }

    public void Update(DomainEvent domainEvent)
    {
        _context.DomainEvents.Update(domainEvent);
    }
}
