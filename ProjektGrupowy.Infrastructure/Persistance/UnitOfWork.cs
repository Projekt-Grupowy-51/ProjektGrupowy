using ProjektGrupowy.Application.Interfaces.UnitOfWork;

namespace ProjektGrupowy.Infrastructure.Persistance;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return context.SaveChangesAsync(ct);
    }
}
