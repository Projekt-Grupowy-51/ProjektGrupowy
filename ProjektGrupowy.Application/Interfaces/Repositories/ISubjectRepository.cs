using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Interfaces.Repositories;

public interface ISubjectRepository
{
    Task<List<Subject>> GetSubjectsAsync(string userId, bool isAdmin);
    Task<Subject> GetSubjectAsync(int id, string userId, bool isAdmin);
    Task AddSubjectAsync(Subject subject);
    Task<List<Subject>> GetSubjectsByProjectAsync(int projectId, string userId, bool isAdmin);
    void DeleteSubject(Subject subject);
    Task<List<Label>> GetSubjectLabelsAsync(int subjectId, string userId, bool isAdmin);
}