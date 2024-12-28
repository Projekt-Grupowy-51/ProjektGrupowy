using ProjektGrupowy.API.DTOs.SubjectVideoGroupAssignment;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services
{
    public interface ISubjectVideoGroupAssignmentService
    {
        Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsAsync();
        Task<Optional<SubjectVideoGroupAssignment>> GetSubjectVideoGroupAssignmentAsync(int id);
        Task<Optional<SubjectVideoGroupAssignment>> AddSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignmentRequest subjectVideoGroupAssignmentRequest);

        Task<Optional<SubjectVideoGroupAssignment>> UpdateSubjectVideoGroupAssignmentAsync(
            int subjectVideoGroupAssignmentId, SubjectVideoGroupAssignmentRequest subjectVideoGroupAssignmentRequest);
        Task DeleteSubjectVideoGroupAssignmentAsync(int subjectVideoGroupAssignmentId);
    }
}
