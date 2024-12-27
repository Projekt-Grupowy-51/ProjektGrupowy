using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services
{
    public interface ISubjectVideoGroupAssignmentService
    {
        Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsAsync();
        Task<Optional<SubjectVideoGroupAssignment>> GetSubjectVideoGroupAssignmentAsync(int id);
        Task<Optional<SubjectVideoGroupAssignment>> AddSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignment subjectVideoGroupAssignment);
        Task<Optional<SubjectVideoGroupAssignment>> UpdateSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignment subjectVideoGroupAssignment);
        Task DeleteSubjectVideoGroupAssignmentAsync(int subjectVideoGroupAssignmentId);
    }
}
