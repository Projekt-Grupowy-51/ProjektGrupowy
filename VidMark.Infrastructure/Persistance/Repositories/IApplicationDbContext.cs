namespace VidMark.Infrastructure.Persistance.Repositories;

public interface IApplicationDbContext : IReadWriteContext
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
