using ProjektGrupowy.API.DTOs.SubjectVideoGroupAssignment;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services;

public interface ISubjectVideoGroupAssignmentService
{
    Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsAsync();
    Task<Optional<SubjectVideoGroupAssignment>> GetSubjectVideoGroupAssignmentAsync(int id);
    Task<Optional<SubjectVideoGroupAssignment>> AddSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignmentRequest subjectVideoGroupAssignmentRequest);

    Task<Optional<SubjectVideoGroupAssignment>> UpdateSubjectVideoGroupAssignmentAsync(
        int subjectVideoGroupAssignmentId, SubjectVideoGroupAssignmentRequest subjectVideoGroupAssignmentRequest);
    Task DeleteSubjectVideoGroupAssignmentAsync(int subjectVideoGroupAssignmentId);

    Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsByProjectAsync(int projectId);

    Task<Optional<IEnumerable<Labeler>>> GetSubjectVideoGroupAssignmentsLabelersAsync(int id);
    Task<Optional<SubjectVideoGroupAssignment>> AssignLabelerToAssignmentAsync(int assignmentId, int labelerId);
    Task<Optional<SubjectVideoGroupAssignment>> UnassignLabelerFromAssignmentAsync(int assignmentId, int labelerId);
    Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsByVideoGroupIdAsync(int videoGroupId);
    Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsByScientistIdAsync(int scientistId);
    Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetAssignmentsForLabelerAsync(int labelerId);
    Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsBySubjectIdAsync(int subjectId);
}