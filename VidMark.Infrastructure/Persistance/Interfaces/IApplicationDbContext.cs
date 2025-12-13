namespace VidMark.Infrastructure.Persistance.Interfaces;

public interface IApplicationDbContext : IReadWriteContext
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
