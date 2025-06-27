using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Application.Services;

public interface IProjectReportService
{
    Task<Optional<IEnumerable<GeneratedReport>>> GetReportsAsync(int projectId);
    Task<Optional<GeneratedReport>> GetReportAsync(int reportId);
    Task DeleteReportAsync(int reportId);
}