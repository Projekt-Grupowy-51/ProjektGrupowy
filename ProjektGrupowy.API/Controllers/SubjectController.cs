using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Subject;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Services;
using ProjektGrupowy.API.Utils.Enums;
using System.Security.Claims;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
[Authorize]
public class SubjectController(ISubjectService subjectService, IScientistService scientistService, ILabelerService labelerService, IProjectService projectService, IMapper mapper) : ControllerBase
{
    private (ActionResult? Error, bool IsScientist, bool IsAdmin, bool IsLabeler, Scientist? Scientist, Labeler? Labeler) CheckGeneralAccess()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return (Unauthorized("User identity not found."), false, false, false, null, null);
        }

        bool isScientist = User.IsInRole(RoleEnum.Scientist.ToString());
        bool isAdmin = User.IsInRole(RoleEnum.Admin.ToString());
        bool isLabeler = User.IsInRole(RoleEnum.Labeler.ToString());

        if (!isScientist && !isAdmin && !isLabeler)
        {
            return (Forbid(), false, false, false, null, null);
        }

        if (isScientist)
        {
            return (null, true, false, false, scientistService.GetScientistByUserIdAsync(userId).Result.GetValueOrThrow(), null);
        }

        if (isLabeler)
        {
            return (null, false, false, true, null, labelerService.GetLabelerByUserIdAsync(userId).Result.GetValueOrThrow());
        }

        return (null, false, true, false, null, null);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetSubjectsAsync()
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsLabeler)
        {
            return Forbid();
        }
    
        var subjects = checkResult.IsAdmin switch {
            true => await subjectService.GetSubjectsAsync(),
            false => await subjectService.GetSubjectsByScientistId(checkResult.Scientist!.Id)
        };

        return subjects.IsSuccess
            ? Ok(mapper.Map<IEnumerable<SubjectResponse>>(subjects.GetValueOrThrow()))
            : NotFound(subjects.GetErrorOrThrow());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectResponse>> GetSubjectAsync(int id)
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsLabeler)
        {
            return Forbid();
        }

        var subjectResult = await subjectService.GetSubjectAsync(id);

        if (subjectResult.IsFailure)
        {
            return NotFound(new { Message = "Subject not found" });
        }

        var subject = subjectResult.GetValueOrThrow();

        if (checkResult.IsScientist)
        {
            if (subject.Project.Scientist.Id != checkResult.Scientist!.Id)
            {
                return Forbid("Not your project");
            }
        }

        return Ok(mapper.Map<SubjectResponse>(subject));
    }

    [HttpPost]
    public async Task<ActionResult<SubjectResponse>> AddSubjectAsync(SubjectRequest subjectRequest)
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsLabeler)
        {
            return Forbid();
        }

        var projectResult = await projectService.GetProjectAsync(subjectRequest.ProjectId);

        if (projectResult.IsFailure)
        {
            return BadRequest(projectResult.GetErrorOrThrow());
        }

        var project = projectResult.GetValueOrThrow();

        if (checkResult.IsScientist)
        {
            if (project.Scientist.Id != checkResult.Scientist!.Id)
            {
                return Forbid("Not your project");
            }
        }

        var result = await subjectService.AddSubjectAsync(subjectRequest);

        if (result.IsFailure)
            return BadRequest(result.GetErrorOrThrow());

        var createdSubject = result.GetValueOrThrow();

        var subjectResponse = mapper.Map<SubjectResponse>(createdSubject);

        return CreatedAtAction("GetSubject", new { id = createdSubject.Id }, subjectResponse);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutSubjectAsync(int id, SubjectRequest subjectRequest)
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsLabeler)
        {
            return Forbid();
        }

        var subjectResult = await subjectService.GetSubjectAsync(id);

        if (subjectResult.IsFailure)
        {
            return NotFound(new { Message = "Subject not found" });
        }

        var subject = subjectResult.GetValueOrThrow();

        if (checkResult.IsScientist)
        {
            var projectsResult = await projectService.GetProjectsOfScientist(checkResult.Scientist!.Id);

            if (projectsResult.IsFailure)
            {
                return BadRequest(projectsResult.GetErrorOrThrow());
            }

            if (!projectsResult.GetValueOrThrow().Any(x => x.Id == subjectRequest.ProjectId))
            {
                return Forbid("Not your project");
            }
        }

        var result = await subjectService.UpdateSubjectAsync(id, subjectRequest);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSubjectAsync(int id)
    {
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsLabeler)
        {
            return Forbid();
        }

        var subjectResult = await subjectService.GetSubjectAsync(id);

        if (subjectResult.IsFailure)
        {
            return NotFound(new { Message = "Subject not found" });
        }

        var subject = subjectResult.GetValueOrThrow();

        if (checkResult.IsScientist)
        {
            if (subject.Project.Scientist.Id != checkResult.Scientist!.Id)
            {
                return Forbid("Not your project");
            }
        }

        await subjectService.DeleteSubjectAsync(id);

        return NoContent();
    }

    [HttpGet("{id:int}/label")]
    public async Task<ActionResult> GetSubjectLabelsAsync(int id)
    {
            /*
        var checkResult = CheckGeneralAccess();
        if (checkResult.Error != null)
        {
            return checkResult.Error;
        }

        if (checkResult.IsScientist)
        {
            return Forbid();
        }

        var labels = await subjectService.GetSubjectLabelsAsync(id);

        return labels.IsSuccess
            ? Ok(mapper.Map<IEnumerable<LabelResponse>>(labels.GetValueOrThrow()))
            : NotFound(labels.GetErrorOrThrow());*/

            return NoContent();
    }
}