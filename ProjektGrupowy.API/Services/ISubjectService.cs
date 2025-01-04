using ProjektGrupowy.API.DTOs.Subject;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface ISubjectService
{
    Task<Optional<IEnumerable<Subject>>> GetSubjectsAsync();
    Task<Optional<Subject>> GetSubjectAsync(int id);
    Task<Optional<Subject>> AddSubjectAsync(SubjectRequest subjectRequest);
    Task<Optional<Subject>> UpdateSubjectAsync(int subjectId, SubjectRequest subjectRequest);

    Task<Optional<IEnumerable<Subject>>> GetSubjectsByProjectAsync(int projectId);
    Task<Optional<IEnumerable<Subject>>> GetSubjectsByProjectAsync(Project project);

    Task DeleteSubjectAsync(int id);
}