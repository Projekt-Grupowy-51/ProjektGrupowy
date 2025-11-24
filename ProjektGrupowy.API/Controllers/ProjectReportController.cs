using AutoMapper;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.ProjectReport;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Utils;
using ProjektGrupowy.Application.Features.ProjectReport.Commands.DeleteReport;
using ProjektGrupowy.Application.Features.ProjectReport.Commands.GenerateReport;
using ProjektGrupowy.Application.Features.ProjectReport.Queries.GetReport;
using ProjektGrupowy.Application.Features.ProjectReport.Queries.GetReports;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Domain.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

/// <summary>
/// Controller for managing project reports. Handles operations such as retrieving, generating, downloading, and deleting project reports.
/// </summary>
[Route("api/project-reports")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
[Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
public class ProjectReportController(
    IMediator mediator,
    ICurrentUserService currentUserService,
    IMapper mapper,
    IConfiguration configuration,
    IBackgroundJobClient backgroundJobClient) : ControllerBase
{
    /// <summary>
    /// Get all reports for a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>A collection of reports for the specified project.</returns>
    [HttpGet("projects/{projectId:int}")]
    public async Task<IActionResult> GetProjectReports(int projectId)
    {
        var query = new GetReportsQuery(projectId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<IEnumerable<GeneratedReportResponse>>(result.Value);
        return Ok(response);
    }

    /// <summary>
    /// Get a specific report by its ID.
    /// </summary>
    /// <param name="reportId">The ID of the report.</param>
    /// <returns>The report with the specified ID.</returns>
    [HttpGet("{reportId:int}")]
    public async Task<IActionResult> GetReport(int reportId)
    {
        var query = new GetReportQuery(reportId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var response = mapper.Map<GeneratedReportResponse>(result.Value);
        return Ok(response);
    }

    /// <summary>
    /// Download a specific report by its ID.
    /// </summary>
    /// <param name="reportId">The ID of the report.</param>
    /// <returns>The report file for download.</returns>
    [HttpGet("download/{reportId:int}")]
    public async Task<IActionResult> DownloadReport(int reportId)
    {
        var query = new GetReportQuery(reportId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result.Errors);
        }

        var report = result.Value;

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
        var jobId = backgroundJobClient.Enqueue<IMediator>(m =>
            m.Send(new GenerateReportCommand(projectId, currentUserService.UserId, currentUserService.IsAdmin), CancellationToken.None));

        if (jobId is null)
        {
            return BadRequest("Failed to enqueue report generation job.");
        }

        return Accepted(new { message = "Report generation job has been enqueued.", jobId });
    }

    /// <summary>
    /// Delete a specific report by its ID.
    /// </summary>
    /// <param name="reportId">The ID of the report to be deleted.</param>
    /// <returns>No content if successful, or NotFound if the report does not exist.</returns>
    [HttpDelete("{reportId:int}")]
    public async Task<IActionResult> DeleteReport(int reportId)
    {
        var command = new DeleteReportCommand(reportId, currentUserService.UserId, currentUserService.IsAdmin);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? NoContent()
            : NotFound(result.Errors);
    }
}
