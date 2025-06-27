using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.DTOs.ProjectReport;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Application.Services.Background;
using ProjektGrupowy.Application.SignalR;
using ProjektGrupowy.Domain.Services;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Utils.Constants;
using ProjektGrupowy.Domain.Utils.Extensions;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
[Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
public class ProjectReportController(
    IProjectReportService projectReportService,
    IMessageService messageService,
    IMapper mapper,
    ICurrentUserService currentUserService,
    IConfiguration configuration) : ControllerBase
{
    [HttpGet("project/{projectId:int}")]
    public async Task<IActionResult> GetProjectReports(int projectId)
    {
        var result = await projectReportService.GetReportsAsync(projectId);
        return result.IsSuccess
            ? Ok(mapper.Map<IEnumerable<GeneratedReportResponse>>(result.GetValueOrThrow()))
            : NotFound(result.GetValueOrThrow());
    }

    [HttpGet("{reportId:int}")]
    public async Task<IActionResult> GetReport(int reportId)
    {
        var result = await projectReportService.GetReportAsync(reportId);
        return result.IsSuccess
            ? Ok(mapper.Map<GeneratedReportResponse>(result.GetValueOrThrow()))
            : NotFound(result.GetValueOrThrow());
    }

    [HttpGet("download/{reportId:int}")]
    public async Task<IActionResult> DownloadReport(int reportId)
    {
        var result = await projectReportService.GetReportAsync(reportId);
        if (result.IsFailure)
            return NotFound(result.GetErrorOrThrow());

        var report = result.GetValueOrThrow();

        if (DockerDetector.IsRunningInDocker())
        {
            var baseUrl = configuration["Reports:NginxUrl"];
            var path = $"{baseUrl}/{Path.GetFileName(report.Path)}";
            return Redirect(path);
        }
        else
        {
            return File(report.ToStream(), "application/json", Path.GetFileName(report.Path), true);
        }
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("{projectId:int}/generate-report")]
    public async Task<IActionResult> GenerateReport(int projectId)
    {
        var result = BackgroundJob.Enqueue<IReportGenerator>(g => g.GenerateAsync(projectId, currentUserService.UserId, currentUserService.IsAdmin));

        if (result is null)
        {
            return BadRequest("Failed to enqueue report generation job.");
        }
        else
        {
            await messageService.SendInfoAsync(
                User.GetUserId(),
                "Report generation job has been enqueued.");
            return Accepted("Report generation job has been enqueued.");
        }
    }

    [HttpDelete("{reportId:int}")]
    public async Task<IActionResult> DeleteReport(int reportId)
    {
        var searchResult = await projectReportService.GetReportAsync(reportId);
        if (searchResult.IsFailure)
            return NotFound();

        await projectReportService.DeleteReportAsync(reportId);
        return NoContent();
    }
}