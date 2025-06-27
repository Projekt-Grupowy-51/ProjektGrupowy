using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

public interface ISubjectRepository
{
    Task<Optional<IEnumerable<Subject>>> GetSubjectsAsync();
    Task<Optional<Subject>> GetSubjectAsync(int id);
    Task<Optional<Subject>> AddSubjectAsync(Subject subject);
    Task<Optional<Subject>> UpdateSubjectAsync(Subject subject);

    Task<Optional<IEnumerable<Subject>>> GetSubjectsByProjectAsync(int projectId);

    Task DeleteSubjectAsync(Subject subject);
    Task<Optional<IEnumerable<Label>>> GetSubjectLabelsAsync(int subjectId);
}