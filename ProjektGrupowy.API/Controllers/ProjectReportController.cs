using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.DTOs.ProjectReport;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Application.Services.Background;
using ProjektGrupowy.Application.SignalR;
using ProjektGrupowy.Application.Utils;
using ProjektGrupowy.Application.Utils.Extensions;
using ProjektGrupowy.Domain.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

/// <summary>
/// Controller for managing project reports. Handles operations such as retrieving, generating, downloading, and deleting project reports.
/// </summary>
/// <param name="projectReportService"></param>
/// <param name="backgroundJobClient"></param>
/// <param name="messageService"></param>
/// <param name="mapper"></param>
/// <param name="currentUserService"></param>
/// <param name="configuration"></param>
[Route("api/project-reports")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
[Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
public class ProjectReportController(
    IProjectReportService projectReportService,
    IBackgroundJobClient backgroundJobClient,
    IMessageService messageService,
    IMapper mapper,
    ICurrentUserService currentUserService,
    IConfiguration configuration) : ControllerBase
{
    /// <summary>
    /// Get all reports for a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>A collection of reports for the specified project.</returns>
    [HttpGet("projects/{projectId:int}")]
    public async Task<IActionResult> GetProjectReports(int projectId)
    {
        var result = await projectReportService.GetReportsAsync(projectId);
        return result.IsSuccess
            ? Ok(mapper.Map<IEnumerable<GeneratedReportResponse>>(result.GetValueOrThrow()))
            : NotFound(result.GetValueOrThrow());
    }

    /// <summary>
    /// Get a specific report by its ID.
    /// </summary>
    /// <param name="reportId">The ID of the report.</param>
    /// <returns>The report with the specified ID.</returns>
    [HttpGet("{reportId:int}")]
    public async Task<IActionResult> GetReport(int reportId)
    {
        var result = await projectReportService.GetReportAsync(reportId);
        return result.IsSuccess
            ? Ok(mapper.Map<GeneratedReportResponse>(result.GetValueOrThrow()))
            : NotFound(result.GetValueOrThrow());
    }

    /// <summary>
    /// Download a specific report by its ID.
    /// </summary>
    /// <param name="reportId">The ID of the report.</param>
    /// <returns>The report file for download.</returns>
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

        return File(report.ToStream(), "application/json", Path.GetFileName(report.Path), true);
    }

    /// <summary>
    /// Generate a new report for a specific project. This operation is enqueued as a background job.
    /// </summary>
    /// <param name="projectId">The ID of the project for which the report is to be generated.</param>
    /// <returns>An acknowledgment that the report generation job has been enqueued.</returns>
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpPost("{projectId:int}/generate-report")]
    public async Task<IActionResult> GenerateReport(int projectId)
    {
        var result = backgroundJobClient
            .Enqueue<IReportGenerator>(g => g.GenerateAsync(projectId, currentUserService.UserId, currentUserService.IsAdmin));

        if (result is null)
        {
            return BadRequest("Failed to enqueue report generation job.");
        }

        await messageService.SendInfoAsync(
            User.GetUserId(),
            "Report generation job has been enqueued.");
        return Accepted("Report generation job has been enqueued.");
    }

    /// <summary>
    /// Delete a specific report by its ID.
    /// </summary>
    /// <param name="reportId">The ID of the report to be deleted.</param>
    /// <returns>No content if successful, or NotFound if the report does not exist.</returns>
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