using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.ProjectReport;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils;
using ProjektGrupowy.API.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class ProjectReportController(
    IProjectReportService projectReportService,
    IMapper mapper,
    IConfiguration configuration) : ControllerBase
{
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("project/{projectId:int}")]
    public async Task<IActionResult> GetProjectReports(int projectId)
    {
        var result = await projectReportService.GetReportsAsync(projectId);
        return result.IsSuccess 
            ? Ok(mapper.Map<IEnumerable<GeneratedReportResponse>>(result.GetValueOrThrow())) 
            : NotFound(result.GetValueOrThrow());
    }
    
    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
    [HttpGet("{reportId:int}")]
    public async Task<IActionResult> GetReport(int reportId)
    {
        var result = await projectReportService.GetReportAsync(reportId);
        return result.IsSuccess
            ? Ok(mapper.Map<GeneratedReportResponse>(result.GetValueOrThrow()))
            : NotFound(result.GetValueOrThrow());
    }

    [Authorize(Policy = PolicyConstants.RequireAdminOrScientist)]
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
}