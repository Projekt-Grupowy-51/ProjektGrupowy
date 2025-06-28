using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Domain.Utils;

namespace ProjektGrupowy.Infrastructure.Repositories;

public interface ISubjectVideoGroupAssignmentRepository
{
    Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsAsync(string userId, bool isAdmin);
    Task<Optional<SubjectVideoGroupAssignment>> GetSubjectVideoGroupAssignmentAsync(int id, string userId, bool isAdmin);
    Task<Optional<SubjectVideoGroupAssignment>> AddSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignment subjectVideoGroupAssignment);
    Task<Optional<SubjectVideoGroupAssignment>> UpdateSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignment subjectVideoGroupAssignment);
    Task DeleteSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignment subjectVideoGroupAssignment);

    Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsByProjectAsync(int projectId, string userId, bool isAdmin);
    Task<Optional<IEnumerable<User>>> GetSubjectVideoGroupAssignmentsLabelersAsync(int id, string userId, bool isAdmin);
    Task<Optional<SubjectVideoGroupAssignment>> AssignLabelerToAssignmentAsync(int assignmentId, string labelerId, string userId, bool isAdmin);
    Task<Optional<SubjectVideoGroupAssignment>> UnassignLabelerFromAssignmentAsync(int assignmentId, string labelerId, string userId, bool isAdmin);
}