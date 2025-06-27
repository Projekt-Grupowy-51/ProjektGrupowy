using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Services;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Infrastructure.Data;

namespace ProjektGrupowy.Infrastructure.Repositories.Impl;

public class ProjectReportRepository(AppDbContext dbContext, ICurrentUserService currentUserService) : IProjectReportRepository
{
    public async Task<Optional<IEnumerable<GeneratedReport>>> GetReportsAsync(int projectId)
    {
        try
        {
            var project = await dbContext.Projects.FilteredProjects(currentUserService.UserId, currentUserService.IsAdmin)
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
            var report = await dbContext.GeneratedReports.FilteredGeneratedReports(currentUserService.UserId, currentUserService.IsAdmin).SingleOrDefaultAsync(r => r.Id == reportId);
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

    public async Task DeleteReportAsync(GeneratedReport report)
    {
        try
        {
            dbContext.GeneratedReports.Remove(report);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to delete report: {e.Message}");
        }
    }
}