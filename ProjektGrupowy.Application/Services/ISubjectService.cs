using ProjektGrupowy.Application.DTOs.Subject;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Application.Services;

public interface ISubjectService
{
    Task<Optional<IEnumerable<Subject>>> GetSubjectsAsync();
    Task<Optional<Subject>> GetSubjectAsync(int id);
    Task<Optional<Subject>> AddSubjectAsync(SubjectRequest subjectRequest);
    Task<Optional<Subject>> UpdateSubjectAsync(int subjectId, SubjectRequest subjectRequest);

    Task<Optional<IEnumerable<Subject>>> GetSubjectsByProjectAsync(int projectId);

    Task DeleteSubjectAsync(int id);

    Task<Optional<IEnumerable<Label>>> GetSubjectLabelsAsync(int subjectId);
}