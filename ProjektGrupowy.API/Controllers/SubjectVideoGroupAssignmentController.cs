using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.SubjectVideoGroupAssignment;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;

namespace ProjektGrupowy.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(ValidateModelStateFilter))]
public class SubjectVideoGroupAssignmentController(ISubjectVideoGroupAssignmentService subjectVideoGroupAssignmentService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectVideoGroupAssignmentResponse>>> GetSubjectVideoGroupAssignmentsAsync()
    {
        var subjectVideoGroupAssignments = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentsAsync();
        return subjectVideoGroupAssignments.IsSuccess
            ? Ok(mapper.Map<IEnumerable<SubjectVideoGroupAssignmentResponse>>(subjectVideoGroupAssignments.GetValueOrThrow()))
            : NotFound(subjectVideoGroupAssignments.GetErrorOrThrow());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectVideoGroupAssignmentResponse>> GetSubjectVideoGroupAssignmentAsync(int id)
    {
        var subjectVideoGroupAssignment = await subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentAsync(id);
        return subjectVideoGroupAssignment.IsSuccess
            ? Ok(mapper.Map<SubjectVideoGroupAssignmentResponse>(subjectVideoGroupAssignment.GetValueOrThrow()))
            : NotFound(subjectVideoGroupAssignment.GetErrorOrThrow());
    }

    [HttpPost]
    public async Task<ActionResult<SubjectVideoGroupAssignmentResponse>> AddSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignmentRequest subjectVideoGroupAssignmentRequest)
    {
        var result = await subjectVideoGroupAssignmentService.AddSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignmentRequest);

        if (result.IsFailure) 
            return BadRequest(result.GetErrorOrThrow());
        
        var createdSubjectVideoGroupAssignment = result.GetValueOrThrow();

        var subjectVideoGroupAssignmentResponse = mapper.Map<SubjectVideoGroupAssignmentResponse>(createdSubjectVideoGroupAssignment);

        return CreatedAtAction("GetSubjectVideoGroupAssignment", new { id = createdSubjectVideoGroupAssignment.Id }, subjectVideoGroupAssignmentResponse);

    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutSubjectVideoGroupAssignmentAsync(int id, SubjectVideoGroupAssignmentRequest subjectVideoGroupAssignmentRequest)
    {
        var result = await subjectVideoGroupAssignmentService.UpdateSubjectVideoGroupAssignmentAsync(id, subjectVideoGroupAssignmentRequest);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.GetErrorOrThrow());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSubjectVideoGroupAssignmentAsync(int id)
    {
        await subjectVideoGroupAssignmentService.DeleteSubjectVideoGroupAssignmentAsync(id);

        return NoContent();
    }

}