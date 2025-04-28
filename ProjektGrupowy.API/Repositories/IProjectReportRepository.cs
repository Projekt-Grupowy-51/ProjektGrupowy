using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories;

public interface IProjectReportRepository
{
    Task<Optional<IEnumerable<GeneratedReport>>> GetReportsAsync(int projectId);
    Task<Optional<GeneratedReport>> GetReportAsync(int reportId);
    Task<Optional<GeneratedReport>> AddReportAsync(GeneratedReport report);
}