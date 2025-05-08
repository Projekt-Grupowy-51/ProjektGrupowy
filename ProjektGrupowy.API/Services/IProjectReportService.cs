using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface IProjectReportService
{
    Task<Optional<IEnumerable<GeneratedReport>>> GetReportsAsync(int projectId);
    Task<Optional<GeneratedReport>> GetReportAsync(int reportId);
    Task DeleteReportAsync(int reportId);
}