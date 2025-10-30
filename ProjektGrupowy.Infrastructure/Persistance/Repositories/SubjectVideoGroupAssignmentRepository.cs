using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.Application.Interfaces.Repositories;
using ProjektGrupowy.Domain.Models;

namespace ProjektGrupowy.Infrastructure.Persistance.Repositories;

public class SubjectVideoGroupAssignmentRepository(IReadWriteContext context) : ISubjectVideoGroupAssignmentRepository
{
    public Task<List<SubjectVideoGroupAssignment>> GetSubjectVideoGroupAssignmentsAsync(string userId, bool isAdmin)
    {
        return context.SubjectVideoGroupAssignments.FilteredSubjectVideoGroupAssignments(userId, isAdmin)
                .ToListAsync();
    }

    public Task<SubjectVideoGroupAssignment> GetSubjectVideoGroupAssignmentAsync(int id, string userId, bool isAdmin)
    {
        return context.SubjectVideoGroupAssignments.FilteredSubjectVideoGroupAssignments(userId, isAdmin)
                .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddSubjectVideoGroupAssignmentAsync(SubjectVideoGroupAssignment subjectVideoGroupAssignment)
    {
        _ = await context.SubjectVideoGroupAssignments.AddAsync(subjectVideoGroupAssignment);
    }

    public void DeleteSubjectVideoGroupAssignment(SubjectVideoGroupAssignment subjectVideoGroupAssignment)
    {
        _ = context.SubjectVideoGroupAssignments.Remove(subjectVideoGroupAssignment);
    }

    public Task<List<SubjectVideoGroupAssignment>> GetSubjectVideoGroupAssignmentsByProjectAsync(int projectId, string userId, bool isAdmin)
    {
        return context.SubjectVideoGroupAssignments.FilteredSubjectVideoGroupAssignments(userId, isAdmin)
                .Where(x => x.Subject.Project.Id == projectId)
                .ToListAsync();
    }
    public Task<List<User>> GetSubjectVideoGroupAssignmentsLabelersAsync(int id, string userId, bool isAdmin)
    {
        return context.SubjectVideoGroupAssignments.FilteredSubjectVideoGroupAssignments(userId, isAdmin)
                .Where(x => x.Id == id)
                .SelectMany(x => x.Labelers)
                .ToListAsync();
    }
}