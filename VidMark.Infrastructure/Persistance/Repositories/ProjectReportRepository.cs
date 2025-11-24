using Microsoft.EntityFrameworkCore;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Domain.Models;

namespace VidMark.Infrastructure.Persistance.Repositories;

public class ProjectReportRepository(IReadWriteContext dbContext) : IProjectReportRepository
{
    public Task<List<GeneratedReport>> GetReportsAsync(int projectId, string userId, bool isAdmin)
    {
        return dbContext.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.ProjectLabelers)
                .Include(p => p.GeneratedReports)
                .FilteredProjects(userId, isAdmin)
                .Where(p => p.Id == projectId)
                .SelectMany(x => x.GeneratedReports)
                .ToListAsync();
    }

    public Task<GeneratedReport> GetReportAsync(int reportId, string userId, bool isAdmin)
    {
        return dbContext.GeneratedReports
                .Include(r => r.CreatedBy)
                .FilteredGeneratedReports(userId, isAdmin)
                .SingleOrDefaultAsync(r => r.Id == reportId);
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