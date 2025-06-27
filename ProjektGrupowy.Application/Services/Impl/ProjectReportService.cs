using Microsoft.AspNetCore.Authorization;
using ProjektGrupowy.Application.Authorization;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Infrastructure.Repositories;
using ProjektGrupowy.Application.SignalR;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Services;

namespace ProjektGrupowy.Application.Services.Impl;

public class ProjectReportService(
    IProjectReportRepository projectReportRepository,
    IMessageService messageService,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService) : IProjectReportService
{
    public async Task<Optional<IEnumerable<GeneratedReport>>> GetReportsAsync(int projectId)
    {
        var reportsOpt = await projectReportRepository.GetReportsAsync(projectId);
        if (reportsOpt.IsFailure)
        {
            return reportsOpt;
        }

        var reports = reportsOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, reports, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException("You do not have permission to access these reports.");
        }

        return reportsOpt;
    }

    public async Task<Optional<GeneratedReport>> GetReportAsync(int reportId)
    {
        var reportOpt = await projectReportRepository.GetReportAsync(reportId);
        if (reportOpt.IsFailure)
        {
            return reportOpt;
        }
        var report = reportOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, report, new ResourceOperationRequirement(ResourceOperation.Read));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException("You do not have permission to access this report.");
        }
        return reportOpt;
    }

    public async Task DeleteReportAsync(int reportId)
    {
        var reportOpt = await projectReportRepository.GetReportAsync(reportId);
        if (reportOpt.IsFailure)
        {
            await messageService.SendInfoAsync(currentUserService.UserId, "Error while deleting report.");
        }

        var reportEntity = reportOpt.GetValueOrThrow();
        var authResult = await authorizationService.AuthorizeAsync(currentUserService.User, reportEntity, new ResourceOperationRequirement(ResourceOperation.Delete));
        if (!authResult.Succeeded)
        {
            throw new ForbiddenException("You do not have permission to delete this report.");
        }

        await projectReportRepository.DeleteReportAsync(reportEntity);
        await messageService.SendInfoAsync(
            currentUserService.UserId, 
            $"Report was deleted.");
    }
}