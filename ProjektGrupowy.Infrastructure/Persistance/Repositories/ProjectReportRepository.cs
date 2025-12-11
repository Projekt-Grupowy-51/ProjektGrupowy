using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Infrastructure.Persistance.Repositories;

public class ProjectReportRepository(IReadWriteContext dbContext) : IProjectReportRepository
{
    public Task<List<GeneratedReport>> GetReportsAsync(int projectId, string userId, bool isAdmin)
    {
        return dbContext.Projects.FilteredProjects(userId, isAdmin)
                .Where(p => p.Id == projectId)
                .SelectMany(x => x.GeneratedReports)
                .ToListAsync();
    }

    public Task<GeneratedReport> GetReportAsync(int reportId, string userId, bool isAdmin)
    {
        return dbContext.GeneratedReports.FilteredGeneratedReports(userId, isAdmin).SingleOrDefaultAsync(r => r.Id == reportId);
    }

    public async Task AddReportAsync(GeneratedReport report)
    {
        _ = await dbContext.GeneratedReports.AddAsync(report);
    }

    public void DeleteReport(GeneratedReport report)
    {
        _ = dbContext.GeneratedReports.Remove(report);
    }
}