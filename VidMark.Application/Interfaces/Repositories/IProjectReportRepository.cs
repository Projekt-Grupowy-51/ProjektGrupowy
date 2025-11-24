using VidMark.Domain.Models;

namespace VidMark.Application.Interfaces.Repositories;

public interface IProjectReportRepository
{
    Task<List<GeneratedReport>> GetReportsAsync(int projectId, string userId, bool isAdmin);
    Task<GeneratedReport> GetReportAsync(int reportId, string userId, bool isAdmin);
    Task AddReportAsync(GeneratedReport report);
    void DeleteReport(GeneratedReport report);
}