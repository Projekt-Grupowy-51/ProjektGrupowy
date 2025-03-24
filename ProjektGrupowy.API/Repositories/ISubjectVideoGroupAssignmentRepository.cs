using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Repositories;

public interface ISubjectVideoGroupAssignmentRepository
{
    Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsAsync();
    Task<Optional<SubjectVideoGroupAssignment>> GetSubjectVideoGroupAssignmentAsync(int id);
    Task<Optional<SubjectVideoGroupAssignment>> AddSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignment subjectVideoGroupAssignment);
    Task<Optional<SubjectVideoGroupAssignment>> UpdateSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignment subjectVideoGroupAssignment);
    Task DeleteSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignment subjectVideoGroupAssignment);

    Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsByProjectAsync(int projectId);
    Task<Optional<IEnumerable<Labeler>>> GetSubjectVideoGroupAssignmentsLabelersAsync(int id);
    Task<Optional<IEnumerable<AssignedLabel>>> GetSubjectVideoGroupAssignmentAsignedLabelsAsync(int id);
    Task<Optional<SubjectVideoGroupAssignment>> AssignLabelerToAssignmentAsync(int assignmentId, int labelerId);
    Task<Optional<SubjectVideoGroupAssignment>> UnassignLabelerFromAssignmentAsync(int assignmentId, int labelerId);
    Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsByVideoGroupIdAsync(int videoGroupId);
    Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsByScientistIdAsync(int scientistId);
    Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetAssignmentsForLabelerAsync(int labelerId);
    Task<Optional<IEnumerable<AssignedLabel>>> GetLabelerAssignedLabelsAsync(int assignmentId, int labelerId);
    Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsBySubjectIdAsync(int subjectId);
}