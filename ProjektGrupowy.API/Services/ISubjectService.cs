using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface ISubjectService
{
    Task<Optional<IEnumerable<Subject>>> GetSubjectsAsync();
    Task<Optional<Subject>> GetSubjectAsync(int id);
    Task<Optional<Subject>> AddSubjectAsync(Subject subject);
    Task<Optional<Subject>> UpdateSubjectAsync(Subject subject);
    Task DeleteSubjectAsync(int id);
}