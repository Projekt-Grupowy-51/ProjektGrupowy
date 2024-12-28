using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.DTOs.Subject;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.API.Services;

namespace ProjektGrupowy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ValidateModelStateFilter))]
    public class SubjectController(ISubjectService subjectService, IMapper mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetSubjectsAsync()
        {
            var subjects = await subjectService.GetSubjectsAsync();
            return subjects.IsSuccess
                ? Ok(mapper.Map<IEnumerable<SubjectResponse>>(subjects.GetValueOrThrow()))
                : NotFound(subjects.GetErrorOrThrow());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectResponse>> GetSubjectAsync(int id)
        {
            var subject = await subjectService.GetSubjectAsync(id);
            return subject.IsSuccess
                ? Ok(mapper.Map<SubjectResponse>(subject.GetValueOrThrow()))
                : NotFound(subject.GetErrorOrThrow());
        }

        [HttpPost]
        public async Task<ActionResult<SubjectResponse>> AddSubjectAsync(SubjectRequest subjectRequest)
        {
            var result = await subjectService.AddSubjectAsync(subjectRequest);

            if (result.IsSuccess)
            {
                var createdSubject = result.GetValueOrThrow();

                var subjectResponse = mapper.Map<SubjectResponse>(createdSubject);

                return CreatedAtAction("GetSubject", new { id = createdSubject.Id }, subjectResponse);
            }

            return BadRequest(result.GetErrorOrThrow());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubjectAsync(int id, SubjectRequest subjectRequest)
        {
            var result = await subjectService.UpdateSubjectAsync(id, subjectRequest);

            return result.IsSuccess
                ? NoContent()
                : BadRequest(result.GetErrorOrThrow());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubjectAsync(int id)
        {
            await subjectService.DeleteSubjectAsync(id);

            return NoContent();
        }
    }
}
