using Microsoft.EntityFrameworkCore;
using VidMark.Application.Interfaces.Repositories;
using VidMark.Domain.Models;

namespace VidMark.Infrastructure.Persistance.Repositories;

public class AssignmentCompletionRepository(IReadWriteContext context) : IAssignmentCompletionRepository
{
    public Task<SubjectVideoGroupAssignmentCompletion?> GetByAssignmentAndLabelerAsync(int assignmentId, string labelerId)
    {
        return context.AssignmentCompletions
            .FirstOrDefaultAsync(ac => ac.SubjectVideoGroupAssignmentId == assignmentId && ac.LabelerId == labelerId);
    }

    public async Task AddAsync(SubjectVideoGroupAssignmentCompletion completion)
    {
        await context.AssignmentCompletions.AddAsync(completion);
    }

    public Task<List<SubjectVideoGroupAssignmentCompletion>> GetByAssignmentIdAsync(int assignmentId)
    {
        return context.AssignmentCompletions
            .Where(ac => ac.SubjectVideoGroupAssignmentId == assignmentId)
            .ToListAsync();
    }

    public Task<List<SubjectVideoGroupAssignmentCompletion>> GetByLabelerIdAsync(string labelerId)
    {
        return context.AssignmentCompletions
            .Include(ac => ac.Assignment)
                .ThenInclude(a => a.Subject)
            .Include(ac => ac.Assignment)
                .ThenInclude(a => a.VideoGroup)
            .Where(ac => ac.LabelerId == labelerId)
            .ToListAsync();
    }
}
