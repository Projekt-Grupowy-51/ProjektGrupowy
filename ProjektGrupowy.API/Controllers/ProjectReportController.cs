using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.ProjectReport;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils.Constants;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[ServiceFilter(typeof(NonSuccessGetFilter))]
[Authorize]
public class ProjectReportController(IProjectReportService projectReportService, IMapper mapper) : ControllerBase
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
}