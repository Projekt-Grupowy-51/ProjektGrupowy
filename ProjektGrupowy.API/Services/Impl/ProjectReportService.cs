using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.SignalR;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl;

public class ProjectReportService(
    IProjectReportRepository projectReportRepository,
    IMessageService messageService) : IProjectReportService
{
    public async Task<Optional<IEnumerable<GeneratedReport>>> GetReportsAsync(int projectId) => 
        await projectReportRepository.GetReportsAsync(projectId);

    public async Task<Optional<GeneratedReport>> GetReportAsync(int reportId) =>
        await projectReportRepository.GetReportAsync(reportId);

    public async Task<Optional<GeneratedReport>> AddReportAsync(GeneratedReport report) =>
        await projectReportRepository.AddReportAsync(report);

    public async Task DeleteReportAsync(GeneratedReport report)
    {
        await projectReportRepository.DeleteReportAsync(report);
        await messageService.SendInfoAsync(
            report.Project.OwnerId, 
            $"Report was deleted.");
    }
}