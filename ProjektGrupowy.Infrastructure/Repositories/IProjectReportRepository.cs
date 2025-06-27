using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

public interface IProjectReportRepository
{
    Task<Optional<IEnumerable<GeneratedReport>>> GetReportsAsync(int projectId);
    Task<Optional<GeneratedReport>> GetReportAsync(int reportId);
    Task<Optional<GeneratedReport>> AddReportAsync(GeneratedReport report);
    Task DeleteReportAsync(GeneratedReport report);
}