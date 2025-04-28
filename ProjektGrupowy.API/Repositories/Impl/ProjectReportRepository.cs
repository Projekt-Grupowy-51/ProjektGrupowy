using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.API.Data;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories.Impl;

public class ProjectReportRepository(AppDbContext dbContext) : IProjectReportRepository
{
    public async Task<Optional<IEnumerable<GeneratedReport>>> GetReportsAsync(int projectId)
    {
        try
        {
            var project = await dbContext.Projects
                .Include(p => p.GeneratedReports)
                .SingleOrDefaultAsync(p => p.Id == projectId);

            return project is null 
                ? Optional<IEnumerable<GeneratedReport>>.Failure($"The project with id {projectId} was not found.") 
                : Optional<IEnumerable<GeneratedReport>>.Success(project.GeneratedReports);
        }
        catch (Exception e)
        {
            return Optional<IEnumerable<GeneratedReport>>.Failure(e.Message);
        }
    }

    public async Task<Optional<GeneratedReport>> GetReportAsync(int reportId)
    {
        try
        {
            var report = await dbContext.GeneratedReports.SingleOrDefaultAsync(r => r.Id == reportId);
            return report is null
                ? Optional<GeneratedReport>.Failure($"Report with id {reportId} was not found")
                : Optional<GeneratedReport>.Success(report);
        }
        catch (Exception e)
        {
            return Optional<GeneratedReport>.Failure(e.Message);
        }
    }

    public async Task<Optional<GeneratedReport>> AddReportAsync(GeneratedReport report)
    {
        try 
        {
            await dbContext.GeneratedReports.AddAsync(report);
            await dbContext.SaveChangesAsync();
            return Optional<GeneratedReport>.Success(report);
        }
        catch (Exception e)
        {
            return Optional<GeneratedReport>.Failure(e.Message);
        }
    }
}