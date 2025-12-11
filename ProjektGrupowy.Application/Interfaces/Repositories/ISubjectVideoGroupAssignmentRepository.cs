using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Application.Interfaces.Repositories;

public interface ISubjectVideoGroupAssignmentRepository
{
    Task<List<SubjectVideoGroupAssignment>> GetSubjectVideoGroupAssignmentsAsync(string userId, bool isAdmin);
    Task<SubjectVideoGroupAssignment> GetSubjectVideoGroupAssignmentAsync(int id, string userId, bool isAdmin);
    Task AddSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignment subjectVideoGroupAssignment);
    void DeleteSubjectVideoGroupAssignment(SubjectVideoGroupAssignment subjectVideoGroupAssignment);

    Task<List<SubjectVideoGroupAssignment>> GetSubjectVideoGroupAssignmentsByProjectAsync(int projectId, string userId, bool isAdmin);
    Task<List<User>> GetSubjectVideoGroupAssignmentsLabelersAsync(int id, string userId, bool isAdmin);
}