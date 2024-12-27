using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Repositories;
using ProjektGrupowy.API.Utils;

namespace ProjektGrupowy.API.Services.Impl
{
    public class SubjectVideoGroupAssignmentService(ISubjectVideoGroupAssignmentRepository subjectVideoGroupAssignmentRepository) : ISubjectVideoGroupAssignmentService
    {
        public async Task<Optional<IEnumerable<SubjectVideoGroupAssignment>>> GetSubjectVideoGroupAssignmentsAsync()
        {
            return await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentsAsync();
        }

        public async Task<Optional<SubjectVideoGroupAssignment>> GetSubjectVideoGroupAssignmentAsync(int id)
        {
            return await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(id);
        }

        public async Task<Optional<SubjectVideoGroupAssignment>> AddSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignment subjectVideoGroupAssignment)
        {
            return await subjectVideoGroupAssignmentRepository.AddSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignment);
        }

        public async Task<Optional<SubjectVideoGroupAssignment>> UpdateSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignment subjectVideoGroupAssignment)
        {
            return await subjectVideoGroupAssignmentRepository.UpdateSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignment);
        }

        public async Task DeleteSubjectVideoGroupAssignmentAsync(int subjectVideoGroupAssignmentId)
        {
            var subjectVideoGroupAssignment = await subjectVideoGroupAssignmentRepository.GetSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignmentId);
            if (subjectVideoGroupAssignment.IsSuccess)
            {
                await subjectVideoGroupAssignmentRepository.DeleteSubjectVideoGroupAssignmentAsync(subjectVideoGroupAssignment.GetValueOrThrow());
            }
        }
    }
}
