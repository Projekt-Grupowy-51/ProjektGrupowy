using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

public interface IProjectReportRepository
{
    Task<Optional<IEnumerable<GeneratedReport>>> GetReportsAsync(int projectId, string userId, bool isAdmin);
    Task<Optional<GeneratedReport>> GetReportAsync(int reportId, string userId, bool isAdmin);
    Task<Optional<GeneratedReport>> AddReportAsync(GeneratedReport report);
    Task DeleteReportAsync(GeneratedReport report);
}