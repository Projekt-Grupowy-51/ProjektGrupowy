using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

public interface ISubjectRepository
{
    Task<Optional<IEnumerable<Subject>>> GetSubjectsAsync(string userId, bool isAdmin);
    Task<Optional<Subject>> GetSubjectAsync(int id, string userId, bool isAdmin);
    Task<Optional<Subject>> AddSubjectAsync(Subject subject);
    Task<Optional<Subject>> UpdateSubjectAsync(Subject subject);

    Task<Optional<IEnumerable<Subject>>> GetSubjectsByProjectAsync(int projectId, string userId, bool isAdmin);

    Task DeleteSubjectAsync(Subject subject);
    Task<Optional<IEnumerable<Label>>> GetSubjectLabelsAsync(int subjectId, string userId, bool isAdmin);
}