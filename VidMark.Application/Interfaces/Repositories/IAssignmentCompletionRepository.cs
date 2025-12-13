using VidMark.Domain.Models;

namespace VidMark.Application.Interfaces.Repositories;

public interface IAssignmentCompletionRepository
{
    Task<SubjectVideoGroupAssignmentCompletion?> GetByAssignmentAndLabelerAsync(int assignmentId, string labelerId);
    Task AddAsync(SubjectVideoGroupAssignmentCompletion completion);
    Task<List<SubjectVideoGroupAssignmentCompletion>> GetByAssignmentIdAsync(int assignmentId);
    Task<List<SubjectVideoGroupAssignmentCompletion>> GetByLabelerIdAsync(string labelerId);
}
